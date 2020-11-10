using System;

namespace Kraken.Api.Main.Models.Measurements
{
    /// <summary>
    /// Represents a source measurement.
    /// </summary>
    public class Source
    {
        /// <value>The measurement id.</value>
        public string Id { get; set; }

        /// <value>The source id.</value>
        public string SourceId { get; set; }

        /// <value>Production, in arbitrary units.</value>
        public float Production { get; set; }

        /// <value>Water quality, in arbitrary units.</value>
        public float WaterQuality { get; set; }

        /// <value>The time the measurement was taken at. Defaults to the current time.</value>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}