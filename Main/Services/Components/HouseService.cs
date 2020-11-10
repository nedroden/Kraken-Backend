using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services.Components
{
    /// <summary>
    /// Facilitates CRUD operations for houses.
    /// </summary>
    public class HouseService : ComponentService<House>
    {
        /// <summary>
        /// Creates a new house service.
        /// </summary>
        /// <param name="databaseEnvironment">Database connection settings.</param>
        public HouseService(IDatabaseEnvironment databaseEnvironment) : base(databaseEnvironment, "houses") { }
    }
}