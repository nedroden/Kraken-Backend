using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Kraken.Api.Sensor.Models;
using Kraken.Api.Sensor.Services;

namespace Kraken.Api.Sensor.Controllers
{
    /// <summary>
    /// The source measurement controller.
    /// </summary>
    [ApiController]
    [Route("api/measurements/[controller]")]
    public class SourceController : ControllerBase
    {
        private DataService _dataService;

        /// <summary>
        /// Creates a new source measurement controller.
        /// </summary>
        /// <param name="dataService">The data service.</param>
        public SourceController(DataService dataService) => _dataService = dataService;

        /// <summary>
        /// Handles requests to fetch all source measurements. Note that this will return ALL measurements,
        /// even older ones.
        /// </summary>
        /// <returns>Status 200 along with list of all source measurements.</returns>
        [HttpGet]
        public ActionResult<List<Source>> Get()
        {
            return Ok(_dataService.GetAll<Source>());
        }

        /// <summary>
        /// Handles requests to return a single measurement.
        /// </summary>
        /// <param name="id">The measurement id.</param>
        /// <returns>Status 200 along with the measurement.</returns>
        [HttpGet("{id:guid}")]
        public ActionResult<Source> GetSingle(string id)
        {
            Source measurement = _dataService.GetSingle<Source>(id);

            if (measurement == null)
            {
                return NotFound();
            }

            return Ok(measurement);
        }

        /// <summary>
        /// Handles requests to fetch the latest measurement for each source.
        /// </summary>
        /// <returns>Status 200 with the latest measurement for each source.</returns>
        [HttpGet("latest")]
        public ActionResult<Source> GetLatest()
        {
            return Ok(_dataService.GetLatest<Source, string>(item => item.SourceId));
        }

        /// <summary>
        /// Handles requests to add a new measurement to the database.
        /// </summary>
        /// <param name="source">The measurement.</param>
        /// <returns>Status 201 along with the measurement.</returns>
        [HttpPost]
        public ActionResult<Source> Create(Source source)
        {
            _dataService.Create(source);

            return CreatedAtAction(nameof(GetSingle), new Source { Id = source.Id }, source);
        }

        /// <summary>
        /// Handles requests to delete a measurement from the database.
        /// </summary>
        /// <param name="id">The source id.</param>
        /// <returns>Status 201 if successful, 404 if not found.</returns>
        [HttpDelete("{id:guid}")]
        public ActionResult Delete(string id)
        {
            Source measurement = _dataService.GetSingle<Source>(id);

            if (measurement == null)
            {
                return NotFound();
            }

            _dataService.Remove(measurement);

            return NoContent();
        }
    }
}