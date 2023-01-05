using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Umbrella.DDD.Abstractions.Domains;

namespace Umbrella.DDD.Domain.Persistence
{
    /// <summary>
    /// Base implementation for Domain Entity repository, where persistence is on filesystem through json files, one per entity Type
    /// </summary>
    /// <typeparam name="T">type of Business Entity</typeparam>
    /// <typeparam name="Tdto">type of DTO taht maps the entity</typeparam>
    public abstract class JsonEntityRepository<T, Tdto> : IEntityRepository<T>
        where T : IEntity<Tdto>
        where Tdto : class, IEntityDto
    {
        #region Fields
        readonly string _StorageFolder;
        readonly string _Filename;
        readonly JsonSerializerOptions _JsonOptions;
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
            _Filename = filename;
            _JsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        /// <summary>
        /// Gets the entityfrom persistence
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T? GetById(string id)
        {
            return this.GetAll().SingleOrDefault(x => x.ID.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }
        /// <summary>
        /// Gets the entire colelctions of entities
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            var entities = new List<T>();
            // set the filename
            var fullPath = Path.Combine(_StorageFolder, _Filename);
            // remove old version
            if (!File.Exists(fullPath))
                return entities;
            // read json
            var jsonString = File.ReadAllText(fullPath);
            //convert back
            var dtos = ToDtoList(jsonString);
            return dtos.Select(x => InstanceEntity(x));
        }
        /// <summary>
        /// Persist the status of entity
        /// </summary>
        /// <param name="entity"></param>
        public string Save(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            //gets the list
            var entities = GetAll().ToList();

            // add or update
            var existing = entities.SingleOrDefault(x => x.ID.Equals(entity.ID, StringComparison.InvariantCultureIgnoreCase));
            if (existing != null)
            {
                var index = entities.IndexOf(existing);
                entities.RemoveAt(index);
            }
            entities.Add(entity);

            // convert to json
            var dtos = entities.Select(x => x.ToDTO()).ToList();
            var jsonString = JsonSerializer.Serialize<IEnumerable<Tdto>>(dtos, this._JsonOptions);

            // set the filename
            var fullPath = Path.Combine(_StorageFolder, _Filename);

            // persists the data
            lock (_Locker)
            {
                // remove old version
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                File.WriteAllText(fullPath, jsonString);
            }

            return entity.ID;
        }

        #region Protected methods

        protected abstract T InstanceEntity(Tdto dto);

        protected virtual IEnumerable<Tdto> ToDtoList(string json)
        {
            var dtos = JsonSerializer.Deserialize<IEnumerable<Tdto>>(json, this._JsonOptions);
            if (dtos == null)
                throw new NullReferenceException("Unexpected null DTO list");
            return dtos;
        }

        #endregion
    }
}
