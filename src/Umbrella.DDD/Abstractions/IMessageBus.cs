namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Abstraction for a MEssage Bus, to manage messages across domain
    /// </summary>
    public interface IMessageBus : IEventPublisher
    {
         
    }
}