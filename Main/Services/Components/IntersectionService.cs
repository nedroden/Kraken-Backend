using Kraken.Api.Main.Models;

namespace Kraken.Api.Main.Services.Components
{
    /// <summary>
    /// Facilitates CRUD operations for intersections.
    /// </summary>
    public class IntersectionService : ComponentService<Intersection>
    {
        /// <summary>
        /// Creates a new intersection service.
        /// </summary>
        /// <param name="databaseEnvironment">Database connection settings.</param>
        public IntersectionService(IDatabaseEnvironment databaseEnvironment) : base(databaseEnvironment, "intersections") { }
    }
}