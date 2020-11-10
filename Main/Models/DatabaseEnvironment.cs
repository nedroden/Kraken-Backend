namespace Kraken.Api.Main.Models
{
    /// <summary>
    /// Database settings format.
    /// </summary>
    public interface IDatabaseEnvironment
    {
        /// <value>The connection string.</value>
        string ConnectionString { get; set; }

        /// <value>The database name.</value>
        string DatabaseName { get; set; }
    }

    /// <summary>A key-value set of database settings.</summary>
    public class DatabaseEnvironment : IDatabaseEnvironment
    {
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <value>The database name.</value>
        public string DatabaseName { get; set; }
    }
}