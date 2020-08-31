using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;

namespace AzureKickStart.Functions
{
    public class ResizeImagesFunction
    {

        private static string key = System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);

        private static TelemetryClient telemetryClient = new TelemetryClient() { InstrumentationKey = key };
  

        [FunctionName("ResizeImagesFunction")]
        public void Run(
            [BlobTrigger("profile-images/original/{name}", Connection = "StorageConnectionString")]Stream inputblob,
            [Blob("profile-images/thumbnails/{name}", FileAccess.Write, Connection = "StorageConnectionString")]Stream thumbnailBlob,
            string name, ILogger log)
        {   
            telemetryClient.TrackTrace($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inputblob.Length} Bytes");

            using (Image image = Image.Load(inputblob))
            {
                image.Mutate(x => x
                        .Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.BoxPad,
                            Size = new Size(200, 200)
                        }).BackgroundColor(new Rgba32(0, 0, 0)));


                image.SaveAsJpegAsync(thumbnailBlob);
            }
            telemetryClient.TrackTrace($"C# Blob trigger function:: Thumbnail image blob created\n Name:{name}");
        }
    }
}
