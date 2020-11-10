using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

using Kraken.Api.Main.Models;
using Kraken.Api.Main.Services.Components;

namespace Kraken.Api.Main.Controllers
{
    /// <summary>
    /// The source controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SourceController : ControllerBase
    {
        private SourceService _sourceService;

        /// <summary>
        /// Creates a new source controller.
        /// </summary>
        /// <param name="sourceService">The source service.</param>
        public SourceController(SourceService sourceService) => _sourceService = sourceService;

        /// <summary>
        /// Handles requests to fetch all sources.
        /// </summary>
        /// <returns>Status 200 along with a list of all sources.</returns>
        [HttpGet]
        public ActionResult<List<Source>> Get()
        {
            return Ok(_sourceService.GetAll());
        }

        /// <summary>
        /// Handles requests to fetch a single source.
        /// </summary>
        /// <param name="id">The id of the source that should be fetched.</param>
        /// <returns>Status 200 along with the source if found, otherwise 404.</returns>
        [HttpGet("{id:length(24)}")]
        public ActionResult<Source> GetSingle(string id)
        {
            Source source = _sourceService.GetSingle(id);

            if (source == null)
            {
                return NotFound();
            }

            return Ok(source);
        }

        /// <summary>
        /// Handles requests to add a source to the database. Subject to authorization.
        /// </summary>
        /// <param name="source">The source that should be saved.</param>
        /// <returns>Status 201 along with the source if authorized, otherwise 401.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult<Source> Create(Source source)
        {
            _sourceService.Create(source);

            return CreatedAtAction(nameof(GetSingle), new { id = source.Id.ToString() }, source);
        }

        /// <summary>
        /// Handles requests to update a source. Subject to authorization.
        /// </summary>
        /// <param name="id">The id of the source.</param>
        /// <param name="updatedSource">An updated model of the source.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpPut("{id:length(24)}")]
        public ActionResult Update(string id, Source updatedSource)
        {
            if (!_sourceService.Exists(id))
            {
                return NotFound();
            }

            _sourceService.Update(id, updatedSource);

            return NoContent();
        }

        /// <summary>
        /// Handles requests to delete a source. Subject to authorization.
        /// </summary>
        /// <param name="id">The id of the source.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public ActionResult Delete(string id)
        {
            if (!_sourceService.Exists(id))
            {
                return NotFound();
            }

            _sourceService.Remove(id);

            return NoContent();
        }
    }
}