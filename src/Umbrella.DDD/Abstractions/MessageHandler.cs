using System;

namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Base class to implement a specific messageHandler
    /// </summary>
    public abstract class MessageHandler<T> : IMessageHandler<T> where T : IMessage
    {
        /// <summary>
        /// Empty Constr
        /// </summary>
        protected MessageHandler()
        {

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
        public void Handle(object message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));
            if (message.GetType() != typeof(T))
                throw new ArgumentException($"Wrong Message: this handler can manage only message of type {typeof(T)}", nameof(message));

            this.HandleThisMessage((T)message);
        }
        /// <summary>
        /// <inheritdoc cref="IMessageHandler{T}.HandleThisMessage(T)"/>
        /// </summary>
        /// <param name="message"></param>
        public abstract void HandleThisMessage(T message);
        /// <summary>
        /// <inheritdoc cref="IMessageHandler{T}.TryHandleThisMessage(T)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Exception? TryHandleThisMessage(T message)
        {
            try
            {
                this.HandleThisMessage(message);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}