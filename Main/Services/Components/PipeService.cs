using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services.Components
{
    /// <summary>
    /// Facilitates CRUD operations for pipes.
    /// </summary>
    public class PipeService : ComponentService<Pipe>
    {
        /// <summary>
        /// Creates a new pipe service.
        /// </summary>
        /// <param name="databaseEnvironment">Database connection settings.</param>
        public PipeService(IDatabaseEnvironment databaseEnvironment) : base(databaseEnvironment, "pipes") { }
    }
}