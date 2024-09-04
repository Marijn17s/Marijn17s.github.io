using System.Data;
using MySqlConnector;

namespace RaceFlowAPI.Database;

internal struct DatabaseConnectionPool
{
    private const string ConnectionString = "server=mysql6008.site4now.net;uid=aac827_rcflow;pwd=SummaIct2024!;database=db_aac827_rcflow;ConvertZeroDateTime=true;"; // Pooling=true;MinimumPoolSize=5;
    private static readonly List<MySqlConnection> ConnectionPool = [];

    internal static MySqlConnection GetConnection()
    {
        // Return an available connection
        
        for (var i = 0; i < ConnectionPool.Count; i++)
        {
            var connection = ConnectionPool[i];
            if (connection.State is ConnectionState.Closed) return connection; // Connection is closed and available

            if (i != ConnectionPool.Count - 1) continue; // Current iteration is not the last in the pool so we check the next connection
            var newConnection = new MySqlConnection(ConnectionString);
            ConnectionPool.Add(newConnection);
            return newConnection;
        }

        var firstConnection = new MySqlConnection(ConnectionString);
        ConnectionPool.Add(firstConnection);
        return firstConnection;
    }
    
    internal static async Task OpenConnectionAsync(MySqlConnection connection)
    {
        if (connection.State != ConnectionState.Closed)
            await connection.CloseAsync().ConfigureAwait(false);
        
        await connection.OpenAsync().ConfigureAwait(false);
    }

    internal static async Task CloseConnectionAsync(MySqlConnection connection)
    {
        if (connection.State != ConnectionState.Closed)
            await connection.CloseAsync().ConfigureAwait(false);
    }
}