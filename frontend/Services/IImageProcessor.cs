using System.Drawing;
using Emgu.CV;

namespace frontend.Services
{
    /// <summary>
    /// Interface for image processing operations.
    /// Abstracts image conversion and manipulation operations.
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// Converts an Emgu.CV Mat to a Bitmap for display.
        /// </summary>
        Bitmap MatToBitmap(Mat mat);

        /// <summary>
        /// Converts an Emgu.CV Mat to a Bitmap with RGB color channels.
        /// </summary>
        Bitmap MatToBitmapRgb(Mat mat);

        /// <summary>
        /// Converts a Bitmap to a byte array (JPEG format).
        /// </summary>
        byte[] BitmapToJpegBytes(Bitmap bitmap, int quality = 90);

        /// <summary>
        /// Crops a region from a Mat with padding.
        /// </summary>
        Mat CropWithPadding(Mat source, Rectangle region, int padding);
    }
}

