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

        public async Task<Byte[]> DownloadBlob(string containerName, string file)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(file);

            await blockBlob.FetchAttributesAsync();
            long fileByteLength = blockBlob.Properties.Length;
            byte[] fileContent = new byte[fileByteLength];
            for (int i = 0; i < fileByteLength; i++)
            {
                fileContent[i] = 0x20;
            }
            await blockBlob.DownloadToByteArrayAsync(fileContent, 0);
            return fileContent;
        }

        public async Task<string> UploadImageToAzureBlobStorageAsync(string containerName, string blobName, byte[] bytes, string contentType)
        {
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            
            var blockBlob = container.GetBlockBlobReference(blobName);

            blockBlob.Properties.ContentType = contentType;

            await blockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

            return blockBlob.Uri.AbsoluteUri;
        }
    }
}
