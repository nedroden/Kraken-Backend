using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kraken.Api.Main.Models
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User : IDatabaseModel
    {
        /// <value>The user's id.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <value>The username.</value>
        [BsonElement("username")]
        public string Username { get; set; }

        /// <value>The user's email.</value>
        [BsonElement("email")]
        public string Email { get; set; }

        /// <value>The user's password.</value>
        [BsonElement("password")]
        public string Password { get; set; }
    }
}