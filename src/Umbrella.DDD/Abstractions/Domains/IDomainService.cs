namespace Umbrella.DDD.Abstractions.Domains
{
    /// <summary>
    /// Marker Interface to identify a DOmain service
    /// </summary>
    public interface IDomainService
    {

    }
    /// <summary>
    /// Interface to model a Domain service to Query domain itself
    /// </summary>
    public interface IQueryDomainService : IDomainService
    {

    }
    /// <summary>
    /// Interface to model a Domain service to change domain status
    /// </summary>
    public interface ICommandDomainService : IDomainService
    {

    }
}
