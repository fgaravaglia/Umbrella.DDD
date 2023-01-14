using System;
using System.Text.Json;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Umbrella.DDD.Abstractions;
using IMessage = Umbrella.DDD.Abstractions.IMessage;

namespace Umbrella.DDD.GCP
{
    /// <summary>
    /// Event Publihser based on Google Pub Sub
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        readonly ILogger _Logger;
        readonly Dictionary<Type, string> _Topics;
        readonly string _ProjectId;
        readonly PublisherServiceApiClient _Publisher;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="projectID"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EventPublisher(ILogger logger, string projectID)
        {
            this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (String.IsNullOrEmpty(projectID))
                throw new ArgumentNullException(nameof(projectID));
            this._ProjectId = projectID;
            this._Topics = new Dictionary<Type, string>();
            this._Publisher = PublisherServiceApiClient.Create();
        }

        #region Private Methods

        private TopicName GetTopicName(string topicId)
        {
            if (String.IsNullOrEmpty(topicId))
                throw new ArgumentNullException(nameof(topicId));

            // get topic name
            TopicName topicName = new TopicName(this._ProjectId, topicId);
            if (topicName == null)
                throw new InvalidOperationException($"Unable to create Topic {topicId}: the item should be created before subscribing");
            return topicName;
        }

        private static string ToJson<T>(T msg) where T : IMessage
        {

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize<T>(msg, options);
        }
        /// <summary>
        /// Publishes a single message
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="jsonMessage"></param>
        /// <param name="attributes"></param>
        /// <returns>id of published message</returns>
        string PublishMessageOnTopic(string topicId, string jsonMessage, Dictionary<string, string> attributes)
        {
            // check existence
            if (this._Publisher == null)
                throw new NullReferenceException("Unexpected null Publisher here");

            // get topic name
            TopicName topicName = GetTopicName(topicId);

            // Publish a message to the topic.
            PubsubMessage pubSubMessage = new PubsubMessage()
            {
                // The data is any arbitrary ByteString. Here, we're using text.
                Data = ByteString.CopyFromUtf8(jsonMessage),
            };
            if (attributes != null)
            {
                foreach (var k in attributes)
                    pubSubMessage.Attributes.Add(k.Key, k.Value);
            }

            this._Logger.LogDebug("Message converted into PubSub Format");
            var response = this._Publisher.Publish(topicName, new[] { pubSubMessage });
            var messageId = response.MessageIds.Select(x => x).First();
            this._Logger.LogInformation("Message succesfully published with Id '{messageId}'", messageId);

            return messageId;
        }

        #endregion

        /// <summary>
        /// <inheritdoc cref="IEventPublisher.PublishMessage(IMessage)"/>
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public string PublishMessage(IMessage msg)
        {
            if (msg == null)
                throw new ArgumentNullException(nameof(msg));
            if (String.IsNullOrEmpty(msg.ID))
                throw new ArgumentNullException(nameof(msg), "MessageID cannot be null");
            if (msg.ID.ToLowerInvariant() == Guid.Empty.ToString().ToLowerInvariant())
                throw new ArgumentException("Message ID cannot be Empty", nameof(msg));

            var messageType = msg.GetType();
            if (!this._Topics.ContainsKey(messageType))
            {
                this._Logger.LogInformation("{Type} has not been registered. the handler is supposed to be on memory",messageType);
                return Guid.NewGuid().ToString();
            }

            // get the topic, than publish
            var topic = this._Topics[messageType];
            string jsonMsg = ToJson(msg);
            this._Logger.LogInformation("Publishing message {messageType} on topic {targetTopic}", messageType, topic);
            string msgId = PublishMessageOnTopic(topic, jsonMsg, new Dictionary<string, string>());
            this._Logger.LogInformation("Message succesfully published");
            return msgId;
        }
        /// <summary>
        /// <inheritdoc cref="IEventPublisher.UsingThisQueueFor{T}(string)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void UsingThisQueueFor<T>(string queueName) where T : IMessage
        {
            if (this._Topics.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"Unable to set the Topic {queueName} for type {typeof(T).FullName}: type already assigned");
            this._Topics.Add(typeof(T), queueName);
        }
        /// <summary>
        /// Creates the topic
        /// </summary>
        /// <param name="topicId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void CreateTopic(string topicId)
        {
            if (String.IsNullOrEmpty(topicId))
                throw new ArgumentNullException(nameof(topicId));

            // First create a topic.
            TopicName topicName = new TopicName(this._ProjectId, topicId);
            this._Publisher.CreateTopic(topicName);
        }
    }
}