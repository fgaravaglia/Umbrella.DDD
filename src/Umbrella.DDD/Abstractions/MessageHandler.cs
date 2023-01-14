using System;
using Microsoft.Extensions.Logging;

namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Base class to implement a specific messageHandler
    /// </summary>
    public abstract class MessageHandler<T> : IMessageHandler<T> where T : IMessage
    {
        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger _Logger;
        
        /// <summary>
        /// Empty Constr
        /// </summary>
        protected MessageHandler(ILogger logger)
        {
            this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        /// <summary>
        /// Checks if current handler can manage the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual bool CanHandleThisMessage(IMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            return true;
        }
        /// <summary>
        /// Handles the message
        /// </summary>
        /// <param name="message"></param>
        public void Handle(IMessage message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));
            if (message.GetType() != typeof(T))
                throw new ArgumentException($"Wrong Message: this handler can manage only message of type {typeof(T)}", nameof(message));
            if(!CanHandleThisMessage(message))
                throw new ArgumentException($"Wrong Message: this handler cannot handle the message of type {message.GetType()}");

            this.HandleThisMessage((T)message);
        }
        /// <summary>
        /// <inheritdoc cref="IMessageHandler{T}.HandleThisMessage(T)"/>
        /// </summary>
        /// <param name="message"></param>
        public abstract void HandleThisMessage(T message);
        /// <summary>
        /// <inheritdoc cref="IMessageHandler{T}.TryHandleMessage(IMessage)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Exception? TryHandleMessage(IMessage message)
        {
            try
            {
                this.Handle(message);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        /// <summary>
        /// Handles the message in safemode, catching any raised exception
        /// </summary>
        /// <param name="message"></param>
        /// <returns>NULL if no error occurred. An Exception otherwise</returns>
        public Exception? TryHandleThisMessage(T message)
        {
            return TryHandleMessage(message);
        }
    }
}