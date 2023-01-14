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
        readonly Dictionary<Type, string> _Topics;
        readonly Dictionary<string, List<IMessage>> _MessagesPerTopic;

        /// <summary>
        /// Empty Constr
        /// </summary>
        public InMemoryPublisher()
        {
            _Topics = new Dictionary<Type, string>();
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
            if (_Topics.ContainsKey(targetType))
            {
                var topic = _Topics[targetType];
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
            if (_Topics.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"Unable to set the Topic {queueName} for type {typeof(T).FullName}: type already assigned");
            _Topics.Add(typeof(T), queueName);
        }
    }
}