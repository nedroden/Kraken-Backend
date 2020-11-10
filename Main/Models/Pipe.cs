using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kraken.Api.Main.Models
{
    /// <summary>
    /// Represents a pipe.
    /// </summary>
    public class Pipe : IDatabaseModel
    {
        /// <value>The pipe id.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <value>The pipe's title (similar to a label).</value>
        [BsonElement("title")]
        public string Title { get; set; }
    }
}