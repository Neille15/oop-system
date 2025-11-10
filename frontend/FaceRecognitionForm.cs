using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Face;

namespace frontend
{
    public partial class FaceRecognitionForm : Form
    {
        private VideoCapture? _camera;
        private PictureBox _cameraView;
        private ComboBox _modeComboBox;
        private Label _statusLabel;
        private Label _loadingLabel;
        private Button _startButton;
        private Button _stopButton;
        private Timer _frameTimer;
        private CascadeClassifier? _faceCascade;
        private bool _isProcessing = false;
        private DateTime _lastRequestTime = DateTime.MinValue;
        private readonly TimeSpan _requestThrottleInterval = TimeSpan.FromSeconds(2); // Minimum 2 seconds between requests
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "http://localhost:5207/api/Attendances";
        private bool _isRunning = false;

        public FaceRecognitionForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            LoadFaceCascade();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Face Recognition Attendance";
            this.Size = new Size(480, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Camera view
            _cameraView = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(440, 330),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black
            };
            this.Controls.Add(_cameraView);

            // Mode selection
            var modeLabel = new Label
            {
                Text = "Mode:",
                Location = new Point(10, 350),
                Size = new Size(50, 23),
                AutoSize = false
            };
            this.Controls.Add(modeLabel);

            _modeComboBox = new ComboBox
            {
                Location = new Point(70, 350),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _modeComboBox.Items.AddRange(new[] { "Time In", "Time Out" });
            _modeComboBox.SelectedIndex = 0;
            this.Controls.Add(_modeComboBox);

            // Status label
            _statusLabel = new Label
            {
                Text = "Status: Ready",
                Location = new Point(10, 380),
                Size = new Size(440, 23),
                AutoSize = false
            };
            this.Controls.Add(_statusLabel);

            // Loading label
            _loadingLabel = new Label
            {
                Text = "",
                Location = new Point(10, 410),
                Size = new Size(440, 23),
                AutoSize = false,
                ForeColor = Color.Blue,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(_loadingLabel);

            // Start button
            _startButton = new Button
            {
                Text = "Start Camera",
                Location = new Point(10, 440),
                Size = new Size(100, 30)
            };
            _startButton.Click += StartButton_Click;
            this.Controls.Add(_startButton);

            // Stop button
            _stopButton = new Button
            {
                Text = "Stop Camera",
                Location = new Point(120, 440),
                Size = new Size(100, 30),
                Enabled = false
            };
            _stopButton.Click += StopButton_Click;
            this.Controls.Add(_stopButton);

            // Frame timer
            _frameTimer = new Timer
            {
                Interval = 33 // ~30 FPS
            };
            _frameTimer.Tick += FrameTimer_Tick;

            this.ResumeLayout(false);
        }

        private void LoadFaceCascade()
        {
            try
            {
                // Try multiple locations for the cascade file
                string[] possiblePaths = new[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml"),
                    Path.Combine(Environment.CurrentDirectory, "haarcascade_frontalface_default.xml"),
                    "haarcascade_frontalface_default.xml",
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "haarcascade_frontalface_default.xml")
                };

                string? cascadePath = null;
                foreach (var path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        cascadePath = path;
                        break;
                    }
                }

                if (cascadePath != null)
                {
                    _faceCascade = new CascadeClassifier(cascadePath);
                    UpdateStatus("Face detection ready");
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
                UpdateStatus($"Error loading face cascade: {ex.Message}");
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
                if (File.Exists(path))
                {
                    _faceCascade = new CascadeClassifier(path);
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() => UpdateStatus("Face detection ready")));
                    }
                    else
                    {
                        UpdateStatus("Face detection ready");
                    }
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateStatus($"Error loading cascade: {ex.Message}")));
                }
                else
                {
                    UpdateStatus($"Error loading cascade: {ex.Message}");
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
                _startButton.Enabled = false;
                _stopButton.Enabled = true;
                _modeComboBox.Enabled = false;
                _frameTimer.Start();
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
            _frameTimer.Stop();
            
            if (_camera != null)
            {
                _camera.Dispose();
                _camera = null;
            }

            _cameraView.Image?.Dispose();
            _cameraView.Image = null;

            _startButton.Enabled = true;
            _stopButton.Enabled = false;
            _modeComboBox.Enabled = true;
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
                        CvInvoke.Rectangle(rgbFrame, face, new MCvScalar(0, 255, 0), 2);
                    }

                    // Process the first detected face
                    if (faces.Length > 0 && CanSendRequest())
                    {
                        ProcessFace(frame, faces[0]);
                    }
                }

                // Display frame
                Bitmap bitmap = rgbFrame.ToBitmap();
                var oldImage = _cameraView.Image;
                _cameraView.Image = bitmap;
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
            catch
            {
                return Array.Empty<Rectangle>();
            }
        }

        private bool CanSendRequest()
        {
            return DateTime.Now - _lastRequestTime >= _requestThrottleInterval;
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
                // Crop face with some padding
                int padding = 20;
                int x = Math.Max(0, faceRect.X - padding);
                int y = Math.Max(0, faceRect.Y - padding);
                int width = Math.Min(frame.Width - x, faceRect.Width + padding * 2);
                int height = Math.Min(frame.Height - y, faceRect.Height + padding * 2);

                using Mat croppedFace = new Mat(frame, new Rectangle(x, y, width, height));

                // Convert to base64
                using Bitmap faceBitmap = croppedFace.ToBitmap();
                string base64Image = BitmapToBase64(faceBitmap);

                // Get selected mode
                string status = _modeComboBox.SelectedItem?.ToString() == "Time In" ? "Time In" : "Time Out";

                // Send to backend
                var request = new
                {
                    Photo = base64Image,
                    Status = status
                };

                UpdateLoading("Sending to server...");
                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    UpdateStatus($"Success: {status} recorded");
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

        private string BitmapToBase64(Bitmap bitmap)
        {
            using MemoryStream ms = new MemoryStream();
            // Use high quality JPEG encoding
            var jpegEncoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 90L); // 90% quality
            bitmap.Save(ms, jpegEncoder, encoderParams);
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
            _statusLabel.Text = $"Status: {message}";
        }

        private void UpdateLoading(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateLoading), message);
                return;
            }
            _loadingLabel.Text = message;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopButton_Click(null, EventArgs.Empty);
            _httpClient?.Dispose();
            _faceCascade?.Dispose();
            base.OnFormClosing(e);
        }
    }
}

