using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace frontend.Services
{
    /// <summary>
    /// Attendance service implementation for API communication.
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AttendanceService(HttpClient httpClient, string apiBaseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiBaseUrl = apiBaseUrl ?? throw new ArgumentNullException(nameof(apiBaseUrl));
        }

        public async Task<AttendanceResponse> RecordAttendanceAsync(byte[] faceImageBytes, string type)
        {
            using var formData = new MultipartFormDataContent();

            // Add image file
            var fileContent = new ByteArrayContent(faceImageBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            formData.Add(fileContent, "Photo", "face.jpg");

            // Add type
            formData.Add(new StringContent(type), "Type");

            var response = await _httpClient.PostAsync(_apiBaseUrl, formData);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Attendance recording failed: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var attendance = JsonSerializer.Deserialize<AttendanceResponse>(result, options);

            if (attendance == null)
            {
                throw new InvalidOperationException("Failed to parse attendance response");
            }

            return attendance;
        }
    }
}

