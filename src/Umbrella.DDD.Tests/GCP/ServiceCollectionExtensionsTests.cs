using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.GCP;

namespace Umbrella.DDD.Tests.GCP
{
    public class ServiceCollectionExtensionsTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void AddPubSubEventPublisher_ThrowEx_ifServiceIsNull()
        {
            //********* GIVEN
            IServiceCollection services = null;

            //********* WHEN
            TestDelegate testCode = () => services.AddPubSubEventPublisher("");

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("services"));
            Assert.Pass();
        }

        [Test]
        public void AddPubSubEventPublisher_ThrowEx_IfProjectIsNull()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            string projectID = "";

            //********* WHEN
            TestDelegate testCode = () => services.AddPubSubEventPublisher(projectID);

            //********* WHEN
            var ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("projectID"));
            Assert.Pass();
        }

        [Test]
        public void AddInMemoryEventPublisher_RegistersIEventPublisher()
        {
            //********* GIVEN
            IServiceCollection services = new ServiceCollection();
            string projectID = "umbrella-dev";
            services.AddSingleton<ILogger>(x => new Mock<ILogger>().Object);    

            //********* WHEN
            services.AddPubSubEventPublisher(projectID);

            //********* WHEN
            var provider = services.BuildServiceProvider();
            TestDelegate testCode = () => provider.GetRequiredService<IEventPublisher>();
            var ex = Assert.Throws<InvalidOperationException>(testCode);
            Assert.True(ex.Message.StartsWith("The Application Default Credentials are not available", StringComparison.InvariantCultureIgnoreCase));
            Assert.Pass();
        }
    }
}
