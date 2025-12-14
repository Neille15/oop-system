using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using frontend.Services;

namespace frontend
{
    public partial class FaceRecognitionForm : Form
    {
        private readonly FaceScanningManager _scanningManager;
        private readonly HttpClient _httpClient;
        private readonly IAttendanceService _attendanceService;
        private readonly IUserDataService _userDataService;
        private readonly ICameraService _cameraService;
        private readonly IFaceDetector _faceDetector;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFaceDetectionEventHandler _eventHandler;

        public FaceRecognitionForm()
        {
            InitializeComponent();

            // Initialize services using factory
            _httpClient = new HttpClient();
            _cameraService = ServiceFactory.CreateCameraService();
            _faceDetector = ServiceFactory.CreateFaceDetector();
            _imageProcessor = ServiceFactory.CreateImageProcessor();
            _attendanceService = ServiceFactory.CreateAttendanceService(_httpClient);
            _userDataService = ServiceFactory.CreateUserDataService(_httpClient);

            // Create event handler for face detection events
            _eventHandler = ServiceFactory.CreateFaceDetectionEventHandler(
                onFaceCaptured: DisplayCapturedFace,
                onStatusUpdate: UpdateStatus,
                onLoadingUpdate: UpdateLoading
            );

            // Create scanning manager
            _scanningManager = new FaceScanningManager(
                _cameraService,
                _faceDetector,
                _imageProcessor,
                _eventHandler,
                _attendanceService,
                _userDataService
            );

            // Wire up events
            _scanningManager.FrameProcessed += (sender, frame) => OnFrameProcessed(frame);
            _scanningManager.StatusChanged += (sender, message) => OnStatusChanged(message);
            _scanningManager.LoadingChanged += (sender, message) => OnLoadingChanged(message);

            // Initialize face detector in background
            Task.Run(async () => await _faceDetector.InitializeAsync());
        }

        // Face detector/cascade initialization has been moved to the face detector service (IFaceDetector).
        // Legacy cascade-loading logic removed in favor of the service-based implementation.


        // Cascade download handled by the face detector implementation; legacy method removed.


        // Removed: LoadCascadeFromPath (cascade handled by IFaceDetector)


        private void OnFrameProcessed(Bitmap frame)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Bitmap>(OnFrameProcessed), frame);
                return;
            }

            var oldImage = cameraView.Image;
            cameraView.Image = new Bitmap(frame);
            oldImage?.Dispose();
        }

        private void OnStatusChanged(string message)
        {
            UpdateStatus(message);
        }

        private void OnLoadingChanged(string message)
        {
            UpdateLoading(message);
        }

        private async void StartButton_Click(object? sender, EventArgs e)
        {
            try
            {
                await _scanningManager.StartAsync();
                startButton.Enabled = false;
                stopButton.Enabled = true;
                modeComboBox.Enabled = false;
                frameTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting camera: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"Error: {ex.Message}");
            }
        }

        private void StopButton_Click(object? sender, EventArgs e)
        {
            _scanningManager.Stop();
            frameTimer.Stop();

            cameraView.Image?.Dispose();
            cameraView.Image = null;

            startButton.Enabled = true;
            stopButton.Enabled = false;
            modeComboBox.Enabled = true;
            UpdateStatus("Camera stopped");
        }

        private void FrameTimer_Tick(object? sender, EventArgs e)
        {
            // FrameTimer retained for compatibility; actual frame processing is handled by FaceScanningManager.
        }

        // Face detection is handled by IFaceDetector; legacy DetectFaces removed from the form.


        // Throttling and processing logic moved to FaceScanningManager; legacy CanSendRequest removed.


        // ProcessFace moved into FaceScanningManager; legacy method removed from the form.


        // Image conversion utilities moved to IImageProcessor implementation; legacy helpers removed from the form.


        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatus), message);
                return;
            }
            statusLabel.Text = $"Status: {message}";
        }

        private void UpdateLoading(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateLoading), message);
                return;
            }
            loadingLabel.Text = message;
        }

        private void DisplayCapturedFace(Bitmap faceBitmap)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Bitmap>(DisplayCapturedFace), faceBitmap);
                return;
            }


            var oldImage = capturedFaceView.Image;
            capturedFaceView.Image = new Bitmap(faceBitmap);
            oldImage?.Dispose();
        }

        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMode = modeComboBox.SelectedItem?.ToString() ?? "Time In";
            _scanningManager.SetMode(selectedMode);
        }

        private void RegisterButton_Click(object? sender, EventArgs e)
        {
            using RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog(this);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopButton_Click(null, EventArgs.Empty);
            _scanningManager?.Dispose();
            _httpClient?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
