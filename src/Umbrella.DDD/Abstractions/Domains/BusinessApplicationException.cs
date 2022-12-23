using System;

namespace Umbrella.DDD.Abstractions.Domains
{
    /// <summary>
    /// Exception rasied by Entityt inside domain model
    /// </summary>
    public class BusinessApplicationException : ApplicationException
    {
        /// <summary>
        /// ID of eneity that raised exception
        /// </summary>
        /// <value></value>
        public string EntityId { get; private set; }
        /// <summary>
        /// Type of entity that raised exception
        /// </summary>
        /// <value></value>
        public Type EntityType { get; private set; }


        /// <summary>
        /// Default Constr
        /// </summary>
        /// <param name="message"></param>
        /// <param name="entityId"></param>
        /// <param name="entityType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BusinessApplicationException(string message, string entityId, Type entityType) : base(message)
        {
            if (String.IsNullOrEmpty(entityId))
                throw new ArgumentNullException(nameof(entityId));
            if (entityType is null)
                throw new ArgumentNullException(nameof(entityType));
            EntityId = entityId;
            EntityType = entityType;
        }
        /// <summary>
        /// Creates the exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="entityId"></param>
        /// <param name="entityType"></param>
        /// <param name="ex"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BusinessApplicationException(string message, string entityId, Type entityType, Exception ex) : base(message, ex)
        {
            if (String.IsNullOrEmpty(entityId))
                throw new ArgumentNullException(nameof(entityId));
            if (entityType is null)
                throw new ArgumentNullException(nameof(entityType));
            EntityId = entityId;
            EntityType = entityType;
        }
    }
}