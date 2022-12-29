using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DDD.Abstractions.Domains
{
    /// <summary>
    /// Abstraciont for simple plain object that maps the Entity
    /// </summary>
    public interface IEntityDto
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
    }
}