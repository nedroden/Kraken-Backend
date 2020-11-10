using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Kraken.Api.Main.Models;
using Kraken.Api.Main.Services.Components;

namespace Kraken.Api.Main.Controllers
{
    /// <summary>
    /// The area controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AreaController : ControllerBase
    {
        private AreaService _areaService;

        /// <summary>
        /// Creates a new area controller.
        /// </summary>
        /// <param name="areaService">The area service.</param>
        public AreaController(AreaService areaService) => _areaService = areaService;

        /// <summary>
        /// Handles requests to fetch all areas.
        /// </summary>
        /// <returns>A list of all areas.</returns>
        [HttpGet]
        public ActionResult<List<Area>> Get()
        {
            return Ok(_areaService.GetAll());
        }

        /// <summary>
        /// Handles requests to fetch a single area.
        /// </summary>
        /// <param name="id">The area id.</param>
        /// <returns>Status code 200 with the area if found, otherwise status code 404.</returns>
        [HttpGet("{id:length(24)}")]
        public ActionResult<Area> GetSingle(string id)
        {
            Area area = _areaService.GetSingle(id);

            if (area == null)
            {
                return NotFound();
            }

            return Ok(area);
        }

        /// <summary>
        /// Handles requests to save an area to the database. Subject to authorization.
        /// </summary>
        /// <param name="area">The area that should be saved.</param>
        /// <returns>Status code 201 along with the area if authenticated, otherwise 401.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult<Area> Create(Area area)
        {
            _areaService.Create(area);

            return CreatedAtAction(nameof(GetSingle), new { id = area.Id.ToString() }, area);
        }

        /// <summary>
        /// Handles requests to update an area. Subject to authorization.
        /// </summary>
        /// <param name="id">The area id.</param>
        /// <param name="updatedArea">The (updated) area.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if the record was not found.</returns>
        [Authorize]
        [HttpPut("{id:length(24)}")]
        public ActionResult Update(string id, Area updatedArea)
        {
            if (!_areaService.Exists(id))
            {
                return NotFound();
            }

            _areaService.Update(id, updatedArea);

            return NoContent();
        }

        /// <summary>
        /// Handles requests to delete an area from the database. Subject to authorization.
        /// </summary>
        /// <param name="id">The area id.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if the record was not found.</returns>
        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public ActionResult Delete(string id)
        {
            if (!_areaService.Exists(id))
            {
                return NotFound();
            }

            _areaService.Remove(id);

            return NoContent();
        }
    }
}