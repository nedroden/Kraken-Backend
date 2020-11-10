using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Kraken.Api.Sensor.Models;
using Kraken.Api.Sensor.Services;

namespace Kraken.Api.Sensor.Controllers
{
    /// <summary>
    /// The pipe measurement controller.
    /// </summary>
    [ApiController]
    [Route("api/measurements/[controller]")]
    public class PipeController : ControllerBase
    {
        private DataService _dataService;

        /// <summary>
        /// Creates a new pipe measurement controller.
        /// </summary>
        /// <param name="dataService">The data service.</param>
        public PipeController(DataService dataService) => _dataService = dataService;

        /// <summary>
        /// Handles requests to fetch all pipe measurements. Note that this will return ALL measurements,
        /// even older ones.
        /// </summary>
        /// <returns>Status 200 along with list of all pipe measurements.</returns>
        [HttpGet]
        public ActionResult<List<Pipe>> Get()
        {
            return Ok(_dataService.GetAll<Pipe>());
        }

        /// <summary>
        /// Handles requests to return a single measurement.
        /// </summary>
        /// <param name="id">The measurement id.</param>
        /// <returns>Status 200 along with the measurement.</returns>
        [HttpGet("{id:guid}")]
        public ActionResult<Pipe> GetSingle(string id)
        {
            Pipe measurement = _dataService.GetSingle<Pipe>(id);

            if (measurement == null)
            {
                return NotFound();
            }

            return Ok(measurement);
        }

        /// <summary>
        /// Handles requests to fetch the latest measurement for each pipe.
        /// </summary>
        /// <returns>Status 200 with the latest measurement for each pipe.</returns>
        [HttpGet("latest")]
        public ActionResult<Pipe> GetLatest()
        {
            return Ok(_dataService.GetLatest<Pipe, string>(item => item.PipeId));
        }

        /// <summary>
        /// Handles requests to add a new measurement to the database.
        /// </summary>
        /// <param name="pipe">The measurement.</param>
        /// <returns>Status 201 along with the measurement.</returns>
        [HttpPost]
        public ActionResult<Pipe> Create(Pipe pipe)
        {
            _dataService.Create(pipe);

            return CreatedAtAction(nameof(GetSingle), new Pipe { Id = pipe.Id }, pipe);
        }

        /// <summary>
        /// Handles requests to delete a measurement from the database.
        /// </summary>
        /// <param name="id">The pipe id.</param>
        /// <returns>Status 201 if successful, 404 if not found.</returns>
        [HttpDelete("{id:guid}")]
        public ActionResult Delete(string id)
        {
            Pipe measurement = _dataService.GetSingle<Pipe>(id);

            if (measurement == null)
            {
                return NotFound();
            }

            _dataService.Remove(measurement);

            return NoContent();
        }
    }
}