using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Umbrella.DDD.Modules
{
    /// <summary>
    /// Base implementation of a given Module
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ApplicationModule : IApplicationModule
    {
        #region Fields
        protected readonly IConfiguration _Configuration;
        #endregion

        #region Properties
        /// <summary>
        /// Name of module
        /// </summary>
        /// <value></value>
        public string ModuleName { get; private set; }
        /// <summary>
        /// Lists of packages that belong to current module
        /// </summary>
        /// <value></value>
        public IEnumerable<ApplicationPackage> Packages { get; private set; }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configuration"></param>
        protected ApplicationModule(string name, IConfiguration configuration)
        {
            if (String.IsNullOrEmpty(name))
                throw new NotImplementedException(nameof(name));
            this.ModuleName = name;
            this._Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.Packages = new List<ApplicationPackage>();
        }
        /// <summary>
        /// Sets the proper container with all required implementations
        /// </summary>
        /// <param name="services"></param>
        public void AddServices(IServiceCollection services)
        {
            AddModuleServices(services);
        }

        #region Protected Methods

        protected abstract void AddModuleServices(IServiceCollection services);

        #endregion
    }
}