using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Libraries.Shared;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace ProcessedMessageArchiverFunctions
{
    public class ProcessedMessageArchiverFunction
    {
        public IAzureStorageHelpers azureStorageHelpers = default(IAzureStorageHelpers);
        public IOptions<Settings> optionsSettings = default(IOptions<Settings>);

        public ProcessedMessageArchiverFunction(IAzureStorageHelpers azureStorageHelpers,
            IOptions<Settings> optionsSettings)
        {
            this.azureStorageHelpers = azureStorageHelpers;
            this.optionsSettings = optionsSettings;
        }

        [FunctionName("ProcessedMessageArchiverFunction")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "casestudy2db",
            collectionName: "sensordata",
            ConnectionStringSetting = "COSMOS_DB_CONNECTION_STRING",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists = true)]
            IReadOnlyList<Document> input, ILogger log)
        {
            try
            {
                List<DeviceReading> backupDocuments = new List<DeviceReading>();
                CloudBlobClient cloudBlobClient = azureStorageHelpers.ConnectToBlobClient(
                    optionsSettings.Value.STORAGE_ACCOUNT_NAME,
                    optionsSettings.Value.STORAGE_ACCOUNT_KEY);
                CloudBlobContainer blobContainer = azureStorageHelpers.GetBlobContainer(
                    cloudBlobClient,
                    optionsSettings.Value.STORAGE_CONTAINER);
                string backupFile = Path.Combine($"{DateTime.Now.ToString("dd-MM-yyyy")}-{new Random().Next(1, 10000000)}-backup.json");

                if (input != null && input.Count > 0)
                {
                    foreach (var document in input)
                    {
                        // Persist to blob storage
                        var deviceReading = JsonConvert.DeserializeObject<DeviceReading>(document.ToString());
                        backupDocuments.Add(deviceReading);
                        log.LogInformation($"{document.Id} has been added to list");
                    }
                }

                var jsonData = JsonConvert.SerializeObject(backupDocuments);

                using (StreamWriter file = File.CreateText(backupFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, jsonData);
                }

                await azureStorageHelpers.UploadBlobToStorage(blobContainer, backupFile);
            }
            catch (Exception ex)
            {
                log.LogWarning($"Something went wrong. Exception thrown: {ex.Message}");

                throw;
            }

            await Task.Yield();
        }
    }
}
