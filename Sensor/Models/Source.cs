using System;
using Cassandra.Mapping.Attributes;

namespace Kraken.Api.Sensor.Models
{
    /// <summary>
    /// Represents a source measurement.
    /// </summary>
    [Table("source_measurements")]
    public class Source : IDatabaseModel
    {
        /// <value>The measurement id.</value>
        [Column("id")]
        public string Id { get; set; }

        /// <value>The source id.</value>
        [Column("source_id")]
        public string SourceId { get; set; }

        /// <value>Water production in arbitrary units.</value>
        [Column("production")]
        public float Production { get; set; }

        /// <value>Water quality in arbitrary units.</value>
        [Column("water_quality")]
        public float WaterQuality { get; set; }

        /// <value>Time at which the measurement was taken; defaults to the current time.</value>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}