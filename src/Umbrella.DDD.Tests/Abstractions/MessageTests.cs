using NUnit.Framework;
using System;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD.Tests.Abstractions;

public class MessageTests
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
    }

    [Test]
    public void Constructor_SetsPublishDate_AsNow()
    {
        //********* GIVEN

        //********* WHEN
        TestMessage msg = new TestMessage("123");

        //********* WHEN
        Assert.That(msg.PublishedOn, Is.Not.EqualTo(DateTime.MinValue));
        Assert.False(String.IsNullOrEmpty(msg.ID));

        Assert.Pass();
    }
}