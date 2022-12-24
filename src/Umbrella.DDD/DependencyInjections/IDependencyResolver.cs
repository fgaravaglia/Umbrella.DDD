using Microsoft.Extensions.DependencyInjection;

namespace Umbrella.DDD.DependencyInjection
{
    /// <summary>
    /// Abstraction for adependency resolver
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// fill registry container
        /// </summary>
        /// <param name="services"></param>
        void AddServices(IServiceCollection services);
    
    }
}