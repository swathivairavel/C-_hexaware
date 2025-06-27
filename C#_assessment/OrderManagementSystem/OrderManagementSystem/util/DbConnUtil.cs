
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace OrderManagementSystem.util
{

    public static class DbConnUtil
    {
        private static IConfiguration _iconfiguration;

        static DbConnUtil()
        {
            GetAppSettingsFile();
        }

        private static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json");
            _iconfiguration = builder.Build();

        }

        public static string GetConnectionString()
        {
            return _iconfiguration.GetConnectionString("LocalConnectionString");
        }

        public static SqlConnection GetConnectionObject()
        {
            SqlConnection sqlConn = new SqlConnection(GetConnectionString());
            return sqlConn;
        }
    }
}
