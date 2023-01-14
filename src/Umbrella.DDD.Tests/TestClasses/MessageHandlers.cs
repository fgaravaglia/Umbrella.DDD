using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD.Tests.TestClasses
{
    internal class TestMessageHandler : MessageHandler<TestMessage>
    {
        public TestMessageHandler(ILogger logger) : base(logger) { }

        public override bool CanHandleThisMessage(IMessage message)
        {
            if (message == null)
                return false;
            return message.GetType() == typeof(TestMessage);
        }

        public override void HandleThisMessage(TestMessage message)
        { }

    }

    internal class NewTestMessageHandler : MessageHandler<TestMessage>
    {
        public NewTestMessageHandler(ILogger logger) : base(logger) { }

        public override void HandleThisMessage(TestMessage message)
        { }

    }
}
