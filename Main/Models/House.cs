using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kraken.Api.Main.Models
{
    /// <summary>
    /// Represents a house.
    /// </summary>
    public class House : IDatabaseModel
    {
        /// <value>The house id.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <value>
        /// The house number. Note that this is a string since we also consider postfixes (e.g. 'a')
        /// as part of the number.
        /// </value>
        [BsonElement("number")]
        public string Number { get; set; }

        /// <value>The street id (must point to an existing database record).</value>
        public string StreetId { get; set; }

        /// <value>The street this house belongs to (determined by the value of StreetId)</value>
        [BsonIgnore]
        public Street Street { get; set; }
    }
}