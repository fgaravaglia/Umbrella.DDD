namespace Umbrella.DDD.Abstractions.Domains
{

    /// <summary>
    /// Abstranction for repository of Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityRepository<T> : IRepository<T>
        where T : IEntity
    {
    }
}
