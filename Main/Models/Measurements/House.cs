using System;

namespace Kraken.Api.Main.Models.Measurements
{
    /// <summary>
    /// Represents a house measurement.
    /// </summary>
    public class House
    {
        /// <value>The measurement id.</value>
        public string Id { get; set; }

        /// <value>The house id.</value>
        public string HouseId { get; set; }

        /// <value>Consumption, in arbitrary units.</value>
        public float Consumption { get; set; }

        /// <value>The time at which the measurement was taken. Defaults to the current time.</value>
        public DateTime CreatedAt { get; set; }
    }
}