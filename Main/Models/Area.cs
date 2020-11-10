using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kraken.Api.Main.Models
{
    /// <summary>
    /// Represents an area.
    /// </summary>
    public class Area : IDatabaseModel
    {
        /// <value>The area id.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <value>The ZIP code.</value>
        [BsonElement("zip_code")]
        public string ZipCode { get; set; }
    }
}