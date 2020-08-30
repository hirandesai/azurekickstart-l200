using System;
using System.Drawing.Imaging;
using System.IO;
using AzureKickStart.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AzureKickStart.Functions
{
    public static class ResizeImagesFunction
    {
        [FunctionName("ResizeImagesFunction")]
        public static void Run(
            [BlobTrigger("profile-images/original/{name}", Connection = "StorageConnectionString")]Stream inputblob,
            [Blob("profile-images/thumbnails/{name}", FileAccess.Write, Connection = "StorageConnectionString")]Stream thumbnailBlob,
            string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {inputblob.Length} Bytes");

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
            log.LogInformation($"C# Blob trigger function:: Thumbnail image blob created\n Name:{name}");
        }
    }
}
