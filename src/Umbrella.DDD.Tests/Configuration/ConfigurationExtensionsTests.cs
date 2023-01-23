using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Umbrella.DDD.Configuration;

namespace Umbrella.DDD.Tests.Configuration
{
    public class ConfigurationExtensionsTests
    {
        [Test]
        public void GetMessageBusSettings_ThrowsEx_IfServicesIsNull()
        {
            //********* GIVEN
            IConfiguration config = null;

            //********* WHEN
            TestDelegate testCode = () => config.GetMessageBusSettings();

            //********* WHEN
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(testCode);
            Assert.That(ex.ParamName, Is.EqualTo("config"));
            Assert.Pass();
        }

        [Test]
        public void GetMessageBusSettings_Returns_AppSettingsSection()
        {
            //********* GIVEN
            IConfiguration config = new ConfigurationManager().InitConfigurationFromFile();

            //********* WHEN
            var settings = config.GetMessageBusSettings();

            //********* WHEN
            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.Publisher, Is.EqualTo("InMemory"));
            Assert.True(settings.IsInMemory);
            Assert.That(settings.Queues.Count,  Is.EqualTo(1));
            Assert.That(settings.Queues[0].EventType, Is.EqualTo("myEventType"));
            Assert.That(settings.Queues[0].QueueName, Is.EqualTo("myQueue"));
            Assert.Pass();
        }
    }
}