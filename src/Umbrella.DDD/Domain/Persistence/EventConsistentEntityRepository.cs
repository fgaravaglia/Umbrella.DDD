using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.Abstractions.Domains;

namespace Umbrella.DDD.Domain.Persistence
{
    /// <summary>
    /// Base implementation of repository that keeps event consistency of the entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Tdto"></typeparam>
    public class EventConsistentEntityRepository<T> : IEntityRepository<T>
        where T : IEntity
    {
        #region protected methods
        readonly protected IEntityRepository<T> _Repository;
        readonly protected IMessageBus _Bus;
        readonly protected ILogger _Logger;
        #endregion

        /// <summary>
        /// Default constr
        /// </summary>
        /// <param name="repo"></param>
        protected EventConsistentEntityRepository(ILogger logger, IEntityRepository<T> repo, IMessageBus bus)
        {
            this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._Repository = repo ?? throw new ArgumentNullException(nameof(repo));
            this._Bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        /// <summary>
        /// Gets the entity from persistence
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T? GetById(string id)
        {
            return this._Repository.GetById(id);
        }
        /// <summary>
        /// Saves the entity. Since this repository keeps the event consistency, all uncommitted changes are publishes using MessageBus
        /// </summary>
        /// <param name="entity"></param>
        public string Save(T entity)
        {
           var entityId = this._Repository.Save(entity);

           // gets the uncommitted events
           var events = entity.GetUncommittedChanges();
           foreach(var msg in events)
           {
                try
                {
                    this._Bus.PublishMessage(msg);
                }
                catch(Exception ex)
                {
                    this._Logger.LogError(ex, "unexpected error publishing event {eventId} - {eventType} after saving entity {entityId}",
                                                msg.ID, msg.GetType(), entity.ID);
                }
           }

           return entityId;
        }
    }
}