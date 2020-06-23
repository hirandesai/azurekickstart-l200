using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKickStart.Common
{
    public class ResizeImageQueueRequest
    {
        public string ContainerName { get; set; }
        public string FileName { get; set; }
    }

    public class QueueService
    {
        private readonly CloudQueueClient queueClient;
        public QueueService(string storageConnectionString)
        {
            var mycloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            queueClient = mycloudStorageAccount.CreateCloudQueueClient();
        }

        public async Task InsertMessage(string queueName, ResizeImageQueueRequest message)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();

            // Send a message to the queue
            string messageJSON = JsonConvert.SerializeObject(message);
            var fileMessage = new CloudQueueMessage(messageJSON);
            await queue.AddMessageAsync(fileMessage);
        }
    }
}
