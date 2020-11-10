using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kraken.Api.Main.Models
{
    /// <summary>
    /// Represents a street.
    /// </summary>
    public class Street : IDatabaseModel
    {
        /// <value>The street id.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <value>The street name.</value>
        [BsonElement("name")]
        public string Name { get; set; }

        /// <value>The area id (must point to an existing record in the database).</value>
        [BsonElement("area_id")]
        public string AreaId { get; set; }

        /// <value>The area this street belongs to (determined by AreaId).</value>
        [BsonIgnore]
        public Area Area { get; set; }
    }
}