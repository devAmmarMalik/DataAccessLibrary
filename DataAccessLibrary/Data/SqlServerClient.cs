using Microsoft.Data.SqlClient;
using System.Data;
using DataAccessLibrary.Logging;

namespace DataAccessLibrary.Data;

public class SqlServerClient
{
    private readonly string _connectionString;
    public SqlServerClient(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DataTable ExecuteQuery(string query)
    { 
        DataTable retTable = new DataTable();
        try
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader read = command.ExecuteReader();
            retTable.Load(read);
            LoggingService.Initialize(@"f:\ammar\");
            LoggingService.Logger.Information($"Query Successful with {retTable.Rows.Count} rows");
        }
        catch (Exception ex) {
            LoggingService.Logger.Error(ex, "There was an issue with the query");
        }

        return retTable;
    }
}
