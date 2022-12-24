using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DDD.DependencyInjection;

namespace Umbrella.DDD.Modules
{
    /// <summary>
    /// Abstraction of an Application module
    /// </summary>
    public interface IApplicationModule : IDependencyResolver
    {
        /// <summary>
        /// NAme of the module
        /// </summary>
        /// <value></value>
        string ModuleName { get; }
        /// <summary>
        /// Lists of packages that belong to current module
        /// </summary>
        /// <value></value>
        IEnumerable<ApplicationPackage> Packages { get; }
    }
}