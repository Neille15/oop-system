using System.Threading.Tasks;

namespace frontend.Services
{
    /// <summary>
    /// Interface for attendance-related API operations.
    /// </summary>
    public interface IAttendanceService
    {
        /// <summary>
        /// Records attendance (time-in or time-out) with the provided face image.
        /// </summary>
        /// <param name="faceImageBytes">The face image as byte array</param>
        /// <param name="type">The attendance type ("time-in" or "time-out")</param>
        /// <returns>Attendance response containing userID and attendance details</returns>
        Task<AttendanceResponse> RecordAttendanceAsync(byte[] faceImageBytes, string type);
    }

    /// <summary>
    /// Response model for attendance recording.
    /// </summary>
    public class AttendanceResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("userID")]
        public int UserID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("attendanceID")]
        public int? AttendanceID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string? Status { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("eventDate")]
        public string? EventDate { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("eventTime")]
        public string? EventTime { get; set; }
    }
}

