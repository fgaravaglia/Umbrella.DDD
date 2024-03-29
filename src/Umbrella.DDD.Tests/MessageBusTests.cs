﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.Tests.TestClasses;
using TestMessage = Umbrella.DDD.Tests.TestClasses.TestMessage;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] 

namespace Umbrella.DDD.Tests
{
    public class MessageBusTests
    {
        Mock<ILogger> _Logger;
        Mock<IRepository<Saga1Status>> _Repository;
        Mock<IEventPublisher> _Publisher;
        IMessageBus _Bus;
        List<Saga1Status> _Saga1StatusList;

        Mock<IRepository<Saga1Status>> MockStatusRepo()
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
            return repo;
        }

        [SetUp]
        public void Setup()
        {
            this._Logger = new Mock<ILogger>();
            this._Publisher = new Mock<IEventPublisher>();
            this._Publisher.Setup(x => x.PublishMessage(It.IsAny<IMessage>())).Returns(Guid.NewGuid().ToString());
            this._Saga1StatusList = new List<Saga1Status>();
            this._Repository = MockStatusRepo();
        }

        #region Tests on constructor
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

        #endregion

        [Test]
        public void Publish_ReturnsNotNullMessageId()
        {
            //********* GIVEN
            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(x => this._Logger.Object);
            services.AddTransient<IMessageHandler<TestMessage>, TestMessageHandler>();
            services.AddTransient<IMessageHandler<TestMessage>, NewTestMessageHandler>();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(this._Logger.Object, this._Publisher.Object, provider);

            //********* WHEN
            string msgId = this._Bus.PublishMessage(new TestMessage("SSSSS"));

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            Assert.Pass();
        }

        [Test]
        public void Publish_ReturnsNotNullMessageId_EvenThereAreNoHandlers()
        {
            //********* GIVEN
            var services = new ServiceCollection();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(this._Logger.Object, this._Publisher.Object, provider);

            //********* WHEN
            string msgId = this._Bus.PublishMessage(new TestMessage("SSSSS"));

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            Assert.Pass();
        }

        [Test]
        [Description(@"This test has been added to  replicate a bug context (#3).
        A message from generic list of events does not resolve succesfully the handler")]
        public void Publish_FromListOfMessages_InvokesExpectedHandler()
        {
            //********* GIVEN
            var services = new ServiceCollection();
            var handler = new Mock<IMessageHandler<Umbrella.DDD.Tests.TestClasses.TestMessage>>();
            handler.Setup(x => x.CanHandleThisMessage(It.IsAny<IMessage>())).Returns(true);
            services.AddSingleton<ILogger>(x => this._Logger.Object);
            services.AddScoped<IMessageHandler>(x => handler.Object);
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(this._Logger.Object, this._Publisher.Object, provider);
            var messages = new List<IMessage>(){new TestMessage("SSSSS") };

            //********* WHEN
            string msgId = this._Bus.PublishMessage(messages[0]);

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            handler.Verify(x => x.TryHandleMessage(It.IsAny<IMessage>()), Times.Once);
            Assert.Pass();
        }

        [Test]
        [Description(@"This test has been added to  replicate a bug context (#3).
        A message from generic list of events does not resolve succesfully the handler")]
        public void Publish_FromListOfMessages_InvokesExpectedHandler_And_SkipsSaga()
        {
            //********* GIVEN
            var services = new ServiceCollection();
            var handler = new Mock<IMessageHandler<Umbrella.DDD.Tests.TestClasses.TestMessage>>();
            handler.Setup(x => x.CanHandleThisMessage(It.IsAny<IMessage>())).Returns(true);
            services.AddSingleton<ILogger>(x => this._Logger.Object);
            services.AddScoped<IMessageHandler>(x => handler.Object);
            services.AddSingleton<IRepository<Saga1Status>>(this._Repository.Object);
            services.AddTransient<ISaga, Saga1>();
            services.AddTransient<ISaga, Saga2>();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(this._Logger.Object, this._Publisher.Object, provider);
            var messages = new List<IMessage>() { new TestMessage("SSSSS") };

            //********* WHEN
            string msgId = this._Bus.PublishMessage(messages[0]);

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            handler.Verify(x => x.TryHandleMessage(It.IsAny<IMessage>()), Times.Once);
            Assert.Pass();
        }

        [Test]
        public void Publish_FromListOfMessages_WithHandlersDisabled_DoesNotInvokesAnyHandler()
        {
            //********* GIVEN
            var services = new ServiceCollection();
            var handler = new Mock<IMessageHandler<Umbrella.DDD.Tests.TestClasses.TestMessage>>();
            handler.Setup(x => x.CanHandleThisMessage(It.IsAny<IMessage>())).Returns(true);
            services.AddSingleton<ILogger>(x => this._Logger.Object);
            services.AddScoped<IMessageHandler<TestMessage>>(x => handler.Object);
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(this._Logger.Object, this._Publisher.Object, provider, enableInMemoryEventHandlers: false);
            var messages = new List<IMessage>() { new TestMessage("SSSSS") };

            //********* WHEN
            string msgId = this._Bus.PublishMessage(messages[0]);

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            handler.Verify(x => x.TryHandleMessage(It.IsAny<IMessage>()), Times.Never);
            Assert.Pass();
        }

        #region Tests on Saga Management

        [Test]
        public void Publish_Runs_The_Saga_And_StatusIsPersisted()
        {
            //********* GIVEN
            var services = new ServiceCollection();
            services.AddSingleton<IRepository<Saga1Status>>(MockStatusRepo().Object);
            services.AddTransient<ISaga, Saga1>();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(this._Logger.Object, this._Publisher.Object, provider);
            Assert.That(this._Saga1StatusList.Count, Is.EqualTo(0), "Precondition: SAGA not started (status null) failed");

            //********* WHEN
            string msgId = this._Bus.PublishMessage(new TestMessage("SSSSS"));

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
            var services = new ServiceCollection();
            services.AddSingleton<IRepository<Saga1Status>>(this._Repository.Object);
            services.AddTransient<ISaga, Saga1>();
            services.AddTransient<ISaga, Saga2>();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(this._Logger.Object, this._Publisher.Object, provider);
            Assert.That(this._Saga1StatusList.Count, Is.EqualTo(0), "Precondition: SAGA not started (status null) failed");

            //********* WHEN
            string msgId = this._Bus.PublishMessage(new TestMessage("SSSSS"));

            //********* WHEN
            Assert.False(String.IsNullOrEmpty(msgId));
            Assert.That(this._Saga1StatusList.Count, Is.EqualTo(2));
            Assert.Pass();
        }

        [Test]
        public void PublishingAllMessages_SagaCanBeCompleted()
        {
            //********* GIVEN
            var services = new ServiceCollection();
            // registering saga that is nadling the 2 types of messages below
            services.AddSingleton<IRepository<Saga1Status>>(this._Repository.Object);
            services.AddSingleton<ISaga, Saga3>();
            IServiceProvider provider = services.BuildServiceProvider();
            this._Bus = new MessageBus(this._Logger.Object, this._Publisher.Object, provider);
            Assert.That(this._Saga1StatusList.Count, Is.EqualTo(0), "Precondition: SAGA not started (status null) failed");

            //********* WHEN
            this._Bus.PublishMessage(new TestMessage("SSSSS"));
            this._Bus.PublishMessage(new TestMessage2("END"));

            //********* WHEN
            Assert.That(this._Saga1StatusList.Count, Is.EqualTo(1));
            Assert.That(this._Saga1StatusList[0].Message, Is.EqualTo("SSSSS"));
            Assert.That(this._Saga1StatusList[0].SagaName, Is.EqualTo("Test1"));
            Assert.That(this._Saga1StatusList[0].SagaId, Is.Not.EqualTo(Guid.Empty.ToString()));
            Assert.False(this._Saga1StatusList[0].IsRunning);
            Assert.True(this._Saga1StatusList[0].IsCompleted);
            Assert.Pass();
        }
    
        #endregion
    }
}
