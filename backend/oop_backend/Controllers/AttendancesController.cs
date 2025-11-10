using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using oop_backend.Models;

namespace oop_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendancesController : ControllerBase
    {
        private readonly AttendanceContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AttendancesController(AttendanceContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: api/Attendances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Attendance>>> GetAttendance()
        {
            return await _context.Attendance.ToListAsync();
        }

        // GET: api/Attendances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Attendance>> GetAttendance(int? id)
        {
            var attendance = await _context.Attendance.FindAsync(id);

            if (attendance == null)
            {
                return NotFound();
            }

            return attendance;
        }

        // PUT: api/Attendances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttendance(int? id, Attendance attendance)
        {
            if (id != attendance.attendanceID)
            {
                return BadRequest();
            }

            _context.Entry(attendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Attendances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Attendance>> PostAttendance(AttendanceRequest request)
        {
            // Call Python face recognition service to verify the face
            int? userId = null;
            
            try
            {
                var faceServiceUrl = _configuration["FaceRecognitionServiceUrl"] ?? "http://127.0.0.1:5000";
                var httpClient = _httpClientFactory.CreateClient();
                
                // Prepare the request body for Python service
                var requestBody = new
                {
                    img = request.Photo
                };

                var response = await httpClient.PostAsJsonAsync(
                    $"{faceServiceUrl}/verify",
                    requestBody
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { 
                        error = "Face recognition failed", 
                        details = errorContent 
                    });
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                
                // Parse the JSON response from Python service
                // Python service can return either a single object or an array
                FaceVerifyResult? bestMatch = null;
                
                try
                {
                    // Try to parse as array first (when matches are found)
                    var verifyResults = JsonSerializer.Deserialize<List<FaceVerifyResult>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (verifyResults != null && verifyResults.Count > 0)
                    {
                        bestMatch = verifyResults.FirstOrDefault();
                    }
                }
                catch (JsonException)
                {
                    // If array parsing fails, try parsing as single object (when no match)
                    try
                    {
                        bestMatch = JsonSerializer.Deserialize<FaceVerifyResult>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    catch (JsonException)
                    {
                        return BadRequest(new { 
                            error = "Invalid response format from face recognition service",
                            details = responseContent
                        });
                    }
                }

                if (bestMatch == null || !bestMatch.Verified || string.IsNullOrEmpty(bestMatch.Id))
                {
                    return BadRequest(new { 
                        error = "Face not verified or user not found",
                        verified = false,
                        reason = bestMatch?.Reason ?? "No matching face found"
                    });
                }

                // Parse userID from string to int
                if (!int.TryParse(bestMatch.Id, out int parsedUserId))
                {
                    return BadRequest(new { 
                        error = "Invalid user ID returned from face recognition service"
                    });
                }

                userId = parsedUserId;
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new { 
                    error = "Face recognition service unavailable", 
                    details = ex.Message 
                });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new { 
                    error = "Failed to parse face recognition response", 
                    details = ex.Message 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "An error occurred while verifying face", 
                    details = ex.Message 
                });
            }

            // Create attendance record
            var now = DateTime.Now;
            var attendance = new Attendance
            {
                userID = userId.Value,
                eventDate = now.ToString("yyyy-MM-dd"),
                eventTime = now.ToString("HH:mm:ss"),
                status = request.Status ?? "Present"
            };

            // Save attendance to database
            _context.Attendance.Add(attendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttendance", new { id = attendance.attendanceID }, attendance);
        }

        // Helper class for parsing Python verify response
        private class FaceVerifyResult
        {
            public bool Verified { get; set; }
            public string? Id { get; set; }
            public string? Reason { get; set; }
            public double? Distance { get; set; }
        }

        // DELETE: api/Attendances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int? id)
        {
            var attendance = await _context.Attendance.FindAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _context.Attendance.Remove(attendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AttendanceExists(int? id)
        {
            return _context.Attendance.Any(e => e.attendanceID == id);
        }
    }
}
