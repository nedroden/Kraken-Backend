namespace Kraken.Api.Sensor.Models
{
    /// <summary>
    /// Database settings format.
    /// </summary>
    public interface IConnectionInfo
    {
        /// <value>The hostname.</value>
        string HostName { get; set; }

        /// <value>The keyspace name</value>
        string KeyspaceName { get; set; }
    }

    /// <summary>A key-value set of database settings.</summary>
    public class ConnectionInfo : IConnectionInfo
    {
        /// <value>The hostname.</value>
        public string HostName { get; set; }

        /// <value>The keyspace name</value>
        public string KeyspaceName { get; set; }
    }
}