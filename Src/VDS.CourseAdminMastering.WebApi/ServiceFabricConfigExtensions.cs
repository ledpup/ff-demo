using Microsoft.Extensions.Configuration;
using System.Fabric;

namespace VDS.CourseAdminMastering.WebApi
{
    public static class ServiceFabricConfigExtensions
    {
        public static IConfigurationBuilder AddJsonFile(
            this IConfigurationBuilder builder,
            string env,
            ServiceContext serviceContext)
        {
            // Get the Config package directory
            var configFolderPath = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config").Path;

            // Combine it with appsettings.json file name
            var appsettingsFile = env == "Development" ? "appsettings.Development.json" : "appsettings.json";
            var appSettingsFilePath = System.IO.Path.Combine(configFolderPath, appsettingsFile);

            // Add to the builder, making sure it will be reloaded every time the file changes, e.g. during Config-only deployment
            builder.AddJsonFile(appSettingsFilePath, optional: false, reloadOnChange: true);

            return builder;
        }
    }
}
