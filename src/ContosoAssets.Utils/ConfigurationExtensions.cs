using Microsoft.Extensions.Configuration;

namespace ContosoAssets.Utils
{
    public static class ConfigurationExtensions
    {
        public static string GetContosoAssetsConnectionString(this IConfiguration configuration,
            string connectionStringName)
        {
            return configuration[$"ConnectionStrings:{connectionStringName}"];
        }

        public static string GetContosoAssetsDefaultConnectionString(this IConfiguration configuration)
        {
            return configuration.GetContosoAssetsConnectionString("ContosoAssets");
        }
    }
}
