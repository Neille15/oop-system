using System;
using System.Drawing;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace frontend.Services
{
    /// <summary>
    /// Manages the face scanning workflow: camera capture, face detection, and event handling.
    /// This class coordinates all the services to provide a clean abstraction for face scanning operations.
    /// </summary>
    public class FaceScanningManager : IDisposable
    {
        private readonly ICameraService _cameraService;
        private readonly IFaceDetector _faceDetector;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFaceDetectionEventHandler? _eventHandler;
        private readonly IAttendanceService? _attendanceService;
        private readonly IUserDataService? _userDataService;

        private bool _isProcessing = false;
        private DateTime _lastRequestTime = DateTime.MinValue;
        private readonly TimeSpan _requestThrottleInterval = TimeSpan.FromSeconds(2);
        private string _currentMode = "time-in";

        /// <summary>
        /// Event raised when a frame is processed and ready for display.
        /// </summary>
        public event EventHandler<Bitmap>? FrameProcessed;

        /// <summary>
        /// Event raised when status changes.
        /// </summary>
        public event EventHandler<string>? StatusChanged;

        /// <summary>
        /// Event raised when loading state changes.
        /// </summary>
        public event EventHandler<string>? LoadingChanged;

        public bool IsRunning => _cameraService.IsRunning;
        public bool IsProcessing => _isProcessing;

        public FaceScanningManager(
            ICameraService cameraService,
            IFaceDetector faceDetector,
            IImageProcessor imageProcessor,
            IFaceDetectionEventHandler? eventHandler = null,
            IAttendanceService? attendanceService = null,
            IUserDataService? userDataService = null)
        {
            _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
            _faceDetector = faceDetector ?? throw new ArgumentNullException(nameof(faceDetector));
            _imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
            _eventHandler = eventHandler;
            _attendanceService = attendanceService;
            _userDataService = userDataService;

            // Wire up camera events
            _cameraService.FrameCaptured += OnFrameCaptured;
            _cameraService.StatusChanged += OnCameraStatusChanged;

            // Wire up face detector events
            _faceDetector.StatusChanged += OnFaceDetectorStatusChanged;
        }

        /// <summary>
        /// Sets the current attendance mode (time-in or time-out).
        /// </summary>
        public void SetMode(string mode)
        {
            _currentMode = mode;
        }

        /// <summary>
        /// Starts the scanning process.
        /// </summary>
        public async Task StartAsync()
        {
            if (!_faceDetector.IsReady)
            {
                await _faceDetector.InitializeAsync();
            }

            _cameraService.Start();
        }

        /// <summary>
        /// Stops the scanning process.
        /// </summary>
        public void Stop()
        {
            _cameraService.Stop();
        }

        private void OnFrameCaptured(object? sender, Mat frame)
        {
            if (_isProcessing || !_faceDetector.IsReady)
                return;

            try
            {
                // Convert to RGB for display
                using Mat rgbFrame = new Mat();
                CvInvoke.CvtColor(frame, rgbFrame, ColorConversion.Bgr2Rgb);

                // Detect faces
                Rectangle[] faces = _faceDetector.DetectFaces(frame);

                // Draw bounding boxes and process faces
                ProcessDetectedFaces(faces, frame, rgbFrame);

                // Display frame
                Bitmap displayBitmap = _imageProcessor.MatToBitmapRgb(rgbFrame);
                FrameProcessed?.Invoke(this, displayBitmap);
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error processing frame: {ex.Message}");
            }
        }

        private void ProcessDetectedFaces(Rectangle[] faces, Mat originalFrame, Mat displayFrame)
        {
            if (faces.Length == 0)
            {
                _eventHandler?.OnNoFacesDetected();
                return;
            }

            // Draw bounding boxes
            foreach (var face in faces)
            {
                // Green for single face, Red for multiple
                MCvScalar color = faces.Length == 1 
                    ? new MCvScalar(0, 255, 0) 
                    : new MCvScalar(0, 0, 255);
                CvInvoke.Rectangle(displayFrame, face, color, 2);
            }

            // Handle different face count scenarios
            if (faces.Length == 1)
            {
                if (CanSendRequest())
                {
                    ProcessSingleFace(faces[0], originalFrame);
                }
            }
            else if (faces.Length > 1)
            {
                _eventHandler?.OnMultipleFacesDetected(faces.Length);
                OnStatusChanged("Multiple faces detected. Please ensure only one person is in frame.");
            }
        }

        private bool CanSendRequest()
        {
            return !_isProcessing && DateTime.Now - _lastRequestTime >= _requestThrottleInterval;
        }

        private async void ProcessSingleFace(Rectangle face, Mat frame)
        {
            if (_isProcessing)
                return;

            _isProcessing = true;
            _lastRequestTime = DateTime.Now;
            OnLoadingChanged("Processing face...");

            try
            {
                // Crop face with padding
                using Mat croppedFace = _imageProcessor.CropWithPadding(frame, face, padding: 50);
                using Bitmap faceBitmap = _imageProcessor.MatToBitmap(croppedFace);

                // Notify event handler
                _eventHandler?.OnSingleFaceDetected(face, new Bitmap(faceBitmap));

                // Process attendance if service is available
                if (_attendanceService != null)
                {
                    await ProcessAttendanceAsync(faceBitmap);
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error processing face: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                OnLoadingChanged("");
            }
        }

        private async Task ProcessAttendanceAsync(Bitmap faceBitmap)
        {
            try
            {
                byte[] imageBytes = _imageProcessor.BitmapToJpegBytes(faceBitmap);
                string type = _currentMode.ToLower().Contains("in") ? "time-in" : "time-out";

                OnLoadingChanged("Sending to server...");
                var attendance = await _attendanceService!.RecordAttendanceAsync(imageBytes, type);

                OnStatusChanged($"Success: {type} recorded");

                // Fetch user data after successful time-in
                if (type == "time-in" && _userDataService != null && attendance.UserID > 0)
                {
                    await FetchAndDisplayUserData(attendance.UserID);
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error: {ex.Message}");
            }
        }

        private async Task FetchAndDisplayUserData(int userId)
        {
            try
            {
                var userData = await _userDataService!.GetUserDataAsync(userId);
                if (userData != null)
                {
                    string userInfo = $"User: {userData.FirstName} {userData.LastName} ({userData.StudentNumber})";
                    OnStatusChanged($"Success: time-in recorded - {userInfo}");
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the attendance recording
                System.Diagnostics.Debug.WriteLine($"Failed to fetch user data: {ex.Message}");
            }
        }

        private void OnCameraStatusChanged(object? sender, string message)
        {
            OnStatusChanged(message);
        }

        private void OnFaceDetectorStatusChanged(object? sender, string message)
        {
            OnStatusChanged(message);
        }

        private void OnStatusChanged(string message)
        {
            StatusChanged?.Invoke(this, message);
        }

        private void OnLoadingChanged(string message)
        {
            LoadingChanged?.Invoke(this, message);
        }

        public void Dispose()
        {
            _cameraService.FrameCaptured -= OnFrameCaptured;
            _cameraService.StatusChanged -= OnCameraStatusChanged;
            _faceDetector.StatusChanged -= OnFaceDetectorStatusChanged;
            _cameraService?.Dispose();
        }
    }
}

