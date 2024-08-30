using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace RequestsForData.Library.Internal.DataAccess
{
    internal class SqlDataAccess
    {
        public string GetConnectionString(string connectionString)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<SqlDataAccess>()
                .Build();

            // Equivalent connection string:
            // "User Id=<DB_USER>;Password=<DB_PASS>;Server=<INSTANCE_HOST>;Database=<DB_NAME>;"
            SqlConnectionStringBuilder connString = new()
            {

                // Set Host to 'cloudsql' when deploying to App Engine Flexible environment
                DataSource = config["DataSource"],
                InitialCatalog = config["DB_NAME"],
                UserID = config["DB_USER"],
                Password = config["DB_PASS"],

                // The Cloud SQL proxy provides encryption between the proxy and instance
                Encrypt = false,
            };
            connString.Pooling = true;

            //return connectionString;

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
