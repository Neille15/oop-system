using System;
using System.Drawing;

namespace frontend.Services
{
    /// <summary>
    /// Interface for handling face detection events.
    /// Allows decoupling of face detection logic from UI handling.
    /// </summary>
    public interface IFaceDetectionEventHandler
    {
        /// <summary>
        /// Called when faces are detected in a frame.
        /// </summary>
        /// <param name="faces">Array of detected face rectangles</param>
        /// <param name="frame">The original frame</param>
        void OnFacesDetected(Rectangle[] faces, System.Drawing.Bitmap frame);

        /// <summary>
        /// Called when exactly one face is detected and should be processed.
        /// </summary>
        /// <param name="face">The detected face rectangle</param>
        /// <param name="faceImage">The cropped face image</param>
        void OnSingleFaceDetected(Rectangle face, System.Drawing.Bitmap faceImage);

        /// <summary>
        /// Called when multiple faces are detected.
        /// </summary>
        /// <param name="faceCount">Number of faces detected</param>
        void OnMultipleFacesDetected(int faceCount);

        /// <summary>
        /// Called when no faces are detected.
        /// </summary>
        void OnNoFacesDetected();
    }
}

