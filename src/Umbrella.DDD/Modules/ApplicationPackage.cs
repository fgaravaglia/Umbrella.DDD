using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.DDD.Modules
{
    /// <summary>
    /// DTO to map a package (aka: assembly) taht belog to a given module
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ApplicationPackage
    {
        /// <summary>
        /// Name of Package
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// Physical name of deployable unit (aka: assembly file name)
        /// </summary>
        /// <value></value>
        public string DeployUnit { get; set; }
        /// <summary>
        /// Type of package
        /// </summary>
        /// <value></value>
        public string PackageType { get; set; }
        /// <summary>
        /// Order to apply when loading dependencies at startup
        /// </summary>
        /// <value></value>
        public int StartupOrder { get; set; }
        /// <summary>
        /// Empty Constr
        /// </summary>
        public ApplicationPackage()
        {
            this.Name = "";
            this.DeployUnit = "";
            this.PackageType = "";
            this.StartupOrder = 0;
        }
    }
}