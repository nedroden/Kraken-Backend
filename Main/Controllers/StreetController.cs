using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

using Kraken.Api.Main.Models;
using Kraken.Api.Main.Services.Components;

namespace Kraken.Api.Main.Controllers
{
    /// <summary>
    /// The street controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StreetController : ControllerBase
    {
        private StreetService _streetService;
        private AreaService _areaService;

        /// <summary>
        /// Creates a new street controller.
        /// </summary>
        /// <param name="streetService">The street service.</param>
        /// <param name="areaService">The area service.</param>
        public StreetController(StreetService streetService, AreaService areaService)
        {
            _streetService = streetService;
            _areaService = areaService;
        }

        /// <summary>
        /// Handles requests to get all streets. Automatically injects the area if AreaId is set.
        /// </summary>
        /// <returns>Status 200 along with a list of all streets.</returns>
        [HttpGet]
        public ActionResult<List<Street>> Get()
        {
            List<Street> streets = _streetService.GetAll();

            streets.ForEach((Street street) =>
            {
                if (street.AreaId == null)
                {
                    return;
                }

                street.Area = _areaService.GetSingle(street.AreaId);
            });

            return Ok(streets);
        }

        /// <summary>
        /// Handles requests to fetch a single street from the database. Automatically injects the
        /// area if AreaId is set.
        /// </summary>
        /// <param name="id">The id of the street that should be fetched.</param>
        /// <returns>Status 200 along with the street if found, otherwise 404.</returns>
        [HttpGet("{id:length(24)}")]
        public ActionResult<Street> GetSingle(string id)
        {
            Street street = _streetService.GetSingle(id);

            if (street == null)
            {
                return NotFound();
            }

            if (street.AreaId != null)
            {
                Area area = _areaService.GetSingle(street.AreaId);
                street.Area = area;
            }

            return Ok(street);
        }

        /// <summary>
        /// Handles requests to save a street to the database. Subject to authorization.
        /// </summary>
        /// <param name="street">The street that should be saved.</param>
        /// <returns>Status 201 along with the street if successful, 401 if unauthorized.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult<Street> Create(Street street)
        {
            _streetService.Create(street);

            return CreatedAtAction(nameof(GetSingle), new { id = street.Id.ToString() }, street);
        }

        /// <summary>
        /// Handles requests to update a street. Subject to authorization.
        /// </summary>
        /// <param name="id">The id of the street.</param>
        /// <param name="updatedStreet">An updated model of the street.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpPut("{id:length(24)}")]
        public ActionResult Update(string id, Street updatedStreet)
        {
            if (!_streetService.Exists(id))
            {
                return NotFound();
            }

            _streetService.Update(id, updatedStreet);

            return NoContent();
        }

        /// <summary>
        /// Handles requests to delete a street from the database. Subject to authorization.
        /// </summary>.
        /// <param name="id">The id of the street that should be removed.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public ActionResult Delete(string id)
        {
            if (!_streetService.Exists(id))
            {
                return NotFound();
            }

            _streetService.Remove(id);

            return NoContent();
        }
    }
}