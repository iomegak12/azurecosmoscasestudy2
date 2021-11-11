using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace TriggerEventsHandlerFunctions
{
    public static class StartupExtensions
    {
        public static void AddEventHubsClient(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<EventHubProducerClient>((s) =>
            {
                var settings = s.GetService<IOptions<Settings>>();

                return new EventHubProducerClient(
                    connectionString: settings.Value.EVENT_HUB_CONNECTION_STRING,
                    eventHubName: settings.Value.EVENT_HUB_NAME);
            });
        }
    }
}
