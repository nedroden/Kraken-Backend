using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Kraken.Api.Sensor.Models;
using Kraken.Api.Sensor.Services;

namespace Kraken.Api.Sensor.Controllers
{
    /// <summary>
    /// The house measurement controller.
    /// </summary>
    [ApiController]
    [Route("api/measurements/[controller]")]
    public class HouseController : ControllerBase
    {
        private DataService _dataService;

        /// <summary>
        /// Creates a new house measurement controller.
        /// </summary>
        /// <param name="dataService">The data service.</param>
        public HouseController(DataService dataService) => _dataService = dataService;

        /// <summary>
        /// Handles requests to fetch all house measurements. Note that this will return ALL measurements,
        /// even older ones.
        /// </summary>
        /// <returns>Status 200 along with list of all house measurements.</returns>
        [HttpGet]
        public ActionResult<List<House>> Get()
        {
            return Ok(_dataService.GetAll<House>());
        }

        /// <summary>
        /// Handles requests to return a single measurement.
        /// </summary>
        /// <param name="id">The measurement id.</param>
        /// <returns>Status 200 along with the measurement.</returns>
        [HttpGet("{id:guid}")]
        public ActionResult<House> GetSingle(string id)
        {
            House measurement = _dataService.GetSingle<House>(id);

            if (measurement == null)
            {
                return NotFound();
            }

            return Ok(measurement);
        }

        /// <summary>
        /// Handles requests to fetch the latest measurement for each house.
        /// </summary>
        /// <returns>Status 200 with the latest measurement for each house.</returns>
        [HttpGet("latest")]
        public ActionResult<House> GetLatest()
        {
            return Ok(_dataService.GetLatest<House, string>(item => item.HouseId));
        }

        /// <summary>
        /// Handles requests to add a new measurement to the database.
        /// </summary>
        /// <param name="house">The measurement.</param>
        /// <returns>Status 201 along with the measurement.</returns>
        [HttpPost]
        public ActionResult<House> Create(House house)
        {
            _dataService.Create(house);

            return CreatedAtAction(nameof(GetSingle), new House { Id = house.Id }, house);
        }

        /// <summary>
        /// Handles requests to delete a measurement from the database.
        /// </summary>
        /// <param name="id">The house id.</param>
        /// <returns>Status 201 if successful, 404 if not found.</returns>
        [HttpDelete("{id:guid}")]
        public ActionResult Delete(string id)
        {
            House measurement = _dataService.GetSingle<House>(id);

            if (measurement == null)
            {
                return NotFound();
            }

            _dataService.Remove(measurement);

            return NoContent();
        }
    }
}