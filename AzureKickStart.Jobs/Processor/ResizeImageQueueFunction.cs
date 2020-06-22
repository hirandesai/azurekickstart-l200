using AzureKickStart.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace AzureKickStart.Jobs.Processor
{
    public class ResizeImageQueueFunction
    {
        private readonly StorageService storageService;
        public ResizeImageQueueFunction(StorageService storageService)
        {
            this.storageService = storageService;
        }
        public async Task Execute(ResizeImageQueueRequest request)
        {
            var fileContent = await storageService.DownloadBlob(request.ContainerName, request.FileName);
            var image = GetImageFromByteArray(fileContent);

            var imageResizer = new ImageResizer(image);

            var resizedImageContent = ResizeImage(imageResizer, 30, 30, ImageFormat.Jpeg);
            await UploadImage(request.ContainerName, request.FileName, 30, 30, resizedImageContent, "Image/jpeg");

            resizedImageContent = ResizeImage(imageResizer, 200, 200, ImageFormat.Jpeg);
            await UploadImage(request.ContainerName, request.FileName, 200, 200, resizedImageContent, "Image/jpeg");

            resizedImageContent = ResizeImage(imageResizer, 300, 450, ImageFormat.Jpeg);
            await UploadImage(request.ContainerName, request.FileName, 300, 450, resizedImageContent, "Image/jpeg");
        }

        private Image GetImageFromByteArray(byte[] fileContent)
        {
            using (var ms = new MemoryStream(fileContent))
            {
                return Image.FromStream(ms);
            }
        }

        private byte[] ResizeImage(ImageResizer imageResizer, int width, int height, ImageFormat format)
        {
            var resizedImage = imageResizer.ResizeImage(width, height);
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Save(ms, format);
                return ms.ToArray();
            }
        }

        private async Task UploadImage(string containerName, string originalFileName, int width, int height, byte[] fileContents, string contentType)
        {
            string newFileName = $"{Path.GetFileNameWithoutExtension(originalFileName)}-{width}x{height}{Path.GetExtension(originalFileName)}";
            await storageService.UploadImageToAzureBlobStorageAsync(containerName, newFileName, fileContents, contentType);
        }

    }



    public class ResizeImageQueueRequest
    {
        public string ContainerName { get; set; }
        public string FileName { get; set; }
    }
}
