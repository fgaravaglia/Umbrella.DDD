using System;
using Umbrella.DDD.Abstractions.LongRunningProcesses;

namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Interface to model a long-runnning-process (aka: saga)
    /// </summary>
    public interface ISaga
    {
        /// <summary>
        /// unique identifier
        /// </summary>
        string Id { get; }
        /// <summary>
        /// the saga name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// type of event that fires the saga
        /// </summary>
        /// <value></value>
        Type StarterEventType {get;}
        /// <summary>
        /// /the saga status to be persisted
        /// </summary>
        ISagaStatus? Status { get; }
        /// <summary>
        /// Restores the status from persistence
        /// </summary>
        void RefreshStatus();
        /// <summary>
        /// Checks if saga can manage this event or not
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool CanHandleThis<T>() where T : IMessage;
        /// <summary>
        /// 
        /// </summary>
        void Start<T>(T message) where T : IMessage;
        /// <summary>
        /// saves the status
        /// </summary>
        void PersistStatus();
    }
}