using MongoDB.Driver;
using System.Collections.Generic;

using Kraken.Api.Main.Extensions;
using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services.Components
{
    /// <summary>
    /// Facilitates the necessary CRUD operations on components such as houses, streets
    /// and intersections. By inheriting from this class, component services need only
    /// provide the collection name and connection info. This class takes care of the rest.
    /// </summary>
    /// <typeparam name="T">The data model.</typeparam>
    public abstract class ComponentService<T> : Service<T> where T : IDatabaseModel
    {
        /// <summary>
        /// Loads the items in the given collection.
        /// </summary>
        /// <param name="databaseEnvironment">Database connection settings.</param>
        /// <param name="collectionName">The collection name.</param>
        public ComponentService(IDatabaseEnvironment databaseEnvironment, string collectionName) : base(databaseEnvironment, collectionName) { }

        /// <summary>
        /// Fetches all records in the collection (no filter applied).
        /// </summary>
        /// <returns>All records in the collection.</returns>
        public override List<T> GetAll()
        {
            return this.ReconnectOnFailure<T, List<T>>(() => items.Find(item => true).ToList());
        }

        /// <summary>
        /// Fetches a single record.
        /// </summary>
        /// <param name="id">The record id.</param>
        /// <returns>The record if found, otherwise null.</returns>
        public override T GetSingle(string id)
        {
            return this.ReconnectOnFailure<T, T>(() => items.Find<T>(item => item.Id == id).FirstOrDefault());
        }

        /// <summary>
        /// Adds a new record.
        /// </summary>
        /// <param name="item">The record that should be added.</param>
        /// <returns>The record.</returns>
        public override T Create(T item)
        {
            this.ReconnectOnFailure<T>(() => items.InsertOne(item));

            return item;
        }

        /// <summary>
        /// Updates a record.
        /// </summary>
        /// <param name="id">The record id.</param>
        /// <param name="item">The (updated) record.</param>
        public override void Update(string id, T item)
        {
            this.ReconnectOnFailure<T>(() => items.ReplaceOne(item => item.Id == id, item));
        }

        /// <summary>
        /// Removes a record.
        /// </summary>
        /// <param name="id">The record id.</param>
        /// <seealso cref="Remove(T)" />
        public override void Remove(string id)
        {
            this.ReconnectOnFailure<T>(() => items.DeleteOne(item => item.Id == id));
        }

        /// <summary>
        /// Removes a record.
        /// </summary>
        /// <param name="item">The record that should be removed.</param>
        /// <seealso cref="Remove(string)" />
        public override void Remove(T item)
        {
            this.ReconnectOnFailure<T>(() => Remove(item.Id));
        }

        /// <summary>
        /// Checks whether or not a certain record exists.
        /// </summary>
        /// <param name="id">The supposed record id.</param>
        /// <returns>Whether or not the record exists.</returns>
        public override bool Exists(string id)
        {
            return GetSingle(id) != null;
        }
    }
}