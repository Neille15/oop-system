using System;
using System.Drawing;
using Emgu.CV;

namespace frontend.Services
{
    /// <summary>
    /// Interface for camera services.
    /// Abstracts camera operations to allow different camera sources.
    /// </summary>
    public interface ICameraService : IDisposable
    {
        /// <summary>
        /// Checks if the camera is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Checks if the camera is opened and ready.
        /// </summary>
        bool IsOpened { get; }

        /// <summary>
        /// Starts the camera.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the camera.
        /// </summary>
        void Stop();

        /// <summary>
        /// Captures the current frame from the camera.
        /// </summary>
        /// <returns>The captured frame, or null if no frame is available</returns>
        Mat? CaptureFrame();

        /// <summary>
        /// Event raised when a new frame is available.
        /// </summary>
        event EventHandler<Mat>? FrameCaptured;

        /// <summary>
        /// Event raised when camera status changes.
        /// </summary>
        event EventHandler<string>? StatusChanged;
    }
}

