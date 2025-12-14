using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace frontend.Services
{
    /// <summary>
    /// Image processor implementation for converting between Mat and Bitmap formats.
    /// </summary>
    public class ImageProcessor : IImageProcessor
    {
        public Bitmap MatToBitmap(Mat mat)
        {
            if (mat == null || mat.IsEmpty)
                throw new ArgumentException("Mat is null or empty", nameof(mat));

            // For BGR format (original camera frame)
            using Image<Bgr, byte> image = mat.ToImage<Bgr, byte>();
            return image.AsBitmap();
        }

        public Bitmap MatToBitmapRgb(Mat mat)
        {
            if (mat == null || mat.IsEmpty)
                throw new ArgumentException("Mat is null or empty", nameof(mat));

            // For RGB format (converted for display)
            using Image<Rgb, byte> image = mat.ToImage<Rgb, byte>();
            return image.AsBitmap();
        }

        public byte[] BitmapToJpegBytes(Bitmap bitmap, int quality = 90)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            using MemoryStream ms = new MemoryStream();
            var jpegEncoder = ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

            if (jpegEncoder != null)
            {
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
                bitmap.Save(ms, jpegEncoder, encoderParams);
            }
            else
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
            }

            return ms.ToArray();
        }

        public Mat CropWithPadding(Mat source, Rectangle region, int padding)
        {
            if (source == null || source.IsEmpty)
                throw new ArgumentException("Source Mat is null or empty", nameof(source));

            int x = Math.Max(0, region.X - padding);
            int y = Math.Max(0, region.Y - padding);
            int width = Math.Min(source.Width - x, region.Width + padding * 2);
            int height = Math.Min(source.Height - y, region.Height + padding * 2);

            Rectangle cropRect = new Rectangle(x, y, width, height);
            return new Mat(source, cropRect);
        }
    }
}

