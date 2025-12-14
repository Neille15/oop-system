using System.Net.Http;

namespace frontend.Services
{
    /// <summary>
    /// Factory class for creating service instances.
    /// Makes it easy to configure and create all required services.
    /// </summary>
    public static class ServiceFactory
    {
        private const string DefaultAttendanceApiUrl = "http://localhost:5207/api/Attendances/recordAttendance";
        private const string DefaultUserDataApiUrl = "http://localhost:5207/api/UserDatas";

        /// <summary>
        /// Creates a new camera service instance.
        /// </summary>
        public static ICameraService CreateCameraService()
        {
            return new CameraService();
        }

        /// <summary>
        /// Creates a new face detector instance.
        /// </summary>
        public static IFaceDetector CreateFaceDetector()
        {
            return new HaarCascadeFaceDetector();
        }

        /// <summary>
        /// Creates a new image processor instance.
        /// </summary>
        public static IImageProcessor CreateImageProcessor()
        {
            return new ImageProcessor();
        }

        /// <summary>
        /// Creates a new attendance service instance.
        /// </summary>
        public static IAttendanceService CreateAttendanceService(HttpClient httpClient, string? apiUrl = null)
        {
            return new AttendanceService(httpClient, apiUrl ?? DefaultAttendanceApiUrl);
        }

        /// <summary>
        /// Creates a new user data service instance.
        /// </summary>
        public static IUserDataService CreateUserDataService(HttpClient httpClient, string? apiUrl = null)
        {
            return new UserDataService(httpClient, apiUrl ?? DefaultUserDataApiUrl);
        }

        /// <summary>
        /// Creates a new face detection event handler instance.
        /// </summary>
        public static IFaceDetectionEventHandler CreateFaceDetectionEventHandler(
            System.Action<System.Drawing.Bitmap>? onFaceCaptured = null,
            System.Action<string>? onStatusUpdate = null,
            System.Action<string>? onLoadingUpdate = null)
        {
            return new FaceDetectionEventHandler(onFaceCaptured, onStatusUpdate, onLoadingUpdate);
        }

        /// <summary>
        /// Creates a new face scanning manager with all required services.
        /// </summary>
        public static FaceScanningManager CreateFaceScanningManager(
            HttpClient httpClient,
            string? attendanceApiUrl = null,
            string? userDataApiUrl = null,
            IFaceDetectionEventHandler? eventHandler = null)
        {
            var cameraService = CreateCameraService();
            var faceDetector = CreateFaceDetector();
            var imageProcessor = CreateImageProcessor();
            var attendanceService = CreateAttendanceService(httpClient, attendanceApiUrl);
            var userDataService = CreateUserDataService(httpClient, userDataApiUrl);

            return new FaceScanningManager(
                cameraService,
                faceDetector,
                imageProcessor,
                eventHandler,
                attendanceService,
                userDataService
            );
        }
    }
}

