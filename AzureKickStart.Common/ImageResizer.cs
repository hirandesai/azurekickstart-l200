using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AzureKickStart.Common
{
    public class ImageResizer
    {
        private Image image;
        public ImageResizer(Image image)
        {
            this.image = image;
        }

        public Bitmap ResizeImage(int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public static Image GetImageFromByteArray(byte[] fileContent)
        {
            using (var ms = new MemoryStream(fileContent))
            {
                return Image.FromStream(ms);
            }
        }

        public static byte[] ResizeImage(ImageResizer imageResizer, int width, int height, ImageFormat format)
        {
            var resizedImage = imageResizer.ResizeImage(width, height);
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Save(ms, format);
                return ms.ToArray();
            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
