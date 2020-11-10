using Cassandra.Mapping.Attributes;
using System;

namespace Kraken.Api.Sensor.Models
{
    /// <summary>
    /// Represents a pipe measurement.
    /// </summary>
    [Table("pipe_measurements")]
    public class Pipe : IDatabaseModel
    {
        /// <value>The measurement id.</value>
        [Column("id")]
        public string Id { get; set; }

        /// <value>The pipe id.</value>
        [Column("pipe_id")]
        public string PipeId { get; set; }

        /// <value>The water flow volume, in arbitrary units.</value>
        [Column("water_flow_volume")]
        public float WaterFlowVolume { get; set; }

        /// <value>Water quality, in arbitrary units.</value>
        [Column("water_quality")]
        public float WaterQuality { get; set; }

        /// <value>Time at which the measurement was taken. Defaults to the current time.</value>
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}