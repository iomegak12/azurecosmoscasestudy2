using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProcessedMessageArchiverFunctions;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ProcessedMessageArchiverFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            builder
                .Services
                .AddOptions<Settings>()
                .Configure<IConfiguration>((settings, existingConfiguration) =>
                {
                    var configurationBuilder = new ConfigurationBuilder();
                    var configuration =
                        configurationBuilder
                            .SetBasePath(builder.GetContext().ApplicationRootPath)
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

                    configuration.GetSection(nameof(Settings)).Bind(settings);
                });

            builder.Services.AddSingleton<IAzureStorageHelpers, AzureStorageHelpers>();
        }
    }
}
