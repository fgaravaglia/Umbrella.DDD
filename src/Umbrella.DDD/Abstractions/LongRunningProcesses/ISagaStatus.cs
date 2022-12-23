using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DDD.Abstractions.LongRunningProcesses
{
    /// <summary>
    /// Abstraction for status
    /// </summary>
    public interface ISagaStatus
    {
        /// <summary>
        /// unique identifier
        /// </summary>
        string SagaId { get; }
        /// <summary>
        /// Name of saga type
        /// </summary>
        string SagaName { get; }
        /// <summary>
        /// TRUE if saga is completed; FALSE otherwhise
        /// </summary>
        bool IsCompleted { get; }
        /// <summary>
        /// TRUE is saga is running, FALSE otherwise
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// Initializes the status
        /// </summary>
        /// <param name="sagaId"></param>
        /// <param name="sagaName"></param>
        void Initialize(string sagaId, string sagaName);
    }
}
