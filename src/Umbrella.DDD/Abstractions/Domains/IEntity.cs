using System;
using System.Collections.Generic;

namespace Umbrella.DDD.Abstractions.Domains
{
    /// <summary>
    /// Abstraction for a given entity of domain
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        /// <value></value>
        string ID { get; }
        /// <summary>
        /// Creation date of entity
        /// </summary>
        /// <value></value>
        DateTime CreatedOn { get; }
        /// <summary>
        /// Las update timestamp of entity
        /// </summary>
        /// <value></value>
        DateTime? LastUpdatedOn { get; }
        /// <summary>
        /// TRUE if there are some not committed changes; FALSE otherwise
        /// </summary>
        /// <value></value>
        bool HasChanged { get; }

        /// <summary>
        /// Gets the list of not-already-committed events
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMessage> GetUncommittedChanges();
        /// <summary>
        /// mark the changes as committed, to reset entity status
        /// </summary>
        void ChangesCommitted();
        /// <summary>
        /// Restores the entity status from DTO
        /// </summary>
        /// <param name="dto"></param>
        void SetStatusFromDTO(object dto);
        /// <summary>
        /// converts the entity ro DTO obj
        /// </summary>
        /// <param name="dto"></param>
        object ToDTO();
    }
}