using System.Drawing;
using Emgu.CV;

namespace frontend.Services
{
    /// <summary>
    /// Interface for face detection services.
    /// Allows easy swapping of different face detection algorithms.
    /// </summary>
    public interface IFaceDetector
    {
        /// <summary>
        /// Detects faces in the given frame.
        /// </summary>
        /// <param name="frame">The video frame to analyze</param>
        /// <returns>Array of rectangles representing detected faces</returns>
        Rectangle[] DetectFaces(Mat frame);

        /// <summary>
        /// Checks if the face detector is ready to use.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Initializes the face detector (loads models, etc.)
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Event raised when face detection status changes (e.g., model loaded, error occurred)
        /// </summary>
        event EventHandler<string>? StatusChanged;
    }
}

