using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD.Api.TestScenarios
{
    public class TestEventOccurredHandler: MessageHandler<TestEventOccurred>
    {
        /// <summary>
        /// Default COnstr
        /// </summary>
        /// <param name="logger"></param>
        public TestEventOccurredHandler(ILogger logger) : base(logger)
        {
        }
        /// <summary>
        /// Checks if current handler can manage the message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool CanHandleThisMessage(IMessage message)
        {
            if (!base.CanHandleThisMessage(message))
                return false;

            return message.GetType() == typeof(TestEventOccurred);
        }
        /// <summary>
        /// handels the msg
        /// </summary>
        /// <param name="message"></param>
        public override void HandleThisMessage(TestEventOccurred message)
        {
            this._Logger.LogInformation("Handling message {message}", message);

            //this._Repo.AddEpisodeToHistoricalData(message.EntityId, message.Episode);

            this._Logger.LogInformation("Message succesfully handled");
        }
    }
}