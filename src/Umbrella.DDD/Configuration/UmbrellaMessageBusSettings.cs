using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DDD.Configuration
{
    /// <summary>
    /// MessageBus settings
    /// <example>
    /// this class is mapping the appSettings.json section:
    /// <code>
    /// 
    /// "UmbrellaMessageBus": {
    ///    "Publisher": "InMemory",
    ///    "Queues": [
    ///     {
    ///         "EventType": "NewHeadacheEpisodeAdded",
    ///         "QueueName": "pub001-prod-headache-episode-added"
    ///     }]
    /// }
    /// 
    /// </code>
    /// </example>
    /// </summary>
    public class UmbrellaMessageBusSettings
    {
        /// <summary>
        /// Name of queue
        /// </summary>
        /// <value></value>
        public string Publisher { get; set; }
        /// <summary>
        /// COnfigures Queues
        /// </summary>
        /// <value></value>
        public List<QueueSettings> Queues { get; set; }
        /// <summary>
        /// TRUE if publisher in in memory
        /// </summary>
        /// <value></value>
        public bool IsInMemory
        {
            get { return !String.IsNullOrEmpty(this.Publisher) && this.Publisher.Equals(PublisherNames.IN_MEMEORY, StringComparison.InvariantCultureIgnoreCase); }
        }
        /// <summary>
        /// EMpty COnstr
        /// </summary>
        public UmbrellaMessageBusSettings()
        {
            this.Queues = new List<QueueSettings>();
            this.Publisher = "";
        }
    }
    /// <summary>
    /// Queue settings
    /// </summary>
    public class QueueSettings
    {
        /// <summary>
        /// Name of queue
        /// </summary>
        /// <value></value>
        public string QueueName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string EventType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QueueSettings()
        {
            this.QueueName = "";
            this.EventType = "";
        }
    }
}
