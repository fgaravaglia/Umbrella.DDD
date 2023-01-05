namespace Umbrella.DDD.Abstractions
{
    /// <summary>
    /// Abstranction for repository of given class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Gets the entity from persistence
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? GetById(string id);

        /// <summary>
        /// Persist the status of entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>ID of entity</returns>
        string Save(T entity);
    }
}
