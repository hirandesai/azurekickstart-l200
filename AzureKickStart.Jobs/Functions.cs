using AzureKickStart.Common;
using AzureKickStart.Jobs.Processor;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureKickStart.Jobs
{
    public class Functions
    {
        public static async Task ResizeImageQueue([QueueTrigger("resizeimagequeue")] string message, TextWriter log)
        {
            try
            {
                await log.WriteLineAsync(message);

                var storageConnectionString = new ConfigurationService().GetConnectionStringValue("StorageConnectionString");

                ResizeImageQueueFunction resizeImageQueueFunction = new ResizeImageQueueFunction(new StorageService(storageConnectionString));
                await resizeImageQueueFunction.Execute(JsonConvert.DeserializeObject<ResizeImageQueueRequest>(message));
            }
            catch (Exception ex)
            {
                await log.WriteLineAsync(ex.Message);
            }
        }
    }
}
