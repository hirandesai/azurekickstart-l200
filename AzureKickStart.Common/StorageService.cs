using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace AzureKickStart.Common
{
    public class StorageService
    {
        private readonly CloudBlobClient blobClient;

        public StorageService(string storageConnectionString)
        {
            var mycloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            blobClient = mycloudStorageAccount.CreateCloudBlobClient();
        }

        public async Task<string> UploadImageToAzureBlobStorageAsync(string containerName, string blobName, byte[] bytes, string contentType)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            await container.CreateIfNotExistsAsync();
            var blockBlob = container.GetBlockBlobReference(blobName);

            blockBlob.Properties.ContentType = contentType;

            await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

            return blockBlob.Uri.AbsoluteUri;
        }
    }
}
