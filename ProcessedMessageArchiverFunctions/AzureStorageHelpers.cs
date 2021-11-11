﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProcessedMessageArchiverFunctions
{
    public class AzureStorageHelpers : IAzureStorageHelpers
    {
        public CloudBlobClient ConnectToBlobClient(string accountName, string accountKey)
        {
            try
            {
                StorageCredentials storageCredentials = new StorageCredentials(accountName, accountKey);
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                return blobClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown due to: {ex.Message}");
                throw;
            }
        }

        public CloudBlobContainer GetBlobContainer(CloudBlobClient client, string containerName)
        {
            try
            {
                CloudBlobContainer container = client.GetContainerReference(containerName);
                return container;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown due to: {ex.Message}");
                throw;
            }
        }

        public async Task UploadBlobToStorage(CloudBlobContainer cloudBlobContainer, string blobName)
        {
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            await cloudBlockBlob.UploadFromFileAsync(blobName);
        }

        public async Task DownloadBlob(CloudBlobContainer cloudBlobContainer, string blobName)
        {
            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            await cloudBlockBlob.DownloadToFileAsync(blobName, FileMode.Open);
        }
    }
}
