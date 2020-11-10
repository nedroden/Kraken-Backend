using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Kraken.Api.Main.Models.Measurements;
using Kraken.Api.Main.Services;

namespace Kraken.Api.Main.Controllers
{
    /// <summary>
    /// The measurement controller. All requests are forwarded to the sensor api using the Deferrer class.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MeasurementController : ControllerBase
    {
        private readonly Deferrer _deferrer;

        /// <summary>
        /// Creates a new measurement controller.
        /// </summary>
        /// <param name="deferrer">A request deferrer.</param>
        public MeasurementController(Deferrer deferrer) => _deferrer = deferrer;

        /// <summary>
        /// Handles requests to get the lastest house measurements.
        /// </summary>
        /// <returns>Status 200 along with the latest house measurements, 503 upon failure.</returns>
        [HttpGet("house")]
        public async Task<ActionResult<List<House>>> GetHouseMeasurements()
        {
            try
            {
                List<House> measurements =
                    await _deferrer.DeferGetRequest<List<House>>(TargetApi.Sensor, "/measurements/house/latest");

                return Ok(measurements);
            }
            catch (HttpRequestException exception)
            {
                Console.Error.WriteLine($"Could not get house measurements: ${exception.Message}");

                return StatusCode(503);
            }
        }

        /// <summary>
        /// Handles requests to get the lastest pipe measurements.
        /// </summary>
        /// <returns>Status 200 along with the latest pipe measurements, 503 upon failure.</returns>
        [HttpGet("pipe")]
        public async Task<ActionResult<List<Pipe>>> GetPipeMeasurements()
        {
            try
            {
                List<Pipe> measurements =
                    await _deferrer.DeferGetRequest<List<Pipe>>(TargetApi.Sensor, "/measurements/pipe/latest");

                return Ok(measurements);
            }
            catch (HttpRequestException exception)
            {
                Console.Error.WriteLine($"Could not get pipe measurements: ${exception.Message}");

                return StatusCode(503);
            }
        }

        /// <summary>
        /// Handles requests to get the lastest source measurements.
        /// </summary>
        /// <returns>Status 200 along with the latest source measurements, 503 upon failure.</returns>
        [HttpGet("source")]
        public async Task<ActionResult<List<Source>>> GetSourceMeasurements()
        {
            try
            {
                List<Source> measurements =
                    await _deferrer.DeferGetRequest<List<Source>>(TargetApi.Sensor, "/measurements/source/latest");

                return Ok(measurements);
            }
            catch (HttpRequestException exception)
            {
                Console.Error.WriteLine($"Could not get source measurements: ${exception.Message}");

                return StatusCode(503);
            }
        }
    }
}