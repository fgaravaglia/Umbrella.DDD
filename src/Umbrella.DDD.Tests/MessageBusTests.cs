using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.Tests.TestClasses;
using TestMessage = Umbrella.DDD.Tests.TestClasses.TestMessage;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] 

namespace Umbrella.DDD.Tests
{
    public class MessageBusTests
    {
        IMessageBus _Bus;
        List<Saga1Status> _Saga1StatusList;

        IRepository<Saga1Status> MockStatusRepo()
        {
            var repo = new Mock<IRepository<Saga1Status>>();
            repo.Setup(x => x.Save(It.IsAny<Saga1Status>())).Callback<Saga1Status>(x =>
            {
                var existing = this._Saga1StatusList.SingleOrDefault(s => s.SagaId == x.SagaId);
                if (existing == null)
                    this._Saga1StatusList.Add(x);
                existing = this._Saga1StatusList.Single(s => s.SagaId == x.SagaId);
                existing.Message = x.Message;
                existing.IsRunning = x.IsRunning;
                existing.IsCompleted = x.IsCompleted;
            });
            repo.Setup(x => x.GetById(It.IsAny<string>())).Returns<string>(x => this._Saga1StatusList.SingleOrDefault(s => s.SagaId == x));
            return repo.Object;
        }

        [SetUp]
        public void Setup()
        {
            this._Saga1StatusList = new List<Saga1Status>();
        }

        [Test]
        public void Constructor_ThrowEx_ifLoggerIsNull()
        {
            //********* GIVEN
            ILogger logger = null;
            IEventPublisher publisher = new Mock<IEventPublisher>().Object;
            IServiceProvider provider = new ServiceCollection().BuildServiceProvider();

            //********* WHEN
            TestDelegate testCode = () => this._Bus = new MessageBus(logger, publisher, provider);

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("logger"));  
            Assert.Pass();
        }

        [Test]
        public void Constructor_ThrowEx_ifPublisherIsNull()
        {
            //********* GIVEN
            ILogger logger = new Mock<ILogger>().Object;
            IEventPublisher publisher = null;
            IServiceProvider provider = new ServiceCollection().BuildServiceProvider();

            //********* WHEN
            TestDelegate testCode = () => this._Bus = new MessageBus(logger, publisher, provider);

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("publisher"));
            Assert.Pass();
        }

        [Test]
        public void Constructor_ThrowEx_IfServiceProviderIsNull()
        {
            //********* GIVEN
            ILogger logger = new Mock<ILogger>().Object; 
            IEventPublisher publisher = new Mock<IEventPublisher>().Object;
            IServiceProvider provider = null;

            //********* WHEN
            TestDelegate testCode = () => this._Bus = new MessageBus(logger, publisher, provider);

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("serviceProvider"));
            Assert.Pass();
        }

        [Test]
        public void Publish_ReturnsNotNullMessageId()
        {
            //********* GIVEN
            ILogger logger = new Mock<ILogger>().Object;
            var publisher = new Mock<IEventPublisher>();
            publisher.Setup(x => x.PublishEvent<TestMessage>(It.IsAny<TestMessage>())).Returns(Guid.NewGuid().ToString());
            var services = new ServiceCollection();
            services.AddTransient<IMessageHandler<TestMessage>, TestMessageHandler>();
            services.AddTransient<IMessageHandler<TestMessage>, NewTestMessageHandler>();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(logger, publisher.Object, provider);

            //********* WHEN
            string msgId = this._Bus.PublishEvent(new TestMessage("SSSSS"));

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            Assert.Pass();
        }

        [Test]
        public void Publish_ReturnsNotNullMessageId_EvenThereAreNoHandlers()
        {
            //********* GIVEN
            ILogger logger = new Mock<ILogger>().Object;
            var publisher = new Mock<IEventPublisher>();
            publisher.Setup(x => x.PublishEvent<TestMessage>(It.IsAny<TestMessage>())).Returns(Guid.NewGuid().ToString());
            var services = new ServiceCollection();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(logger, publisher.Object, provider);

            //********* WHEN
            string msgId = this._Bus.PublishEvent(new TestMessage("SSSSS"));

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            Assert.Pass();
        }

        [Test]
        public void Publish_Runs_The_Saga_And_StatusIsPersisted()
        {
            //********* GIVEN
            ILogger logger = new Mock<ILogger>().Object;
            var publisher = new Mock<IEventPublisher>();
            publisher.Setup(x => x.PublishEvent<TestMessage>(It.IsAny<TestMessage>())).Returns(Guid.NewGuid().ToString());
            var services = new ServiceCollection();
            services.AddSingleton<IRepository<Saga1Status>>(MockStatusRepo());
            services.AddTransient<ISaga, Saga1>();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(logger, publisher.Object, provider);
            Assert.That(this._Saga1StatusList.Count, Is.EqualTo(0), "Precondition: SAGA not started (status null) failed");

            //********* WHEN
            string msgId = this._Bus.PublishEvent(new TestMessage("SSSSS"));

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            Assert.That(this._Saga1StatusList.Count, Is.EqualTo(1), "Saga didn't started!");
            Assert.That(this._Saga1StatusList[0].Message, Is.EqualTo("SSSSS"));
            Assert.That(this._Saga1StatusList[0].SagaName, Is.EqualTo("Test1"));
            Assert.That(this._Saga1StatusList[0].SagaId, Is.Not.EqualTo(Guid.Empty.ToString()));
            Assert.True(this._Saga1StatusList[0].IsRunning, "Sage is not running!");
            Assert.False(this._Saga1StatusList[0].IsCompleted);
            Assert.Pass();
        }

        [Test]
        public void Publish_Runs_AllExpectedSaga_WithRelatedStatus()
        {
            //********* GIVEN
            ILogger logger = new Mock<ILogger>().Object;
            var publisher = new Mock<IEventPublisher>();
            publisher.Setup(x => x.PublishEvent<TestMessage>(It.IsAny<TestMessage>())).Returns(Guid.NewGuid().ToString());
            var services = new ServiceCollection();
            services.AddSingleton<IRepository<Saga1Status>>(MockStatusRepo());
            services.AddTransient<ISaga, Saga1>();
            services.AddTransient<ISaga, Saga2>();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(logger, publisher.Object, provider);
            Assert.That(this._Saga1StatusList.Count, Is.EqualTo(0), "Precondition: SAGA not started (status null) failed");

            //********* WHEN
            string msgId = this._Bus.PublishEvent(new TestMessage("SSSSS"));

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            Assert.That(this._Saga1StatusList.Count, Is.EqualTo(2));
            Assert.Pass();
        }

        // [Test]
        // public void PublishingAllMessages_SagaCanBeCompleted()
        // {
        //     //********* GIVEN
        //     ILogger logger = new Mock<ILogger>().Object;
        //     var publisher = new Mock<IEventPublisher>();
        //     publisher.Setup(x => x.PublishEvent<TestMessage>(It.IsAny<TestMessage>())).Returns(Guid.NewGuid().ToString());
        //     var services = new ServiceCollection();
        //     services.AddSingleton<IRepository<Saga1Status>>(MockStatusRepo());
        //     services.AddSingleton<ISaga, Saga3>();
        //     IServiceProvider provider = services.BuildServiceProvider();
        //     this._Bus = new MessageBus(logger, publisher.Object, provider);
        //     Assert.That(this._Saga1StatusList.Count, Is.EqualTo(0), "Precondition: SAGA not started (status null) failed");

        //     //********* WHEN
        //     this._Bus.PublishEvent(new TestMessage("SSSSS"));
        //     this._Bus.PublishEvent(new TestMessage2("END"));

        //     //********* WHEN
        //     Assert.That(this._Saga1StatusList.Count, Is.EqualTo(1));
        //     Assert.That(this._Saga1StatusList[0].Message, Is.EqualTo("SSSSS"));
        //     Assert.That(this._Saga1StatusList[0].SagaName, Is.EqualTo("Test1"));
        //     Assert.That(this._Saga1StatusList[0].SagaId, Is.Not.EqualTo(Guid.Empty.ToString()));
        //     Assert.False(this._Saga1StatusList[0].IsRunning);
        //     Assert.True(this._Saga1StatusList[0].IsCompleted);
        //     Assert.Pass();
        // }
    }
}
