using System;

namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Abstraction for handler messages
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// verifies if handler manages the current event or not
        /// </summary>
        /// <param name="message"></param>
        /// <returns>TRUE if the mesage interacts with the handler; FALSE otherwise</returns>
        bool CanHandleThisMessage(IMessage message);
        /// <summary>
        /// Handles the message
        /// </summary>
        void Handle(IMessage message);
        /// <summary>
        /// Handles the message in safemode, catching any raised exception
        /// </summary>
        /// <param name="message"></param>
        /// <returns>NULL if no error occurred. An Exception otherwise</returns>
        Exception? TryHandleMessage(IMessage message);
    }

    /// <summary>
    /// Abstraction for handler messages
    /// </summary>
    public interface IMessageHandler<T> : IMessageHandler where T : IMessage
    {
        /// <summary>
        /// Handles the message in safemode, catching any raised exception
        /// </summary>
        /// <param name="message"></param>
        /// <returns>NULL if no error occurred. An Exception otherwise</returns>
        Exception? TryHandleThisMessage(T message);
    }
}