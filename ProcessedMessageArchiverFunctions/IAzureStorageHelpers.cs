using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProcessedMessageArchiverFunctions
{
    public interface IAzureStorageHelpers
    {
        CloudBlobClient ConnectToBlobClient(string accountName, string accountKey);
        CloudBlobContainer GetBlobContainer(CloudBlobClient client, string containerName);
        Task UploadBlobToStorage(CloudBlobContainer cloudBlobContainer, string blobName);
        Task DownloadBlob(CloudBlobContainer cloudBlobContainer, string blobName);
    }
}
