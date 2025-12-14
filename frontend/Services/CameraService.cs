using System;
using System.Drawing;
using Emgu.CV;

namespace frontend.Services
{
    /// <summary>
    /// Camera service implementation using Emgu.CV VideoCapture.
    /// </summary>
    public class CameraService : ICameraService
    {
        private VideoCapture? _camera;
        private bool _isRunning = false;
        private System.Windows.Forms.Timer? _frameTimer;

        public bool IsRunning => _isRunning;
        public bool IsOpened => _camera != null && _camera.IsOpened;

        public event EventHandler<Mat>? FrameCaptured;
        public event EventHandler<string>? StatusChanged;

        public void Start()
        {
            if (_isRunning)
                return;

            try
            {
                _camera = new VideoCapture(0);
                if (!_camera.IsOpened)
                {
                    OnStatusChanged("Failed to open camera");
                    return;
                }

                _isRunning = true;

                // Start frame capture timer
                _frameTimer = new System.Windows.Forms.Timer();
                _frameTimer.Interval = 33; // ~30 FPS
                _frameTimer.Tick += FrameTimer_Tick;
                _frameTimer.Start();

                OnStatusChanged("Camera started");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error starting camera: {ex.Message}");
                _isRunning = false;
            }
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _frameTimer?.Stop();
            _frameTimer?.Dispose();
            _frameTimer = null;

            _camera?.Dispose();
            _camera = null;
            _isRunning = false;

            OnStatusChanged("Camera stopped");
        }

        public Mat? CaptureFrame()
        {
            if (!IsOpened)
                return null;

            try
            {
                Mat frame = new Mat();
                if (_camera!.Read(frame) && !frame.IsEmpty)
                {
                    return frame;
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error capturing frame: {ex.Message}");
            }

            return null;
        }

        private void FrameTimer_Tick(object? sender, EventArgs e)
        {
            var frame = CaptureFrame();
            if (frame != null)
            {
                FrameCaptured?.Invoke(this, frame);
            }
        }

        private void OnStatusChanged(string message)
        {
            StatusChanged?.Invoke(this, message);
        }

        public void Dispose()
        {
            Stop();
            _camera?.Dispose();
        }
    }
}

