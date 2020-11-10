using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services.Components
{
    /// <summary>
    /// Facilitates CRUD operations for areas.
    /// </summary>
    public class AreaService : ComponentService<Area>
    {
        /// <summary>
        /// Creates a new area service.
        /// </summary>
        /// <param name="databaseEnvironment">Database connection settings.</param>
        public AreaService(IDatabaseEnvironment databaseEnvironment) : base(databaseEnvironment, "areas") { }
    }
}