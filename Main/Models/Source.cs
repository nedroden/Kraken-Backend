using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kraken.Api.Main.Models
{
    /// <summary>
    /// Represents a water source.
    /// </summary>
    public class Source : IDatabaseModel
    {
        /// <value>The source id.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <value>The title (similar to a label).</value>
        [BsonElement("title")]
        public string Title { get; set; }
    }
}