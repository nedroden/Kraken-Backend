using System;
using Cassandra.Mapping.Attributes;

namespace Kraken.Api.Sensor.Models
{
    /// <summary>
    /// Represents a house measurement.
    /// </summary>
    [Table("house_measurements")]
    public class House : IDatabaseModel
    {
        /// <value>The house measurement id.</value>
        [Column("id")]
        public string Id { get; set; }

        /// <value>The house id.</value>
        [Column("house_id")]
        public string HouseId { get; set; }

        /// <value>Water consupmtion in arbitrary units.</value>
        [Column("consumption")]
        public float Consumption { get; set; }

        /// <value>Time at which the measurement was taken. Defaults to the current time.</value>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}