using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD.Tests.Publishers
{
    public abstract class PublisherTests<T> where T : IEventPublisher
    {
        protected T _Publisher;

        public abstract void UsingThisQueueFor_ThrowsException_IfTypeIsAlreadyRegistered();

        public abstract void PublishEvent_ThrowsException_IfMessageIsNull();
    }


    public class InMemoryPublisherTests : PublisherTests<InMemoryPublisher>
    {
        #region Used Types
        class TestMessage : Message
        {
            public TestMessage(string messageProperty)
            {
                this.Body = messageProperty;
            }
        }
        #endregion

        [SetUp]
        public void Setup()
        {
            this._Publisher = new InMemoryPublisher();
        }

        [Test]
        public override void UsingThisQueueFor_ThrowsException_IfTypeIsAlreadyRegistered()
        {
            //********* GIVEN
            TestMessage msg = new TestMessage("test");
            this._Publisher.UsingThisQueueFor<TestMessage>("queueName");

            //********* WHEN
            TestDelegate testCode = () => this._Publisher.UsingThisQueueFor<TestMessage>("queueName2");

            //********* WHEN
            Assert.Throws<InvalidOperationException>(testCode);
            Assert.Pass();
        }

        [Test]
        public override void PublishEvent_ThrowsException_IfMessageIsNull()
        {
            //********* GIVEN
            TestMessage msg = null;
            this._Publisher.UsingThisQueueFor<TestMessage>("queueName");

            //********* WHEN
            TestDelegate testCode = () => this._Publisher.PublishEvent<TestMessage>(msg);

            //********* WHEN
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("msg"));
            Assert.Pass();
        }

    }
}
