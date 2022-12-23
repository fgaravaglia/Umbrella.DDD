using System;

namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Abstraction for handler messages
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles the message
        /// </summary>
        void Handle(object message);
    }

    /// <summary>
    /// Abstraction for handler messages
    /// </summary>
    public interface IMessageHandler<in T> : IMessageHandler where T : IMessage
    {
        /// <summary>
        /// verifies if handler manages the current event or not
        /// </summary>
        /// <param name="message"></param>
        /// <returns>TRUE if the mesage interacts with the handler; FALSE otherwise</returns>
        bool CanHandleThisMessage(IMessage  message);
        /// <summary>
        /// Handles the message in safemode, catching any raised exception
        /// </summary>
        /// <param name="message"></param>
        /// <returns>NULL if no error occurred. An Exception otherwise</returns>
        Exception? TryHandleThisMessage(T message);
    }
}