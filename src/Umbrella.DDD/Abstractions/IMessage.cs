using System;
using System.Runtime.CompilerServices;

namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Definition of a message, base for message-driven applications
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Unique Identifier for a given message
        /// </summary>
        /// <value></value>
        string ID { get; }
        /// <summary>
        /// Contento of the message
        /// </summary>
        /// <value></value>
        object Body { get; }
        /// <summary>
        /// Timestamp of emission timeframe
        /// </summary>
        /// <value></value>
        DateTime PublishedOn { get; }
    }
}