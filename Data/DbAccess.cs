using Microsoft.Data.SqlClient;
using System.Data;

namespace MiniAccountManagementSystem.Data
{
    public class DbAccess
    {
        private readonly string _connectionString;

        public DbAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(string storedProcedure, SqlParameter[] parameters, Func<SqlDataReader, T> mapper)
        {
            var results = new List<T>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(storedProcedure, connection) { CommandType = CommandType.StoredProcedure })
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            results.Add(mapper(reader));
                        }
                    }
                }
            }
            return results;
        }

        public async Task<int> ExecuteNonQueryAsync(string storedProcedure, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(storedProcedure, connection) { CommandType = CommandType.StoredProcedure })
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
