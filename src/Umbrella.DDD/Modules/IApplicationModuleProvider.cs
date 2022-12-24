using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DDD.Modules
{
    /// <summary>
    /// Provider to get all application modules
    /// </summary>
    public interface IApplicationModuleProvider
    {
        /// <summary>
        /// Gets the modules
        /// </summary>
        /// <returns></returns>
        IEnumerable<IApplicationModule> GetModules();
    }
}