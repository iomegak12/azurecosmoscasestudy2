using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorDataProcessorFunctions
{
    public static class StartupExtensions
    {
        public static void AddCosmosDBClient(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<CosmosClient>((s) =>
            {
                var settings = s.GetService<IOptions<Settings>>();

                return new CosmosClient(connectionString: settings.Value.COSMOS_CONNECTION_STRING);
            });
        }
    }
}
