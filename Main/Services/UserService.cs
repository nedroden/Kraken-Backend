using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

using Kraken.Api.Main.Extensions;
using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services
{
    /// <summary>
    /// Provides CRUD operations for users.
    /// </summary>
    public class UserService : Service<User>
    {
        /// <summary>
        /// Creates a new user service.
        /// </summary>
        /// <param name="databaseEnvironment">
        /// An object containing the necessary settings to establish a database connection.
        /// </param>
        public UserService(IDatabaseEnvironment databaseEnvironment) : base(databaseEnvironment, "users") { }

        /// <summary>
        /// Fetches all users from the database.
        /// </summary>
        /// <returns>A list of users.</returns>
        public override List<User> GetAll()
        {
            return this.ReconnectOnFailure<User, List<User>>(() => items.Find(user => true).ToList());
        }

        /// <summary>
        /// Fetches a single user from the database, given a certain id.
        /// </summary>
        /// <param name="id">The id of the user that should be fetched from the database.</param>
        /// <returns>The user if found, otherwise null.</returns>
        public override User GetSingle(string id)
        {
            return this.ReconnectOnFailure<User, User>(() => items.Find<User>(user => user.Id == id).FirstOrDefault());
        }

        /// <summary>
        /// Fetches a single user from the database, given a certain email. Useful for checking if a user exists
        /// when used in combination with GetByUsername().
        /// </summary>
        /// <param name="email">The email address of the user that should be fetched from the database.</param>
        /// <returns>The user if found, otherwise null.</returns>
        public User GetByEmail(string email)
        {
            return this.ReconnectOnFailure<User, User>(() => items.Find<User>(user => user.Email == email).FirstOrDefault());
        }

        /// <summary>
        /// Fetches a single user from the database, given a certain username. Useful for checking if a user exists
        /// when used in combination with GetByEmail().
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public User GetByUsername(string username)
        {
            return this.ReconnectOnFailure<User, User>(() => items.Find<User>(user => user.Username == username).FirstOrDefault());
        }

        /// <summary>
        /// Stores a user in the database.
        /// </summary>
        /// <param name="user">Information about the user.</param>
        /// <returns>The newly created user.</returns>
        public override User Create(User user)
        {
            this.ReconnectOnFailure<User>(() => items.InsertOne(user));

            return user;
        }

        /// <summary>
        /// Updates the user with the specified id.
        /// </summary>
        /// <param name="id">The id of the user that should be updated.</param>
        /// <param name="user">(Updated) user information.</param>
        public override void Update(string id, User user)
        {
            this.ReconnectOnFailure<User>(() => items.ReplaceOne(user => user.Id == id, user));
        }

        /// <summary>
        /// Removes a user from the database.
        /// </summary>
        /// <param name="id">The id of the user that should be removed.</param>
        /// <exception cref="Exception">Thrown when attempting to remove the last user.</exception>
        public override void Remove(string id)
        {
            if (items.EstimatedDocumentCount() == 1)
            {
                throw new Exception("Cannot remove last user.");
            }

            this.ReconnectOnFailure<User>(() => items.DeleteOne(user => user.Id == id));
        }

        /// <summary>
        /// Removes a user from the database.
        /// </summary>
        /// <param name="user">The user that should be removed.</param>
        /// <seealso cref="Remove(string)" />
        public override void Remove(User user)
        {
            this.ReconnectOnFailure<User>(() => Remove(user.Id));
        }

        /// <summary>
        /// Checks whether or not a user exists by attempting to load it and checking if the result
        /// is equal to null.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>Whether or not the user exists.</returns>
        public override bool Exists(string id)
        {
            return GetSingle(id) != null;
        }
    }
}