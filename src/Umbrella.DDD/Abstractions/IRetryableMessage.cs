namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Abstraction for messages that can be re-tried more than once
    /// </summary>
    public interface IRetryableMessage : IMessage
    {
        /// <summary>
        /// counter for total number of executions
        /// </summary>
        /// <value></value>

        int CurrentRetry { get; }
        /// <summary>
        /// Number of max retry before ignoreid (Dead Letter Queue)
        /// </summary>
        /// <value></value>
        int MaxRetry { get; }
        /// <summary>
        /// True if currentRetry is less than maxretry, false otherwise
        /// </summary>
        /// <value></value>
        bool CanRetry { get; }
    }
}