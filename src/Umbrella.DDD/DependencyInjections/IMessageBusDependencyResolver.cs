using Microsoft.Extensions.DependencyInjection;

namespace Umbrella.DDD.DependencyInjection
{
    /// <summary>
    /// Abstraction for behavior to Configure services collection with dependencies
    /// </summary>
    public interface IMessageBusDependencyResolver
    {
        /// <summary>
        /// Sets the services provided by the cussrent assembl7 / module / component
        /// </summary>
        /// <param name="services"></param>
        IServiceCollection AddEventHandlers(IServiceCollection services);

        /// <summary>
        /// Sets the services provided by the sagas stored in the current assembly
        /// </summary>
        /// <param name="services"></param>
        IServiceCollection AddSagas(IServiceCollection services);
    }
}