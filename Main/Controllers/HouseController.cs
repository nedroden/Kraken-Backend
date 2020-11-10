using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

using Kraken.Api.Main.Services.Components;
using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Controllers
{
    /// <summary>
    /// The house controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HouseController : ControllerBase
    {
        private HouseService _houseService;
        private StreetService _streetService;

        /// <summary>
        /// Creates a new house controller.
        /// </summary>
        /// <param name="houseService">The house service.</param>
        /// <param name="streetService">The street service.</param>
        public HouseController(HouseService houseService, StreetService streetService)
        {
            _houseService = houseService;
            _streetService = streetService;
        }

        /// <summary>
        /// Handles requests to fetch all houses. Also automatically injects the street if StreetId if set.
        /// </summary>
        /// <returns>Status 200 along with a list of houses.</returns>
        [HttpGet]
        public ActionResult<List<House>> Get()
        {
            List<House> houses = _houseService.GetAll();

            houses.ForEach((House house) =>
            {
                if (house.StreetId == null)
                {
                    return;
                }

                house.Street = _streetService.GetSingle(house.StreetId);
            });

            return Ok(houses);
        }

        /// <summary>
        /// Handles requests to fetch a single house. Automatically injects the street if StreetId is set.
        /// </summary>
        /// <param name="id">The house id.</param>
        /// <returns>Status 200 along with the house if found, otherwise 404.</returns>
        [HttpGet("{id:length(24)}")]
        public ActionResult<House> GetSingle(string id)
        {
            House house = _houseService.GetSingle(id);

            if (house == null)
            {
                return NotFound();
            }

            if (house.StreetId != null)
            {
                house.Street = _streetService.GetSingle(house.StreetId);
            }

            return Ok(house);
        }

        /// <summary>
        /// Handles requests to save a house to the database. Subject to authorization.
        /// </summary>
        /// <param name="house"></param>
        /// <returns>Status 201 along with the house if authorized, otherwise 401.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult<House> Create(House house)
        {
            _houseService.Create(house);

            return CreatedAtAction(nameof(GetSingle), new { id = house.Id.ToString() }, house);
        }

        /// <summary>
        /// Handles requests to update a house. Subject to authorization.
        /// </summary>
        /// <param name="id">The house id.</param>
        /// <param name="updatedHouse">The (updated) house.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpPut("{id:length(24)}")]
        public ActionResult Update(string id, House updatedHouse)
        {
            if (!_houseService.Exists(id))
            {
                return NotFound();
            }

            _houseService.Update(id, updatedHouse);

            return NoContent();
        }

        /// <summary>
        /// Handles requests to delete a house from the database. Subject to authorization.
        /// </summary>
        /// <param name="id">The house id.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public ActionResult Delete(string id)
        {
            if (!_houseService.Exists(id))
            {
                return NotFound();
            }

            _houseService.Remove(id);

            return NoContent();
        }
    }
}