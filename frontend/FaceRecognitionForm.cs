using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using Emgu.CV.Util;

namespace frontend
{
    public partial class FaceRecognitionForm : Form
    {
        private VideoCapture? _camera;
        private CascadeClassifier? _faceCascade;
        private bool _isProcessing = false;
        private DateTime _lastRequestTime = DateTime.MinValue;
        private readonly TimeSpan _requestThrottleInterval = TimeSpan.FromSeconds(2); // Minimum 2 seconds between requests
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "http://localhost:5207/api/Attendances/recordAttendance";
        private bool _isRunning = false;

        public FaceRecognitionForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            LoadFaceCascade();
        }

        private void LoadFaceCascade()
        {
            try
            {
                // Try multiple locations for the Haar Cascade file
                string[] possiblePaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml"),
                    Path.Combine(Environment.CurrentDirectory, "haarcascade_frontalface_default.xml"),
                    "haarcascade_frontalface_default.xml"
                };

                string? cascadePath = null;
                foreach (var path in possiblePaths)
                {
                    string fullPath = Path.IsPathRooted(path) ? path : Path.GetFullPath(path);
                    if (File.Exists(fullPath))
                    {
                        cascadePath = fullPath;
                        break;
                    }
                }

                if (cascadePath != null)
                {
                    LoadCascadeFromPath(cascadePath);
                }
                else
                {
                    // Try to download the cascade file
                    Task.Run(async () => await DownloadCascadeFile());
                    UpdateStatus("Downloading face detection model...");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error loading face detector: {ex.Message}");
            }
        }

        private async Task DownloadCascadeFile()
        {
            try
            {
                string cascadeUrl = "https://raw.githubusercontent.com/opencv/opencv/master/data/haarcascades/haarcascade_frontalface_default.xml";
                string cascadePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml");

                if (File.Exists(cascadePath))
                {
                    LoadCascadeFromPath(cascadePath);
                    return;
                }

                UpdateStatus("Downloading face detection model...");

                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                var response = await httpClient.GetAsync(cascadeUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(cascadePath, content);
                    LoadCascadeFromPath(cascadePath);
                    UpdateStatus("Face detection ready");
                }
                else
                {
                    UpdateStatus("Warning: Could not download face cascade. Face detection may not work.");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Warning: Could not load face cascade: {ex.Message}");
            }
        }

        private void LoadCascadeFromPath(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    UpdateStatus($"Cascade file not found: {path}");
                    return;
                }

                UpdateStatus("Loading face detection model...");
                _faceCascade = new CascadeClassifier(path);

                if (_faceCascade == null)
                {
                    UpdateStatus("Failed to load cascade classifier");
                    return;
                }

                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateStatus("Face detection ready")));
                }
                else
                {
                    UpdateStatus("Face detection ready");
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"Error loading cascade from {path}: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMsg += $" (Inner: {ex.InnerException.Message})";
                }

                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateStatus(errorMsg)));
                }
                else
                {
                    UpdateStatus(errorMsg);
                }
            }
        }

        private void StartButton_Click(object? sender, EventArgs e)
        {
            try
            {
                _camera = new VideoCapture(0); // Use default camera
                if (!_camera.IsOpened)
                {
                    MessageBox.Show("Failed to open camera. Please check if a camera is connected.", "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _isRunning = true;
                startButton.Enabled = false;
                stopButton.Enabled = true;
                modeComboBox.Enabled = false;
                frameTimer.Start();
                UpdateStatus("Camera started");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting camera: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"Error: {ex.Message}");
            }
        }

        private void StopButton_Click(object? sender, EventArgs e)
        {
            _isRunning = false;
            frameTimer.Stop();

            if (_camera != null)
            {
                _camera.Dispose();
                _camera = null;
            }

            cameraView.Image?.Dispose();
            cameraView.Image = null;

            startButton.Enabled = true;
            stopButton.Enabled = false;
            modeComboBox.Enabled = true;
            UpdateStatus("Camera stopped");
        }

        private void FrameTimer_Tick(object? sender, EventArgs e)
        {
            if (!_isRunning || _camera == null || !_camera.IsOpened || _isProcessing)
                return;

            try
            {
                using Mat frame = new Mat();
                if (!_camera.Read(frame) || frame.IsEmpty)
                    return;

                // Convert to RGB for display
                using Mat rgbFrame = new Mat();
                CvInvoke.CvtColor(frame, rgbFrame, ColorConversion.Bgr2Rgb);

                // Detect faces
                Rectangle[] faces = DetectFaces(frame);

                // Draw bounding boxes
                if (faces.Length > 0)
                {
                    foreach (var face in faces)
                    {
                        // Use different colors based on face count
                        MCvScalar color = faces.Length == 1 ? new MCvScalar(0, 255, 0) : new MCvScalar(0, 0, 255); // Green for single face, Red for multiple
                        CvInvoke.Rectangle(rgbFrame, face, color, 2);
                    }

                    // Only process when exactly one face is detected
                    if (faces.Length == 1 && CanSendRequest())
                    {
                        ProcessFace(frame, faces[0]);
                    }
                    else if (faces.Length > 1)
                    {
                        // Show message when multiple faces detected
                        UpdateStatus("Multiple faces detected. Please ensure only one person is in frame.");
                    }
                }

                // Display frame (RGB format)
                Bitmap bitmap = MatToBitmapRgb(rgbFrame);
                var oldImage = cameraView.Image;
                cameraView.Image = bitmap;
                oldImage?.Dispose();
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error processing frame: {ex.Message}");
            }
        }

        private Rectangle[] DetectFaces(Mat frame)
        {
            if (_faceCascade == null)
                return Array.Empty<Rectangle>();

            try
            {
                using Mat gray = new Mat();
                CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
                CvInvoke.EqualizeHist(gray, gray);

                Rectangle[] faces = _faceCascade.DetectMultiScale(
                    gray,
                    1.1,
                    3,
                    new Size(30, 30),
                    Size.Empty
                );

                return faces;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Face detection error: {ex.Message}");
                return Array.Empty<Rectangle>();
            }
        }

        private bool CanSendRequest()
        {
            // Don't send if already processing or if throttled
            return !_isProcessing && DateTime.Now - _lastRequestTime >= _requestThrottleInterval;
        }

        private async void ProcessFace(Mat frame, Rectangle faceRect)
        {
            if (_isProcessing)
                return;

            _isProcessing = true;
            _lastRequestTime = DateTime.Now;
            UpdateLoading("Processing face...");

            try
            {
                int padding = 50;
                int x = Math.Max(0, faceRect.X - padding);
                int y = Math.Max(0, faceRect.Y - padding);
                int width = Math.Min(frame.Width - x, faceRect.Width + padding * 2);
                int height = Math.Min(frame.Height - y, faceRect.Height + padding * 2);

                using Mat croppedFace = new Mat(frame, new Rectangle(x, y, width, height));

                // Convert to bitmap
                using Bitmap faceBitmap = MatToBitmap(croppedFace);

                // Display the cropped face image (create a copy for display)
                DisplayCapturedFace(new Bitmap(faceBitmap));

                // Get selected mode and convert to backend format
                string type = modeComboBox.SelectedItem?.ToString() == "Time In" ? "time-in" : "time-out";

                // Prepare multipart/form-data
                using var formData = new MultipartFormDataContent();

                // Convert bitmap to byte array
                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    var jpegEncoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                    if (jpegEncoder != null)
                    {
                        var encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)90);
                        faceBitmap.Save(ms, jpegEncoder, encoderParams);
                    }
                    else
                    {
                        faceBitmap.Save(ms, ImageFormat.Jpeg);
                    }
                    imageBytes = ms.ToArray();
                }

                // Add image file
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                formData.Add(imageContent, "Photo", "face.jpg");

                // Add type field
                formData.Add(new StringContent(type), "Type");

                UpdateLoading("Sending to server...");
                var response = await _httpClient.PostAsync(_apiBaseUrl, formData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    UpdateStatus($"Success: {type} recorded");
                    UpdateLoading("");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    UpdateStatus($"Error: {response.StatusCode} - {errorContent}");
                    UpdateLoading("");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error: {ex.Message}");
                UpdateLoading("");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private Bitmap MatToBitmap(Mat mat)
        {
            // For BGR format (original camera frame)
            using Image<Bgr, byte> image = mat.ToImage<Bgr, byte>();
            return image.AsBitmap();
        }

        private Bitmap MatToBitmapRgb(Mat mat)
        {
            // For RGB format (converted for display)
            using Image<Rgb, byte> image = mat.ToImage<Rgb, byte>();
            return image.AsBitmap();
        }

        private string BitmapToBase64(Bitmap bitmap)
        {
            using MemoryStream ms = new MemoryStream();
            // Use high quality JPEG encoding
            var jpegEncoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            if (jpegEncoder != null)
            {
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)90); // 90% quality
                bitmap.Save(ms, jpegEncoder, encoderParams);
            }
            else
            {
                bitmap.Save(ms, ImageFormat.Jpeg);
            }
            byte[] imageBytes = ms.ToArray();
            return Convert.ToBase64String(imageBytes);
        }

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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopButton_Click(null, EventArgs.Empty);
            _httpClient?.Dispose();
            _faceCascade?.Dispose();
            base.OnFormClosing(e);
        }

        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void RegisterButton_Click(object? sender, EventArgs e)
        {
            using RegisterForm registerForm = new RegisterForm();
            registerForm.ShowDialog(this);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}

