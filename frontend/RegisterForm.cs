using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using frontend.Services;

namespace frontend
{
    public partial class RegisterForm : Form
    {
        private readonly ICameraService _cameraService;
        private readonly IFaceDetector _faceDetector;
        private readonly IImageProcessor _imageProcessor;
        private readonly IUserDataService _userDataService;
        private readonly HttpClient _httpClient;
        private bool _isProcessing = false;

        public RegisterForm()
        {
            InitializeComponent();

            // Initialize services
            _httpClient = new HttpClient();
            _cameraService = ServiceFactory.CreateCameraService();
            _faceDetector = ServiceFactory.CreateFaceDetector();
            _imageProcessor = ServiceFactory.CreateImageProcessor();
            _userDataService = ServiceFactory.CreateUserDataService(_httpClient);

            // Wire up camera events
            _cameraService.FrameCaptured += OnFrameCaptured;
            _cameraService.StatusChanged += OnCameraStatusChanged;
            _faceDetector.StatusChanged += OnFaceDetectorStatusChanged;

            // Initialize face detector
            frameTimer.Interval = 33;
            frameTimer.Tick += FrameTimer_Tick;
            Task.Run(async () => await _faceDetector.InitializeAsync());
        }

        private void RegisterForm_Load(object? sender, EventArgs e)
        {
            StartCamera();
        }

        private void StartCamera()
        {
            try
            {
                _cameraService.Start();
                if (!_cameraService.IsOpened)
                {
                    MessageBox.Show("Failed to open camera.", "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                frameTimer.Start();
                statusLabel.Text = "Camera started. Position your face in the frame.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting camera: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrameTimer_Tick(object? sender, EventArgs e)
        {
            // Frame processing is handled by camera service events
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

                // Draw bounding boxes
                if (faces.Length > 0)
                {
                    foreach (var face in faces)
                    {
                        CvInvoke.Rectangle(rgbFrame, face, new Emgu.CV.Structure.MCvScalar(0, 255, 0), 2);
                    }
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => captureButton.Enabled = true));
                    }
                    else
                    {
                        captureButton.Enabled = true;
                    }
                }
                else
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => captureButton.Enabled = false));
                    }
                    else
                    {
                        captureButton.Enabled = false;
                    }
                }

                // Display frame
                Bitmap bitmap = _imageProcessor.MatToBitmapRgb(rgbFrame);
                if (InvokeRequired)
                {
                    Invoke(new Action<Bitmap>(UpdateCameraView), bitmap);
                }
                else
                {
                    UpdateCameraView(bitmap);
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<string>(UpdateStatus), $"Error processing frame: {ex.Message}");
                }
                else
                {
                    UpdateStatus($"Error processing frame: {ex.Message}");
                }
            }
        }

        private void UpdateCameraView(Bitmap bitmap)
        {
            var oldImage = cameraView.Image;
            cameraView.Image = new Bitmap(bitmap);
            oldImage?.Dispose();
        }

        private void OnCameraStatusChanged(object? sender, string message)
        {
            UpdateStatus(message);
        }

        private void OnFaceDetectorStatusChanged(object? sender, string message)
        {
            UpdateStatus(message);
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatus), message);
                return;
            }
            statusLabel.Text = message;
        }

        private async void CaptureButton_Click(object? sender, EventArgs e)
        {
            if (_isProcessing || !_cameraService.IsOpened)
                return;

            // Validate form fields
            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                MessageBox.Show("Please enter your first name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                MessageBox.Show("Please enter your last name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(emailTextBox.Text) || !emailTextBox.Text.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (birthDatePicker.Value > DateTime.Now.AddYears(-10))
            {
                MessageBox.Show("Please enter a valid birth date.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isProcessing = true;
            captureButton.Enabled = false;
            statusLabel.Text = "Capturing face...";

            try
            {
                // Capture current frame
                Mat? frame = _cameraService.CaptureFrame();
                if (frame == null || frame.IsEmpty)
                {
                    statusLabel.Text = "Failed to capture frame.";
                    _isProcessing = false;
                    captureButton.Enabled = true;
                    return;
                }

                // Detect face and crop
                Rectangle[] faces = _faceDetector.DetectFaces(frame);
                if (faces.Length == 0)
                {
                    MessageBox.Show("No face detected. Please position your face in the camera view.", "No Face Detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    statusLabel.Text = "No face detected. Please try again.";
                    _isProcessing = false;
                    captureButton.Enabled = true;
                    return;
                }

                // Crop the first detected face
                Rectangle faceRect = faces[0];
                using Mat croppedFace = _imageProcessor.CropWithPadding(frame, faceRect, padding: 50);
                using Bitmap faceBitmap = _imageProcessor.MatToBitmap(croppedFace);

                // Convert to bytes
                byte[] imageBytes = _imageProcessor.BitmapToJpegBytes(faceBitmap);

                // Register user
                statusLabel.Text = "Registering user...";
                var userData = await _userDataService.RegisterUserAsync(
                    imageBytes,
                    firstNameTextBox.Text.Trim(),
                    lastNameTextBox.Text.Trim(),
                    emailTextBox.Text.Trim(),
                    birthDatePicker.Value.ToString("yyyy-MM-dd"),
                    studentNumberTextBox.Text.Trim()
                );

                MessageBox.Show("User registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = "Registration successful!";

                // Clear form
                firstNameTextBox.Clear();
                lastNameTextBox.Clear();
                emailTextBox.Clear();
                birthDatePicker.Value = DateTime.Now.AddYears(-25);
                studentNumberTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during registration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = $"Error: {ex.Message}";
            }
            finally
            {
                _isProcessing = false;
                captureButton.Enabled = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            frameTimer?.Stop();
            _cameraService?.Dispose();
            _httpClient?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
