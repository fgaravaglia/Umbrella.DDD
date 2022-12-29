using System;
using System.Collections.Generic;
using System.Linq;

namespace Umbrella.DDD.Abstractions.Domains
{
    /// <summary>
    /// base implementation of Business domain entity
    /// </summary>
    public abstract class Entity<Tdto> : IEntity<Tdto>
        where Tdto : IEntityDto
    {
        #region Attributes
        /// <summary>
        /// List of event not already eprsisted
        /// </summary>
        protected readonly List<IMessage> _UncommittedEvents;
        /// <summary>
        /// Representation of status of given entity by its DTO
        /// </summary>
        /// <value></value>
        protected Tdto? _PlainStatus;
        #endregion

        #region  Properties
        /// <summary>
        /// Unique identifier
        /// </summary>
        /// <value></value>
        public string ID { get; protected set; }
        /// <summary>
        /// Creation date of entity
        /// </summary>
        /// <value></value>
        public DateTime CreatedOn { get; protected set; }
        /// <summary>
        /// Las update timestamp of entity
        /// </summary>
        /// <value></value>
        public DateTime? LastUpdatedOn { get; protected set; }
        /// <summary>
        /// TRUE if there are some not committed changes; FALSE otherwise
        /// </summary>
        /// <value></value>
        public bool HasChanged { get { return GetUncommittedChanges().Any(); } }

        #endregion

        /// <summary>
        /// Default Constr
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected Entity(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            ID = id;
            CreatedOn = DateTime.Now;
            LastUpdatedOn = null;
            _UncommittedEvents = new List<IMessage>();
            this._PlainStatus = default(Tdto);
        }

        /// <summary>
        /// Gets the list of not-already-committed events
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMessage> GetUncommittedChanges()
        {
            var list = new List<IMessage>();
            list.AddRange(_UncommittedEvents);
            return list;
        }
        /// <summary>
        /// mark the changes as committed, to reset entity status
        /// </summary>
        public void ChangesCommitted()
        {
            _UncommittedEvents.Clear();
        }
        /// <summary>
        /// Updates the LastUpdateDate
        /// </summary>
        protected void SetAsModified()
        {
            LastUpdatedOn = DateTime.Now;
        }
        /// <summary>
        /// Restores the entity status from DTO
        /// </summary>
        /// <param name="dto"></param>
        public virtual void SetStatusFromDTO(Tdto dto)
        {
            this._PlainStatus = dto;
            this.ID = dto.ID;
            this.CreatedOn = dto.CreatedOn;
            this.LastUpdatedOn = dto.LastUpdatedOn;
        }
        /// <summary>
        /// converts the entity ro DTO obj
        /// </summary>
        /// <param name="dto"></param>
        public virtual Tdto ToDTO()
        {
            return this._PlainStatus;
        }
    }
}