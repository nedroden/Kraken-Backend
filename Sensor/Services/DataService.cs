using Cassandra.Mapping;
using Cassandra;
using System.Collections.Generic;
using System.Linq;
using System;

using Kraken.Api.Sensor.Models;
using Kraken.Api.Sensor.Util;

namespace Kraken.Api.Sensor.Services
{
    /// <summary>
    /// Provides an interface for data services.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Represents a method that fetches a single record from the database.
        /// </summary>
        /// <param name="id">The record id.</param>
        /// <typeparam name="T">The model.</typeparam>
        /// <returns>A single record if found, or null.</returns>
        T GetSingle<T>(string id) where T : IDatabaseModel;

        /// <summary>
        /// Represents a method that returns all records for a certain model.
        /// </summary>
        /// <typeparam name="T">The model.</typeparam>
        /// <returns>A list of all records for the specified model.</returns>
        IEnumerable<T> GetAll<T>() where T : IDatabaseModel;

        /// <summary>
        /// Represents a method that returns the latest record for each group (e.g. a group of
        /// measurements for a certain house).
        /// </summary>
        /// <param name="groupBy">Determines how to group the records.</param>
        /// <typeparam name="T">The model.</typeparam>
        /// <typeparam name="R">The result type.</typeparam>
        /// <returns>The latest record for each group.</returns>
        IEnumerable<T> GetLatest<T, R>(Func<T, R> groupBy) where T : IDatabaseModel;

        /// <summary>
        /// Represents a method that saves a record to the database.
        /// </summary>
        /// <param name="item">The item to save.</param>
        /// <typeparam name="T">The type of model.</typeparam>
        void Create<T>(T item) where T : IDatabaseModel;

        /// <summary>
        /// Represents a method that updates a record.
        /// </summary>
        /// <param name="updatedItem">The updated item.</param>
        /// <typeparam name="T">The model.</typeparam>
        void Update<T>(T updatedItem) where T : IDatabaseModel;

        /// <summary>
        /// Represents a method that removes a record from the database.
        /// </summary>
        /// <param name="item">The record to remove.</param>
        /// <typeparam name="T">The model.</typeparam>
        void Remove<T>(T item) where T : IDatabaseModel;

        /// <summary>
        /// Represents a method that checks whether or not a certain record exist.
        /// </summary>
        /// <param name="id">The supposed id of the record.</param>
        /// <typeparam name="T">The model.</typeparam>
        /// <returns>Whether or not the record exists.</returns>
        bool Exists<T>(string id) where T : IDatabaseModel;
    }

    /// <summary>
    /// The data service.
    /// </summary>
    public class DataService : IDataService
    {
        private ICluster _cluster;
        private ISession _session;
        private IMapper _mapper;

        private IConnectionInfo _connectionInfo;
        private ApplicationService _applicationService;

        /// <summary>
        /// Creates a new data service.
        /// </summary>
        /// <param name="connectionInfo">Database connection settings.</param>
        /// <param name="applicationService">The application service.</param>
        public DataService(IConnectionInfo connectionInfo, ApplicationService applicationService)
        {
            _connectionInfo = connectionInfo;
            _applicationService = applicationService;

            Reconnect();
        }

        /// <summary>
        /// Makes 10 attempts to (re) connect to the database.
        /// </summary>
        private void Reconnect()
        {
            try
            {
                var retryableTask = new RetryableTask<NoHostAvailableException>(() => Connect(_connectionInfo), 10);
                retryableTask.Execute("Cassandra server unavailable");
            }
            catch (Exception exception)
            {
                if (exception is NoHostAvailableException)
                {
                    _applicationService.Crash("Unable to connect to the Cassandra server.");
                }
            }
        }

        /// <summary>
        /// Makes a single attempt to connect to the database.
        /// </summary>
        /// <param name="connectionInfo">Database connection settings.</param>
        /// <seealso cref="Reconnect" />
        private void Connect(IConnectionInfo connectionInfo)
        {
            _cluster = Cluster.Builder().AddContactPoint(connectionInfo.HostName).Build();
            _session = _cluster.Connect(connectionInfo.KeyspaceName);
            _mapper = new Mapper(_session);
        }

        /// <summary>
        /// Disconnects from the database. Upon failure, errors are silently ignored, since we can't do
        /// much about them anyway.
        /// </summary>
        private void Disconnect()
        {
            try
            {
                if (_session != null && !_session.IsDisposed)
                {
                    _session.Dispose();
                }

                _cluster.Dispose();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Wraps around an action and reconnects to the database upon connection failure.
        /// </summary>
        /// <param name="action">The action to wrap around.</param>
        private void ReconnectOnFailure(Action action)
        {
            try
            {
                action();
            }
            catch (NoHostAvailableException)
            {
                Disconnect();
                Reconnect();

                action();
            }
        }

        /// <summary>
        /// Wraps around an action and reconnects to the database upon connection failure.
        /// </summary>
        /// <param name="action">The action to wrap around.</param>
        /// <typeparam name="T">The return value of the action.</typeparam>
        /// <returns>The result of the action.</returns>
        private T ReconnectOnFailure<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (NoHostAvailableException)
            {
                Disconnect();
                Reconnect();

                return action();
            }
        }

        /// <summary>
        /// Returns a single record from the database.
        /// </summary>
        /// <param name="id">The record id.</param>
        /// <typeparam name="T">The model.</typeparam>
        /// <returns>The record if it was found, otherwise null.</returns>
        public T GetSingle<T>(string id) where T : IDatabaseModel
        {
            return ReconnectOnFailure<T>(() => _mapper.SingleOrDefault<T>("WHERE id = ?", id));
        }

        /// <summary>
        /// Returns all records from the database.
        /// </summary>
        /// <typeparam name="T">The model.</typeparam>
        /// <returns>A list of all records.</returns>
        public IEnumerable<T> GetAll<T>() where T : IDatabaseModel
        {
            return ReconnectOnFailure<IEnumerable<T>>(() => _mapper.Fetch<T>());
        }

        /// <summary>
        /// Returns the latest measurement per group (e.g. houses or pipes).
        /// </summary>
        /// <param name="groupBy">Function that determines how to group the data, e.g. by house id.</param>
        /// <typeparam name="T">The model</typeparam>
        /// <typeparam name="R">The return value of the group function.</typeparam>
        /// <returns>The latest record for each group.</returns>
        public IEnumerable<T> GetLatest<T, R>(Func<T, R> groupBy) where T : IDatabaseModel
        {
            return ReconnectOnFailure<IEnumerable<T>>(() => _mapper
                  .Fetch<T>()
                  .GroupBy(groupBy)
                  .Select(group => group.OrderByDescending(record => record.CreatedAt).First()));
        }

        /// <summary>
        /// Saves a record to the database.
        /// </summary>
        /// <param name="item">The item that should be saved.</param>
        /// <typeparam name="T">The model.</typeparam>
        public void Create<T>(T item) where T : IDatabaseModel
        {
            item.Id = Guid.NewGuid().ToString();

            ReconnectOnFailure(() => _mapper.Insert(item));
        }

        /// <summary>
        /// Updates a record.
        /// </summary>
        /// <param name="updatedItem">The updated record.</param>
        /// <typeparam name="T">The model.</typeparam>
        public void Update<T>(T updatedItem) where T : IDatabaseModel
        {
            ReconnectOnFailure(() => _mapper.Update(updatedItem));
        }

        /// <summary>
        /// Removes a record from the database.
        /// </summary>
        /// <param name="item">The record to remove.</param>
        /// <typeparam name="T">The model.</typeparam>
        public void Remove<T>(T item) where T : IDatabaseModel
        {
            ReconnectOnFailure(() => _mapper.Delete(item));
        }

        /// <summary>
        /// Checks whether or not a record exists with the given id.
        /// </summary>
        /// <param name="id">The supposed record id.</param>
        /// <typeparam name="T">The model.</typeparam>
        /// <returns>Whether or not the record exists.</returns>
        public bool Exists<T>(string id) where T : IDatabaseModel
        {
            return GetSingle<T>(id) != null;
        }
    }
}