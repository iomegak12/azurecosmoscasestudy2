using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Libraries.Shared;
using Newtonsoft.Json;

namespace SensorDataProcessorFunctions
{
    public class SensorDataProcessorFunction
    {
        private IOptions<Settings> optionsSettings = default(IOptions<Settings>);
        private CosmosClient cosmosClient = default(CosmosClient);

        public SensorDataProcessorFunction(IOptions<Settings> optionsSettings, CosmosClient cosmosClient)
        {
            this.optionsSettings = optionsSettings;
            this.cosmosClient = cosmosClient;
        }

        [FunctionName("SensorDataProcessorFunction")]
        public async Task Run(
            [EventHubTrigger("sensormessages", Connection = "EVENT_HUBS_CONNECTION_STRING")]
            EventData[] events,
            ILogger log, ExecutionContext executionContext)
        {
            var exceptions = new List<Exception>();

            var connectionString = this.optionsSettings.Value.COSMOS_CONNECTION_STRING;
            var databaseId = this.optionsSettings.Value.COSMOS_DB;
            var containerId = this.optionsSettings.Value.COSMOS_CONTAINER;

            this.cosmosClient = new CosmosClient(connectionString);
            var container = this.cosmosClient.GetContainer(databaseId, containerId);

            foreach (EventData eventData in events)
            {
                try
                {
                    var messageBody = Encoding.UTF8.GetString(
                        eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    var telementryEvent = JsonConvert.DeserializeObject<DeviceReading>(messageBody);

                    await container.CreateItemAsync(telementryEvent);

                    log.LogInformation($"{telementryEvent.DeviceId} has been persisted");
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();

            await Task.Yield();
        }
    }
}
