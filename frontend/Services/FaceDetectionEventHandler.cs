using System;
using System.Drawing;
using System.Windows.Forms;

namespace frontend.Services
{
    /// <summary>
    /// Default implementation of IFaceDetectionEventHandler.
    /// Handles face detection events and updates UI accordingly.
    /// </summary>
    public class FaceDetectionEventHandler : IFaceDetectionEventHandler
    {
        private readonly Action<Bitmap>? _onFaceCaptured;
        private readonly Action<string>? _onStatusUpdate;
        private readonly Action<string>? _onLoadingUpdate;

        public FaceDetectionEventHandler(
            Action<Bitmap>? onFaceCaptured = null,
            Action<string>? onStatusUpdate = null,
            Action<string>? onLoadingUpdate = null)
        {
            _onFaceCaptured = onFaceCaptured;
            _onStatusUpdate = onStatusUpdate;
            _onLoadingUpdate = onLoadingUpdate;
        }

        public void OnFacesDetected(Rectangle[] faces, Bitmap frame)
        {
            // Default implementation - can be overridden by derived classes
        }

        public void OnSingleFaceDetected(Rectangle face, Bitmap faceImage)
        {
            // Display the captured face
            _onFaceCaptured?.Invoke(faceImage);
        }

        public void OnMultipleFacesDetected(int faceCount)
        {
            _onStatusUpdate?.Invoke($"Multiple faces detected ({faceCount}). Please ensure only one person is in frame.");
        }

        public void OnNoFacesDetected()
        {
            // Default implementation - can be overridden
        }
    }
}

