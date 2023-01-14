using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DDD.Abstractions.LongRunningProcesses
{
    /// <summary>
    /// Base implementation for a saga
    /// </summary>
    public abstract class Saga<Tstatus> : ISaga
        where Tstatus : ISagaStatus
    {
        #region Fields

        readonly protected IRepository<Tstatus> _Repository;

        #endregion

        #region Properties
        /// <summary>
        /// <inheritdoc cref="ISaga.Id"/>
        /// </summary>
        public string Id { get { return this.Status != null ? this.Status.SagaId : Guid.Empty.ToString(); } }
        /// <summary>
        /// <inheritdoc cref="ISaga.Name"/>
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// <inheritdoc cref="ISaga.StarterEventType"/>
        /// </summary>
        public Type StarterEventType { get; private set; }
        /// <summary>
        /// <inheritdoc cref="ISaga.Status"/>
        /// </summary>
        public ISagaStatus? Status { get; private set; }

        protected Tstatus? InternalStatus { get { return this.Status != null ? (Tstatus)this.Status : default(Tstatus); } set { this.Status = value; } }
        #endregion

        /// <summary>
        /// Default Constr
        /// </summary>
        /// <param name="name"></param>
        /// <param name="starterType"></param>
        /// <param name="repository"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected Saga(string name, Type starterType, IRepository<Tstatus> repository, Tstatus initialStatus)
        { 
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
            this.StarterEventType = starterType ?? throw new ArgumentNullException(nameof(starterType)); 
            this._Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.Status = initialStatus ?? throw new ArgumentNullException(nameof(initialStatus));
            this.Status.Initialize(Guid.NewGuid().ToString(), this.Name);
        }


        #region Public Methods
        /// <summary>
        /// <inheritdoc cref="ISaga.CanHandleThisType{T}"/>
        /// </summary>
        public abstract bool CanHandleThisType<T>() where T : IMessage;

        /// <summary>
        /// Checks if saga can manage this event or not
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract bool CanHandleThisMessage(IMessage msg);

        /// <summary>
        /// <inheritdoc cref="ISaga.PersistStatus"/>
        /// </summary>
        public void PersistStatus()
        {
            if (InternalStatus == null)
                throw new NullReferenceException($"Unexpecetd NUll status for Saga {this.Id} - {this.Name}");
            this._Repository.Save(this.InternalStatus);
        }
        /// <summary>
        /// <inheritdoc cref="ISaga.RefreshStatus"/>
        /// </summary>
        public void RefreshStatus()
        {
            var status = this._Repository.GetById(this.Id);
            if (status != null)
                this.InternalStatus = status;
        }
        /// <summary>
        /// <inheritdoc cref="ISaga.Start"/>
        /// </summary>
        public void Start<T>(T message) where T : IMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var msgType = message.GetType();
            if (StarterEventType != msgType)
                throw new ArgumentException($"Expcted starter event of Type {this.StarterEventType.Name} but found {msgType.Name} instead");

            StartSagaFromMessage(message);
        }

        #endregion

        protected abstract void StartSagaFromMessage<T>(T message) where T : IMessage;
    }
}
