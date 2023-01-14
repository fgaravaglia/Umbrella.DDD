using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DDD.Abstractions;
using Umbrella.DDD.Abstractions.LongRunningProcesses;

namespace Umbrella.DDD.Tests.TestClasses
{
    internal class Saga1Status : ISagaStatus
    {
        public string SagaId { get; private set; }

        public string SagaName { get; private set; }

        public bool IsCompleted { get; internal set; }

        public bool IsRunning { get; internal set; }

        public string Message { get; internal set; }

        public Saga1Status()
        { 
            this.SagaId = Guid.NewGuid().ToString();
            this.SagaName = "Test1";
            this.Message = "";
        }

        public void Initialize(string sagaId, string sagaName)
        {
            this.SagaId = sagaId;
            this.SagaName = SagaName;
        }
    }

    internal class Saga1 : Saga<Saga1Status>
    {
        public Saga1(IRepository<Saga1Status> repository) : base("Test1", typeof(TestMessage), repository, new Saga1Status())
        { 
        
        }

        public override bool CanHandleThisType<T>()
        {
            return (typeof(T) == typeof(TestMessage));
        }

        public override bool CanHandleThisMessage(IMessage message)
        {
            return (message.GetType() == typeof(TestMessage));
        }

        protected override void StartSagaFromMessage<T>(T message)
        {
            this.InternalStatus.Message = message.Body.ToString();
            this.InternalStatus.IsCompleted = false;
            this.InternalStatus.IsRunning = true;
        }
    }

    internal class Saga2 : Saga<Saga1Status>
    {
        public Saga2(IRepository<Saga1Status> repository) : base("Test2", typeof(TestMessage), repository, new Saga1Status())
        {

        }

        public override bool CanHandleThisType<T>()
        {
            return (typeof(T) == typeof(TestMessage));
        }

        public override bool CanHandleThisMessage(IMessage message)
        {
            return (message.GetType() == typeof(TestMessage));
        }

        protected override void StartSagaFromMessage<T>(T message)
        {
            this.InternalStatus.Message = message.Body.ToString();
            this.InternalStatus.IsCompleted = false;
            this.InternalStatus.IsRunning = true;
        }
    }

    internal class Saga3 : Saga<Saga1Status>, IMessageHandler<TestMessage2>
    {
        public Saga3(IRepository<Saga1Status> repository) : base("Test3", typeof(TestMessage), repository, new Saga1Status())
        {

        }

        public override bool CanHandleThisType<T>()
        {
            return (typeof(T) == typeof(TestMessage) || typeof(T) == typeof(TestMessage2));
        }

        public override bool CanHandleThisMessage(IMessage message)
        {
            if(message == null)
                return false;

            return (message.GetType() == typeof(TestMessage2) || message.GetType() == typeof(TestMessage));
        }

        public Exception TryHandleThisMessage(TestMessage2 message)
        {
            return TryHandleMessage(message);
        }

        protected override void StartSagaFromMessage<T>(T message)
        {
            this.InternalStatus.Message = message.Body.ToString();
            this.InternalStatus.IsCompleted = false;
            this.InternalStatus.IsRunning = true;
        }

        public void Handle(IMessage message)
        {
            this.TryHandleThisMessage((TestMessage2)message);
        }

        public Exception TryHandleMessage(IMessage message)
        {
            this.InternalStatus.IsCompleted = true;
            this.InternalStatus.IsRunning = false;
            return null;
        }
    }
}
