using System.Data;
using MySqlConnector;

namespace RaceFlowAPI.Database;

public static class DatabaseClient
{
    internal static async Task<MySqlConnection> GetOpenConnectionAsync()
    {
        var connection = DatabaseConnectionPool.GetConnection();
        await DatabaseConnectionPool.OpenConnectionAsync(connection);
        return connection;
    }

    internal static async Task CloseConnectionAsync(MySqlConnection connection)
    {
        if (connection.State != ConnectionState.Closed)
            await connection.CloseAsync().ConfigureAwait(false);
    }
}