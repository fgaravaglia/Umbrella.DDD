using System;
using System.Collections.Generic;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD
{
    /// <summary>
    /// Simplest implementation of publisher, where configuration is stored in memory
    /// </summary>
    /// <remarks>Useful only for POC/Demo purposes. DO not use it in PROD</remarks>
    public class InMemoryPublisher : IEventPublisher
    {
        readonly Dictionary<string, string> _Topics;
        readonly Dictionary<string, List<IMessage>> _MessagesPerTopic;

        /// <summary>
        /// Empty Constr
        /// </summary>
        public InMemoryPublisher()
        {
            _Topics = new Dictionary<string, string>();
            _MessagesPerTopic = new Dictionary<string, List<IMessage>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string PublishMessage(IMessage msg)
        {
            if (msg == null)
                throw new ArgumentNullException(nameof(msg));
            var targetType = msg.GetType();

            //If key exists, a topic has bee identiefied
            if (_Topics.ContainsKey(targetType.FullName))
            {
                var topic = _Topics[targetType.FullName];
                _MessagesPerTopic[topic].Add(msg);
            }
            return msg.ID;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <typeparam name="T"></typeparam>
        public void UsingThisQueueFor<T>(string queueName) where T : IMessage
        {
            this.UsingThisQueueFor(typeof(T).FullName, queueName);
        }
        /// <summary>
        /// Sets the target queue or topic for a given type
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public void UsingThisQueueFor(string eventType, string queueName)
        {
            if (String.IsNullOrEmpty(eventType))
                throw new ArgumentNullException(nameof(eventType));
            if (String.IsNullOrEmpty(queueName))
                throw new ArgumentNullException(nameof(queueName));
            if (_Topics.ContainsKey(eventType))
                throw new InvalidOperationException($"Unable to set the Topic {queueName} for type {eventType}: type already assigned");
            _Topics.Add(eventType, queueName);
        }
    }
}