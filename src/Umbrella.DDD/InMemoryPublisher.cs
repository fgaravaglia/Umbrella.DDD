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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string PublishEvent<T>(T msg) where T : IMessage
        {
            if (msg == null)
                throw new ArgumentNullException(nameof(msg));

            //If key exists, a topic has bee identiefied
            if (_Topics.ContainsKey(typeof(T)))
            {
                var topic = _Topics[typeof(T)];
                _MessagesPerTopic[topic].Add(msg);
            }
            else
                Console.WriteLine("WARN NO topic found for " + typeof(T));
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