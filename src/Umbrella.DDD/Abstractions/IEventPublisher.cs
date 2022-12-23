namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Abstraction for component dedicated to publish messages
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes the message in a proper queue or Topic
        /// </summary>
        /// <param name="msg">message to be published</param>
        /// <returns>ID of puplished message</returns>
        string PublishEvent<T>(T msg) where T : IMessage;
        /// <summary>
        /// Sets the target queue or topic for a given type
        /// </summary>
        /// <param name="queueName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        void UsingThisQueueFor<T>(string queueName) where T : IMessage;

    }
}