using MongoDB.Driver;
using System.Collections.Generic;
using System;

using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services
{
    /// <summary>
    /// Represents a generic service and enforces the implementation of basic CRUD operations such
    /// as creation and deletion. Also contains certain methods to reduce the amount of boilerplate
    /// code.
    /// </summary>
    /// <typeparam name="T">The model the service serves, e.g. 'User' for the user service.</typeparam>
    public abstract class Service<T> where T : IDatabaseModel
    {
        private IDatabaseEnvironment _databaseEnvironment;

        private MongoClientBase _client;
        private IMongoDatabase _database;

        private string _collectionName;

        /// <value>Contains all records in a given collection.</value>
        protected IMongoCollection<T> items;

        /// <summary>
        /// Handles the database connection part.
        /// </summary>
        /// <param name="databaseEnvironment">Database connection settings.</param>
        /// <param name="collectionName">The name of the collection</param>
        public Service(IDatabaseEnvironment databaseEnvironment, string collectionName)
        {
            _databaseEnvironment = databaseEnvironment;
            _collectionName = collectionName;

            Connect();
            LoadCollection();
        }

        /// <summary>
        /// Loads the collection.
        /// </summary>
        public void LoadCollection()
        {
            items = _database.GetCollection<T>(_collectionName);
        }

        /// <summary>
        /// Attempts to dispose of the current cluster object and reconnect afterwards. Exceptions
        /// thrown while attempting to dispose of the cluster are silenced.
        /// </summary>
        public void Reconnect()
        {
            Console.WriteLine("Lost connection to MongoDB server. Reconnecting...");

            Connect();
            LoadCollection();
        }

        /// <summary>
        /// Establishes a connection with the MongoDB server and selects the database.
        /// </summary>
        public void Connect()
        {
            _client = new MongoClient(_databaseEnvironment.ConnectionString);
            _database = _client.GetDatabase(_databaseEnvironment.DatabaseName)
                .WithReadPreference(ReadPreference.PrimaryPreferred);
        }

        /// <summary>
        /// Represents a method that fetches all records without filters.
        /// </summary>
        /// <returns>A list of items.</returns>
        public abstract List<T> GetAll();

        /// <summary>
        /// Represents a method that fetches a single record, determined by the given id.
        /// </summary>
        /// <param name="id">The id of the record.</param>
        /// <returns>The record if it was found, otherwise null.</returns>
        public abstract T GetSingle(string id);

        /// <summary>
        /// Represents a method that saves a record to the database.
        /// </summary>
        /// <param name="item">The record that should be saved.</param>
        /// <returns>The record.</returns>
        public abstract T Create(T item);

        /// <summary>
        /// Represents a method that updates a record.
        /// </summary>
        /// <param name="id">The id of the record.</param>
        /// <param name="item">The (updated) record.</param>
        public abstract void Update(string id, T item);

        /// <summary>
        /// Represents a method that removes a record from the database.
        /// </summary>
        /// <param name="id">The id of the record that should be removed.</param>
        /// <seealso cref="Remove(T)" />
        public abstract void Remove(string id);

        /// <summary>
        /// Represents a method that removes a record from the database.
        /// </summary>
        /// <param name="item">The record that should be removed.</param>
        /// <seealso cref="Remove(string)" />
        public abstract void Remove(T item);

        /// <summary>
        /// Represents a method that checks if a certain record exists.
        /// </summary>
        /// <param name="id">The supposed id of the record.</param>
        /// <returns>Whether or not the record exists in the database.</returns>
        public abstract bool Exists(string id);
    }
}