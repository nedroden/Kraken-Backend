using Microsoft.Extensions.Hosting;
using System;

namespace Kraken.Api.Sensor.Services
{
    /// <summary>
    /// Provides an easy way to manage the application lifetime.
    /// </summary>
    public class ApplicationService
    {
        IHostApplicationLifetime _lifetime;

        /// <summary>
        /// Creates a new instance of the application service.
        /// </summary>
        /// <param name="lifetime">A host application lifetime object.</param>
        public ApplicationService(IHostApplicationLifetime lifetime) => _lifetime = lifetime;

        /// <summary>
        /// Displays an error message and stops the application nicely.
        /// </summary>
        /// <param name="message">The message to display just before stopping the application.</param>
        public void Crash(string message)
        {
            Console.Error.WriteLine(message);
            _lifetime.StopApplication();
        }
    }
}