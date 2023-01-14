using System;
using System.Diagnostics.CodeAnalysis;

namespace Umbrella.DDD.Abstractions
{

    /// <summary>
    /// Base implementation of a message
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class Message : IMessage
    {
        /// <summary>
        /// Unique Identifier for a given message
        /// </summary>
        /// <value></value>
        public string ID { get; private set; }
        /// <summary>
        /// Contento of the message
        /// </summary>
        /// <value></value>
        public object Body { get; protected set; }
        /// <summary>
        /// Timestamp of emission timeframe
        /// </summary>
        /// <value></value>
        public DateTime PublishedOn { get; private set; }
        /// <summary>
        /// message Type
        /// </summary>
        /// <value></value>
        public string MessageType { get; private set; }

        /// <summary>
        /// Empty COnstr for serialization
        /// </summary>
        protected Message()
        {
            this.ID = Guid.NewGuid().ToString();
            this.PublishedOn = DateTime.Now;
            this.Body = "";
            this.MessageType = "";
        }
        /// <summary>
        /// Empty Constr
        /// </summary>
        /// <param name="messageType"></param>
        protected Message(string messageType) : this()
        {
            if (String.IsNullOrEmpty(messageType))
                throw new ArgumentNullException(nameof(messageType));
            this.MessageType = messageType;
        }
    }

}