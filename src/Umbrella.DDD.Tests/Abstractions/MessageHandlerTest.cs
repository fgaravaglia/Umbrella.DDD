using NUnit.Framework;
using System;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD.Tests.Abstractions;

public class MessageHandlerTests
{
    #region Used Types
    class TestMessage : Message
    {
        public TestMessage(string messageProperty) : base()
        { 
            this.Body = messageProperty;
        }
    }

    class TestMessage2 : Message
    {
        public TestMessage2(string messageProperty) : base()
        {
            this.Body = messageProperty;
        }
    }

    class TestMessagehandler : MessageHandler<TestMessage>
    { 
        public TestMessagehandler() : base() {  }

        public override void HandleThisMessage(TestMessage message)
        { }

    }
    #endregion

    TestMessagehandler _Handler;

    [SetUp]
    public void Setup()
    {
        this._Handler =new TestMessagehandler();
    }

    [Test]
    public void CanHandleThisMessage_ThrowsEx_IfMessageIsNull()
    {
        //********* GIVEN
        TestMessage msg = null;
        var handler = new TestMessagehandler();

        //********* WHEN
        TestDelegate testCode = () => handler.CanHandleThisMessage(msg);

        //********* WHEN
        Assert.Throws<ArgumentNullException>(testCode);
        Assert.Pass();
    }

    [Test]
    public void CanHandleThisMessage_ReturnsTrue_IfisExpectedTypeMessage()
    {
        //********* GIVEN
        TestMessage msg = new TestMessage("123");

        //********* WHEN
        var check = this._Handler.CanHandleThisMessage(msg);

        //********* WHEN
        Assert.True(check);
        Assert.Pass();
    }

    [Test]
    public void CanHandleThisMessage_ReturnsTrue_IfNotOverrided()
    {
        //********* GIVEN
        TestMessage2 msg = new TestMessage2("123");

        //********* WHEN
        var check = this._Handler.CanHandleThisMessage(msg);

        //********* WHEN
        Assert.True(check);
        Assert.Pass();
    }


    [Test]
    public void Handle_ThrowsEx_IfMessageIsNull()
    {
        //********* GIVEN
        TestMessage msg = null;

        //********* WHEN
        TestDelegate testCode = () => this._Handler.Handle(msg);

        //********* WHEN
        Assert.Throws<ArgumentNullException>(testCode);
        Assert.Pass();
    }

    [Test]
    public void Handle_ThrowsEx_IfMessageTypeIsNotmanaged()
    {
        //********* GIVEN
        var msg = new TestMessage2("123") ;

        //********* WHEN
        TestDelegate testCode = () => this._Handler.Handle(msg);

        //********* WHEN
        Assert.Throws<ArgumentException>(testCode);
        Assert.Pass();
    }

    [Test]
    public void TryHandleThisMessage_ReturnsNull_IfTHereIsNoError()
    {
        //********* GIVEN
        TestMessage msg = new TestMessage("123");

        //********* WHEN
        var error = this._Handler.TryHandleThisMessage(msg);

        //********* WHEN
        Assert.IsNull(error);
        Assert.Pass();
    }

}