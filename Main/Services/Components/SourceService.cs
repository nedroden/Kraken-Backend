using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services.Components
{
    /// <summary>
    /// Facilitates CRUD operations for sources.
    /// </summary>
    public class SourceService : ComponentService<Source>
    {
        /// <summary>
        /// Creates a new source service.
        /// </summary>
        /// <param name="databaseEnvironment">Database connection settings.</param>
        public SourceService(IDatabaseEnvironment databaseEnvironment) : base(databaseEnvironment, "sources") { }
    }
}