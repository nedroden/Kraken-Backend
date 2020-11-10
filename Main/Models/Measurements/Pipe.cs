using System;

namespace Kraken.Api.Main.Models.Measurements
{
    /// <summary>
    /// Represents a pipe measurement.
    /// </summary>
    public class Pipe
    {
        /// <value>The measurement id.</value>
        public string Id { get; set; }

        /// <value>The pipe id.</value>
        public string PipeId { get; set; }

        /// <value>Water flow volume, in arbitrary units.</value>
        public float WaterFlowVolume { get; set; }

        /// <value>Water quality, in arbitrary units.</value>
        public float WaterQuality { get; set; }

        /// <value>The time at which the measurement was taken. Defaults to the current time.</value>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}