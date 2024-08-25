using Dapper;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace RequestsForData.Library.Internal.DataAccess
{
    internal class SqlDataAccess
    {
        //private readonly IConfiguration Configuration;

        //public SqlDataAccess(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        public string GetConnectionString(string connectionString)
        {
            //var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            //    .AddUserSecrets<SqlDataAccess>()
            //    .Build;

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<SqlDataAccess>()
                .Build();


            //var webApp = WebApplication.Create();
            //var config = webApp.Configuration["DefaultConnection"];
            Debug.WriteLine(config["PublicHolidays"]);
            return @"DataSource=LTPC2DXY6H\SQLEXPRESS;InitialCatalog=PublicHolidaysData;Integrated Security=True;Multiple Active Result Sets=True;Trust Server Certificate=True";
        }

        public List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
        {
            string connectionString = GetConnectionString(connectionStringName);

            // Makes connection to DB
            using (IDbConnection connection = new SqlConnection(@"Server=LTPC2DXY6H\SQLEXPRESS;Database=PublicHolidaysData;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True"))
            {
                // Makes query to DB
                List<T> rows = connection.Query<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure).ToList();

               return rows;
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
    }
}
