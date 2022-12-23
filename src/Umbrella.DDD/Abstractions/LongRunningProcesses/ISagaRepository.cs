using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DDD.Abstractions.LongRunningProcesses
{
    /// <summary>
    /// Abstraction of persistence
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISagaRepository<T> : IRepository<T> where T : class, ISagaStatus
    {
    }
}
