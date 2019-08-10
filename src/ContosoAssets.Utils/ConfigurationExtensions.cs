using Microsoft.Extensions.Configuration;

namespace ContosoAssets.Utils
{
    public static class ConfigurationExtensions
    {
        private const string localDatabaseConnectionName = "ContosoAssets";
        private const string dockerDatabaseConnectionName = "ContosoAssets_docker";

        public static string GetContosoAssetsConnectionString(this IConfiguration configuration,
            string connectionStringName)
        {
            return configuration[$"ConnectionStrings:{connectionStringName}"];
        }

        public static string GetContosoAssetsDefaultConnectionString(this IConfiguration configuration)
        {
            // This environment variable is set by the base image we are using
            var runningInDockerEnv = System.Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");

            var connectionStringName = localDatabaseConnectionName;

            if (bool.TryParse(runningInDockerEnv, out var isRunningInDocker))
            {
                connectionStringName = isRunningInDocker ? dockerDatabaseConnectionName : localDatabaseConnectionName;
            }

            return configuration.GetContosoAssetsConnectionString(connectionStringName);
        }
    }
}
