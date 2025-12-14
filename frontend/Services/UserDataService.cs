using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace frontend.Services
{
    /// <summary>
    /// User data service implementation for API communication.
    /// </summary>
    public class UserDataService : IUserDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public UserDataService(HttpClient httpClient, string apiBaseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiBaseUrl = apiBaseUrl ?? throw new ArgumentNullException(nameof(apiBaseUrl));
        }

        public async Task<UserDataResponse?> GetUserDataAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{userId}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<UserDataResponse>(jsonContent, options);
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserDataResponse> RegisterUserAsync(
            byte[] faceImageBytes,
            string firstName,
            string lastName,
            string email,
            string birthDate,
            string? studentNumber = null)
        {
            using var formData = new System.Net.Http.MultipartFormDataContent();

            // Add image file
            var fileContent = new System.Net.Http.ByteArrayContent(faceImageBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            formData.Add(fileContent, "Photo", "face.jpg");

            // Add form fields
            formData.Add(new System.Net.Http.StringContent(firstName), "FirstName");
            formData.Add(new System.Net.Http.StringContent(lastName), "LastName");
            formData.Add(new System.Net.Http.StringContent(email), "Email");
            formData.Add(new System.Net.Http.StringContent(birthDate), "BirthDate");
            formData.Add(new System.Net.Http.StringContent(studentNumber ?? ""), "StudentNumber");

            var response = await _httpClient.PostAsync(_apiBaseUrl, formData);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"User registration failed: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var userData = JsonSerializer.Deserialize<UserDataResponse>(result, options);

            if (userData == null)
            {
                throw new InvalidOperationException("Failed to parse user data response");
            }

            return userData;
        }
    }
}

