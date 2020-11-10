using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

using Kraken.Api.Main.Models;
using Kraken.Api.Main.Services.Components;

namespace Kraken.Api.Main.Controllers
{
    /// <summary>
    /// The pipe controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PipeController : ControllerBase
    {
        private PipeService _pipeService;

        /// <summary>
        /// Creates a new pipe controller.
        /// </summary>
        /// <param name="pipeService">The pipe service.</param>
        public PipeController(PipeService pipeService) => _pipeService = pipeService;

        /// <summary>
        /// Handles requests to fetch all pipes.
        /// </summary>
        /// <returns>Status 200 along with a list of all pipes.</returns>
        [HttpGet]
        public ActionResult<List<Pipe>> Get()
        {
            return Ok(_pipeService.GetAll());
        }

        /// <summary>
        /// Handles requests to fetch a single pipe.
        /// </summary>
        /// <param name="id">The id of the pipe that should be fetched.</param>
        /// <returns>Status 200 along with the pipe if found, otherwise 404.</returns>
        [HttpGet("{id:length(24)}")]
        public ActionResult<Pipe> GetSingle(string id)
        {
            Pipe pipe = _pipeService.GetSingle(id);

            if (pipe == null)
            {
                return NotFound();
            }

            return Ok(pipe);
        }

        /// <summary>
        /// Handles requests to create a new pipe. Subject to authorization.
        /// </summary>
        /// <param name="pipe">The pipe that should be saved.</param>
        /// <returns>Status 201 if successful, 401 if unauthorized.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult<Pipe> Create(Pipe pipe)
        {
            _pipeService.Create(pipe);

            return CreatedAtAction(nameof(GetSingle), new { id = pipe.Id.ToString() }, pipe);
        }

        /// <summary>
        /// Handles requests to update an existing pipe. Subject to authorization.
        /// </summary>
        /// <param name="id">The id of the pipe that should be updated.</param>
        /// <param name="updatedPipe">An updated version of the pipe.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpPut("{id:length(24)}")]
        public ActionResult Update(string id, Pipe updatedPipe)
        {
            if (!_pipeService.Exists(id))
            {
                return NotFound();
            }

            _pipeService.Update(id, updatedPipe);

            return NoContent();
        }

        /// <summary>
        /// Handles requests to delete a pipe from the database. Subject to authorization.
        /// </summary>
        /// <param name="id">The id of the pipe to remove.</param>
        /// <returns>Status 204 if successful, 401 if unauthorized, 404 if not found.</returns>
        [Authorize]
        [HttpDelete("{id:length(24)}")]
        public ActionResult Delete(string id)
        {
            if (!_pipeService.Exists(id))
            {
                return NotFound();
            }

            _pipeService.Remove(id);

            return NoContent();
        }
    }
}