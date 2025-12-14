using System.Threading.Tasks;

namespace frontend.Services
{
    /// <summary>
    /// Interface for user data-related API operations.
    /// </summary>
    public interface IUserDataService
    {
        /// <summary>
        /// Fetches user data by user ID.
        /// </summary>
        /// <param name="userId">The user ID to fetch</param>
        /// <returns>User data response, or null if not found</returns>
        Task<UserDataResponse?> GetUserDataAsync(int userId);

        /// <summary>
        /// Registers a new user with face image and user information.
        /// </summary>
        /// <param name="faceImageBytes">The face image as byte array</param>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <param name="email">User's email</param>
        /// <param name="birthDate">User's birth date (yyyy-MM-dd format)</param>
        /// <param name="studentNumber">User's student number (optional)</param>
        /// <returns>User data response with the created user</returns>
        Task<UserDataResponse> RegisterUserAsync(
            byte[] faceImageBytes,
            string firstName,
            string lastName,
            string email,
            string birthDate,
            string? studentNumber = null);
    }

    /// <summary>
    /// Response model for user data operations.
    /// </summary>
    public class UserDataResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("userID")]
        public int? UserID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string? Email { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("studentNumber")]
        public string? StudentNumber { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("birthDate")]
        public string? BirthDate { get; set; }
    }
}

