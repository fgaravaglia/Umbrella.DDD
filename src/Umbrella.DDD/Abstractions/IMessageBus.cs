namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Abstraction for a MEssage Bus, to manage messages across domain
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Publishes the message in a proper queue or Topic
        /// </summary>
        /// <param name="msg">message to be published</param>
        /// <returns>ID of puplished message</returns>
        string PublishMessage(IMessage msg);
    }
}