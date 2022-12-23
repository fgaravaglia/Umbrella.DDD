using System;
using System.IO;
using System.Text.Json;
using Umbrella.DDD.Abstractions.Domains;

namespace Umbrella.DDD.Domain.Persistence
{
    /// <summary>
    /// Base implementation for Domain Entity repository, where persistence is on filesystem through json files
    /// </summary>
    /// <typeparam name="T">type of Business Entity</typeparam>
    /// <typeparam name="Tdto">type of DTO taht maps the entity</typeparam>
    public abstract class JsonEntityRepository<T, Tdto> : IEntityRepository<T>
        where T : IEntity
        where Tdto : class
    {
        #region Fields
        readonly string _StorageFolder;
        readonly string _FilenameBase;
        readonly object _Locker = new object();
        #endregion

        /// <summary>
        /// Default Constr
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected JsonEntityRepository(string path, string filename)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            _StorageFolder = path;
            _FilenameBase = filename;
        }

        /// <summary>
        /// Gets the entityfrom persistence
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T? Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            // set the filename
            var fullPath = Path.Combine(_StorageFolder, $"{_FilenameBase}-{id}.json");

            // remove old version
            if (!File.Exists(fullPath))
                return default(T);

            // read json
            var jsonString = File.ReadAllText(fullPath);
            //convert back
            var dto = ToDto(jsonString);
            return InstanceEntity(dto);
        }
        /// <summary>
        /// Persist the status of entity
        /// </summary>
        /// <param name="entity"></param>
        public void Save(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // convert to json
            var dto = (Tdto)entity.ToDTO();
            var jsonString = ToJson(dto);

            // set the filename
            var fullPath = Path.Combine(_StorageFolder, $"{_FilenameBase}-{entity.ID}.json");

            // remove old version
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            // persists the data
            lock (_Locker)
            {
                File.WriteAllText(fullPath, jsonString);
            }
        }

        #region Protected methods

        protected abstract T InstanceEntity(Tdto dto);

        protected virtual string ToJson(Tdto dto)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            return JsonSerializer.Serialize<Tdto>(dto, options);
        }

        protected virtual Tdto ToDto(string json)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

           var dto = JsonSerializer.Deserialize<Tdto>(json, options);
            if (dto == null)
                throw new NullReferenceException("Unexpected null DTO");
            return dto;
        }

        #endregion
    }
}
