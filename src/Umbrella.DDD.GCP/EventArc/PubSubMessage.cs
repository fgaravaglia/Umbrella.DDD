using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Umbrella.DDD.GCP.EventArc
{
    /// <summary>
    /// Object to map the json published on PubSUb topic
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class PubSubMessage
    {
        /// <summary>
        /// BASE-64 representation of message data
        /// </summary>
        /// <value></value>
        [JsonPropertyName("data")]
        public string data { get; set; }
        /// <summary>
        /// JSon represnetation of data
        /// </summary>
        /// <value></value>
        [JsonIgnore]
        public string JsonData
        {
            get
            {
                // parse base64 string
                if (String.IsNullOrEmpty(data))
                    return String.Empty;

                byte[] realMessageBytes = Convert.FromBase64String(this.data);
                return Encoding.UTF8.GetString(realMessageBytes);
            }
        }
        /// <summary>
        /// ID of publshed message
        /// </summary>
        /// <value></value>
        [JsonPropertyName("messageId")]
        public string MessageId { get; set; }
        /// <summary>
        /// Timestampe of publishing timeframe
        /// </summary>
        /// <value></value>
        [JsonPropertyName("publishTime")]
        public string publishTimeString { get; set; }

        /// <summary>
        /// Empty Const
        /// </summary>
        public PubSubMessage()
        {
            this.data = "";
            this.MessageId = "";
            this.publishTimeString = "";
        }
        /// <summary>
        /// Casts the jsondata into concrete object of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? As<T>()
        {
            if (String.IsNullOrEmpty(this.JsonData))
                throw new ApplicationException($"Unable to Cast null json into {typeof(T).Name}");

            return JsonSerializer.Deserialize<T>(JsonData);
        }
    }
}