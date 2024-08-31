using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace RequestsForData.Library.Internal.DataAccess
{
    internal class SqlDataAccess
    {
        private string GetConnectionString(string connectionString)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<SqlDataAccess>()
                .Build();

            return config[connectionString];
        }

        public List<dynamic> LoadData<U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            // Makes connection to DB
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                // Makes query to DB
                IEnumerable<dynamic> data = connection.Query(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

                List<dynamic> dataList = [.. data];

                return dataList;
            }
        }

        public void SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            // Makes connection to DB
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                // Makes execution of stored procedure to DB
                connection.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public void DeleteData(string storedProcedure, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(storedProcedure, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
