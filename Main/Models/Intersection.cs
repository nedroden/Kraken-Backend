using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Kraken.Api.Main.Models
{
    /// <summary>
    /// Represents an intersection.
    /// </summary>
    public class Intersection : IDatabaseModel
    {
        /// <value>The intersection id.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <value>A list of streets.</value>
        [BsonElement("streets")]
        public List<string> Streets { get; set; }
    }
}