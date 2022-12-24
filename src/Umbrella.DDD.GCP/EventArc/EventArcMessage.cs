using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Umbrella.DDD.GCP.EventArc;

namespace Umbrella.DDD.GCP.EventArc
{
    /// <summary>
    /// MEssage defined by Event Arc protocol
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class EventArcEventMessage
    {
        /// <summary>
        /// original message published on Pub Sub topic
        /// </summary>
        /// <value></value>
        [JsonPropertyName("message")]
        public PubSubMessage Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [JsonPropertyName("subscription")]
        public string Subscription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public string ContextSource { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public string ContexType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public string ContextSpecVersion { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public string ContextPublishTime { get; private set; }

        /// <summary>
        /// Default Constr
        /// </summary>
        public EventArcEventMessage()
        {
            this.Subscription = "";
            this.Message = new PubSubMessage();

            // CE attributes
            this.ContextSource = "";
            this.ContexType = "";
            this.ContextSpecVersion = "";
            this.ContextPublishTime = "";
        }
        /// <summary>
        /// Set context attributes defined by Event Arc Protocol
        /// </summary>
        /// <param name="ceHeaders"></param>
        public void SetCEAttributes(Dictionary<string, string> ceHeaders)
        {
            if (ceHeaders is null)
                throw new ArgumentNullException(nameof(ceHeaders));

            if (!ceHeaders.ContainsKey("ce-source"))
                throw new ApplicationException($"Header ce-source cannot be null");
            if (!ceHeaders.ContainsKey("ce-type"))
                throw new ApplicationException($"Header ce-type cannot be null");
            if (!ceHeaders.ContainsKey("ce-time"))
                throw new ApplicationException($"Header ce-time cannot be null");
            if (!ceHeaders.ContainsKey("ce-specversion"))
                throw new ApplicationException($"Header ce-specversion cannot be null");

            var cesource = ceHeaders["ce-source"];
            var ceType = ceHeaders["ce-type"];
            var ceTime = ceHeaders["ce-time"];
            var ceSpecVersion = ceHeaders["ce-specversion"];

            if (string.IsNullOrEmpty(cesource))
                throw new ApplicationException($"Header ce-source cannot be null");
            if (string.IsNullOrEmpty(ceType))
                throw new ApplicationException($"Header ce-type cannot be null");
            if (string.IsNullOrEmpty(ceTime))
                throw new ApplicationException($"Header ce-time cannot be null");
            if (string.IsNullOrEmpty(ceSpecVersion))
                throw new ApplicationException($"Header ce-specversion cannot be null");

            this.ContextSource = cesource;
            this.ContexType = ceType;
            this.ContextSpecVersion = ceSpecVersion;
            this.ContextPublishTime = ceTime;
        }
    }

    
}