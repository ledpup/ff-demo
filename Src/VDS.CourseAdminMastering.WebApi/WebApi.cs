using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Http;

namespace VDS.CourseAdminMastering.WebApi
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class WebApi : StatelessService
    {
        public WebApi(StatelessServiceContext context)
            : base(context)
        { }


        public static bool AppConfigExists;

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                        var config = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config");

                        var environment = config.Settings.Sections["Environment"].Parameters["ASPNETCORE_ENVIRONMENT"].Value;

                        return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureAppConfiguration((builderContext, config) =>
                                    {
                                        config.AddJsonFile(environment, serviceContext);
                                        var settings = config.Build();
                                        AppConfigExists = !string.IsNullOrWhiteSpace(settings["VDSSettings:AppConfig"]);
                                        if (AppConfigExists)
                                        {
                                            config.AddAzureAppConfiguration(options => {
                                               options.Connect(settings["VDSSettings:AppConfig"])
                                                      .UseFeatureFlags(featureFlagOptions => {
                                                                featureFlagOptions.CacheExpirationTime = TimeSpan.FromSeconds(30); // 30 seconds is default
                                                           });
                                            });
                                        }
                                    })
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .ConfigureLogging(
                                        builder =>
                                        {
                                            builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                                                             (string.Empty, LogLevel.Information);
                                        }
                                    )
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }
    }
}
