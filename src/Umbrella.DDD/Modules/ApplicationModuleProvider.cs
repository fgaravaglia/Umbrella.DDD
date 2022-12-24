using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Umbrella.DDD.Modules
{
    /// <summary>
    /// BAse class for providing modules
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ApplicationModuleProvider : IApplicationModuleProvider
    {
        protected readonly IConfiguration _Config;
        protected readonly string _Environment;

        public ApplicationModuleProvider(IConfiguration config, string environmentName)
        {
            this._Config = config ?? throw new ArgumentNullException(nameof(config));
            this._Environment = environmentName;
        }
        /// <summary>
        /// Gets the modules
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<IApplicationModule> GetModules();
    }
}