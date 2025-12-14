using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace frontend.Services
{
    /// <summary>
    /// Haar Cascade-based face detector implementation.
    /// </summary>
    public class HaarCascadeFaceDetector : IFaceDetector
    {
        private CascadeClassifier? _faceCascade;
        private bool _isReady = false;

        public bool IsReady => _isReady && _faceCascade != null;

        public event EventHandler<string>? StatusChanged;

        public async Task InitializeAsync()
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
                    await DownloadCascadeFileAsync();
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error loading face detector: {ex.Message}");
            }
        }

        private void LoadCascadeFromPath(string path)
        {
            try
            {
                _faceCascade = new CascadeClassifier(path);
                _isReady = true;
                OnStatusChanged("Face detector ready");
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error loading cascade from {path}: {ex.Message}");
            }
        }

        private async Task DownloadCascadeFileAsync()
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

                OnStatusChanged("Downloading face detection model...");

                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);
                var response = await httpClient.GetAsync(cascadeUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(cascadePath, content);
                    LoadCascadeFromPath(cascadePath);
                    OnStatusChanged("Face detection model downloaded and loaded");
                }
                else
                {
                    OnStatusChanged("Failed to download face detection model");
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error downloading cascade file: {ex.Message}");
            }
        }

        public Rectangle[] DetectFaces(Mat frame)
        {
            if (!IsReady || frame.IsEmpty)
                return Array.Empty<Rectangle>();

            try
            {
                using Mat gray = new Mat();
                CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
                CvInvoke.EqualizeHist(gray, gray);

                Rectangle[] faces = _faceCascade!.DetectMultiScale(
                    gray,
                    minNeighbors: 5,
                    scaleFactor: 1.1,
                    minSize: new System.Drawing.Size(30, 30)
                );

                return faces;
            }
            catch (Exception ex)
            {
                OnStatusChanged($"Error detecting faces: {ex.Message}");
                return Array.Empty<Rectangle>();
            }
        }

        private void OnStatusChanged(string message)
        {
            StatusChanged?.Invoke(this, message);
        }
    }
}

