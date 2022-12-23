using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DDD.Abstractions;

namespace Umbrella.DDD.Tests.TestClasses
{
    internal class TestMessage : Message
    {
        public TestMessage(string messageProperty) : base()
        {
            this.Body = messageProperty;
        }
    }

    internal class TestMessage2 : Message
    {
        public TestMessage2(string messageProperty) : base()
        {
            this.Body = messageProperty;
        }
    }

    
}
