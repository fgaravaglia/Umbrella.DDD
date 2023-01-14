namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Base implementation of a retryable message
    /// </summary>
    public abstract class RetryableMessage : Message, IRetryableMessage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int CurrentRetry { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int MaxRetry { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public bool CanRetry { get { return this.CurrentRetry < MaxRetry; } }

        /// <summary>
        /// Empty COnstr for serialization
        /// </summary>
        /// <returns></returns>
        protected RetryableMessage() : base()
        {
            this.MaxRetry = 5;
            this.CurrentRetry = 0;
        }
        /// <summary>
        /// default Constr
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        protected RetryableMessage(string messageType) : base(messageType)
        {
            this.MaxRetry = 5;
            this.CurrentRetry = 0;
        }
    }
}