using System;

namespace Kraken.Api.Sensor.Models
{
    /// <summary>
    /// Indicates that the model has an id and creation date.
    /// </summary>
    public interface IDatabaseModel
    {
        /// <value>The record id.</value>
        string Id { get; set; }

        /// <value>The creation date.</value>
        DateTime CreatedAt { get; set; }
    }
}