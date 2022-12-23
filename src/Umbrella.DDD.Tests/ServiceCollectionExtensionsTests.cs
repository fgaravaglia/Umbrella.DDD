using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.Tests.TestClasses;
using TestMessage = Umbrella.DDD.Tests.TestClasses.TestMessage;

namespace Umbrella.DDD.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [SetUp]
        public void Setup()
        {

        }

        #region Tests on AddInmemoryEventPublisher
        [Test]
        public void AddInMemoryEventPublisher_ThrowEx_ifServiceIsNull()
        {
            //********* GIVEN
            IServiceCollection services = null;

            //********* WHEN
            TestDelegate testCode = () => services.AddInMemoryEventPublisher();

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("services"));
            Assert.Pass();
        }

        [Test]
        public void AddInMemoryEventPublisher_RegistersIEventPublisher()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection(); ;

            //********* WHEN
            services.AddInMemoryEventPublisher();

            //********* WHEN
            var provider = services.BuildServiceProvider();
            var publisher = provider.GetRequiredService<IEventPublisher>();
            Assert.IsNotNull(publisher);
            Assert.Pass();
        }
        #endregion

        #region Tests on AddMessageBus

        [Test]
        public void AddMessageBus_ThrowEx_ifServiceIsNull()
        {
            //********* GIVEN
            IServiceCollection services = null;

            //********* WHEN
            TestDelegate testCode = () => services.AddAddMessageBus();

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("services"));
            Assert.Pass();
        }

        [Test]
        public void AddMessageBus_ThrowEx_IFLoggerIsNotRegistered()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            services.AddAddMessageBus();
            var provider = services.BuildServiceProvider();

            //********* WHEN
            TestDelegate testCode = () => provider.GetService<IMessageBus>();

            //********* WHEN
            var ex = Assert.Throws<InvalidOperationException>(testCode);
            Assert.That(ex.Message, Is.EqualTo("No service for type 'Microsoft.Extensions.Logging.ILogger' has been registered."));
            Assert.Pass();
        }

        [Test]
        public void AddMessageBus_ThrowEx_IfEventPublisherIsNotRegistered()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ILogger>(x => new Mock<ILogger>().Object);
            services.AddAddMessageBus();
            var provider = services.BuildServiceProvider();

            //********* WHEN
            TestDelegate testCode = () => provider.GetService<IMessageBus>();

            //********* WHEN
            var ex = Assert.Throws<InvalidOperationException>(testCode);
            Assert.That(ex.Message, Is.EqualTo("No service for type 'Umbrella.DDD.Abstractions.IEventPublisher' has been registered."));
            Assert.Pass();
        }

        [Test]
        public void AddMessageBus()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ILogger>(x => new Mock<ILogger>().Object);
            services.AddInMemoryEventPublisher();
            services.AddAddMessageBus();
            var provider = services.BuildServiceProvider();

            //********* WHEN
            var bus = provider.GetService<IMessageBus>();

            //********* WHEN
            Assert.IsNotNull(bus);
            Assert.Pass();
        }

        #endregion

        #region Tests on AddEventHandlers

        [Test]
        public void AddEventHandlers_ThrowsErrorIfServicesIsNull()
        {
            //********* GIVEN
            IServiceCollection services = null;
            string folder = Environment.CurrentDirectory;

            //********* WHEN
            TestDelegate testCode = () => services.AddEventHandlers(folder, "Umbrella.DDD.Tests.dll");

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("services"));
            Assert.Pass();
        }

        [Test]
        public void AddEventHandlers_ThrowsErrorIfFolderIsNull()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            string folder = "";

            //********* WHEN
            TestDelegate testCode = () => services.AddEventHandlers(folder, "Umbrella.DDD.Tests.dll");

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("assemblyFolder"));
            Assert.Pass();
        }

        [Test]
        public void AddEventHandlers_ThrowsErrorIfAssemblynameIsNull()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            string folder = Environment.CurrentDirectory;

            //********* WHEN
            TestDelegate testCode = () => services.AddEventHandlers(folder, null);

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("assemblyName"));
            Assert.Pass();
        }


        [Test]
        public void AddEventHandlers_ThrowsErrorIfAssemblyDoesNotExist()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            string folder = Environment.CurrentDirectory;

            //********* WHEN
            TestDelegate testCode = () => services.AddEventHandlers(folder, "Not.existing.Assembly.dll");

            //********* WHEN
            var ex = Assert.Throws<FileNotFoundException>(testCode);
            Assert.Pass();
        }

        [Test]
        public void AddEventHandlers_InjectsAllHandlers()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ILogger>(x => new Mock<ILogger>().Object);
            string folder = Environment.CurrentDirectory;

            //********* WHEN
            services.AddEventHandlers(new DependencyResolver());

            //********* WHEN
            var provider = services.BuildServiceProvider();
            var handlers = provider.GetServices(typeof(IMessageHandler<TestMessage>));
            Assert.That(handlers.Count(), Is.EqualTo(2));
            Assert.Pass();
        }

        [Test]
        public void AddEventHandlers_InjectsAllSaga()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<ILogger>(x => new Mock<ILogger>().Object);
            services.AddSingleton<IRepository<Saga1Status>>(x => new Mock<IRepository<Saga1Status>>().Object);
            string folder = Environment.CurrentDirectory;

            //********* WHEN
            services.AddEventHandlers(new DependencyResolver());

            //********* WHEN
            var provider = services.BuildServiceProvider();
            var handlers = provider.GetServices<ISaga>();
            Assert.That(handlers.Count(), Is.EqualTo(1));
            Assert.Pass();
        }

        #endregion
    }
}
