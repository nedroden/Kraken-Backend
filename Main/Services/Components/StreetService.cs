using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services.Components
{
    /// <summary>
    /// Facilitates CRUD operations for streets.
    /// </summary>
    public class StreetService : ComponentService<Street>
    {
        /// <summary>
        /// Creates a new street service.
        /// </summary>
        /// <param name="databaseEnvironment">Database connection settings.</param>
        public StreetService(IDatabaseEnvironment databaseEnvironment) : base(databaseEnvironment, "streets") { }
    }
}