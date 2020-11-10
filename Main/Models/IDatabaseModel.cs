using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kraken.Api.Main.Models
{
    /// <summary>
    /// Indicates that we are dealing with a model that has an id.
    /// </summary>
    public interface IDatabaseModel
    {
        /// <value>The record id.</value>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }
    }
}