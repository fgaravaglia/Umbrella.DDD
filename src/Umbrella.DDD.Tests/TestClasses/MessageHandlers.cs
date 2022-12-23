using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD.Tests.TestClasses
{
    internal class TestMessageHandler : MessageHandler<TestMessage>
    {
        public TestMessageHandler() : base() { }

        public override void HandleThisMessage(TestMessage message)
        { }

    }

    internal class NewTestMessageHandler : MessageHandler<TestMessage>
    {
        public NewTestMessageHandler() : base() { }

        public override void HandleThisMessage(TestMessage message)
        { }

    }
}
