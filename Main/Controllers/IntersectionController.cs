using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

using Kraken.Api.Main.Services.Components;
using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Controllers
{
    /// <summary>
    /// The intersection controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IntersectionController : ControllerBase
    {
        private IntersectionService _intersectionService;

        /// <summary>
        /// Creates a new intersection controller.
        /// </summary>
        /// <param name="intersectionService">The intersection service.</param>
        public IntersectionController(IntersectionService intersectionService) => _intersectionService = intersectionService;

        /// <summary>
        /// Handles requests to fetch all intersections from the database.
        /// </summary>
        /// <returns>Status 200 along with a list of all intersections.</returns>
        [HttpGet]
        public ActionResult<List<Intersection>> Get()
        {
            return Ok(_intersectionService.GetAll());
        }

        /// <summary>
        /// Handles requests to fetch a single intersection from the database.
        /// </summary>
        /// <param name="id">The intersection id.</param>
        /// <returns>Status 200 along with the intersection if found, otherwise 404.</returns>
        [HttpGet("{id:length(24)}")]
        public ActionResult<Intersection> GetSingle(string id)
        {
            Intersection intersection = _intersectionService.GetSingle(id);

            if (intersection == null)
            {
                return NotFound();
            }

            return Ok(intersection);
        }

        /// <summary>
        /// Handles requests to create an intersection. Subject to authorization.
        /// </summary>
        /// <param name="intersection">The intersection that should be stored.</param>
        /// <returns>201 along with the intersection if authorized, otherwise 401.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult<Intersection> Create(Intersection intersection)
        {
            _intersectionService.Create(intersection);

            return CreatedAtAction(nameof(GetSingle), new { id = intersection.Id.ToString() }, intersection);
        }

        /// <summary>
        /// Handles requests to update an intersection. Subject to authorization.
        /// </summary>
        /// <param name="id">The id of the intersection that should be updated.</param>
        /// <param name="updatedIntersection">The (updated) intersection.</param>
        /// <returns>204 if successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpPut("{id:length(24)}")]
        public ActionResult Update(string id, Intersection updatedIntersection)
        {
            if (!_intersectionService.Exists(id))
            {
                return NotFound();
            }

            _intersectionService.Update(id, updatedIntersection);

            return NoContent();
        }

        /// <summary>
        /// Handles requests to delete an intersection from the database. Subject to authorization.
        /// </summary>
        /// <param name="id">The intersection id.</param>
        /// <returns>204 is successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public ActionResult Delete(string id)
        {
            if (!_intersectionService.Exists(id))
            {
                return NotFound();
            }

            _intersectionService.Remove(id);

            return NoContent();
        }
    }
}