using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD.Api.TestScenarios
{
    public class TestEventOccurred : RetryableMessage
    {

        /// <summary>
        /// Entity target
        /// </summary>
        /// <value></value>
        public string EntityId { get; private set; }
        /// <summary>
        /// Body casted into proper DTO
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public string TextValue { get { return (string)this.Body; } }

        /// <summary>
        /// Empty COnstr
        /// </summary>
        /// <returns></returns>
        public TestEventOccurred() : base()
        {
            this.EntityId = "";
        }
        /// <summary>
        /// Creates a new event
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static TestEventOccurred NewEventFrom(string entityId, string text)
        {
            if (String.IsNullOrEmpty(entityId))
                throw new ArgumentNullException(nameof(entityId));
            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));

            var msg = new TestEventOccurred()
            {
                EntityId = entityId,
                Body = text
            };
            return msg;
        }

    }
}
