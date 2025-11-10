using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
    public class UserDatasController : ControllerBase
    {
        private readonly UserDataContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public UserDatasController(UserDataContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET: api/UserDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserData>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/UserDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserData>> GetUserData(int? id)
        {
            var userData = await _context.Users.FindAsync(id);

            if (userData == null)
            {
                return NotFound();
            }

            return userData;
        }

        // PUT: api/UserDatas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserData(int? id, UserData userData)
        {
            if (id != userData.userID)
            {
                return BadRequest();
            }

            _context.Entry(userData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDataExists(id))
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

        // POST: api/UserDatas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserData>> PostUserData(RegisterUserRequest request)
        {
            // Create UserData from request
            var userData = new UserData
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                BirthDate = request.BirthDate
            };

            // Save user to database
            _context.Users.Add(userData);
            await _context.SaveChangesAsync();

            // Get the generated userID
            var userId = userData.userID;
            if (userId == null)
            {
                return StatusCode(500, new { error = "Failed to generate user ID" });
            }

            // Call Python face recognition service to save the face
            try
            {
                var faceServiceUrl = _configuration["FaceRecognitionServiceUrl"] ?? "http://127.0.0.1:5000";
                var httpClient = _httpClientFactory.CreateClient();
                
                // Prepare the request body for Python service
                var requestBody = new
                {
                    img = request.Photo,
                    id = userId.ToString()
                };

                var response = await httpClient.PostAsJsonAsync(
                    $"{faceServiceUrl}/addFace?id={userId}",
                    requestBody
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    // Rollback user creation if face recognition fails
                    _context.Users.Remove(userData);
                    await _context.SaveChangesAsync();
                    
                    return StatusCode((int)response.StatusCode, new { 
                        error = "Failed to register face", 
                        details = errorContent 
                    });
                }

                var faceResponse = await response.Content.ReadAsStringAsync();
                // Optionally parse and log the response
            }
            catch (HttpRequestException ex)
            {
                // Rollback user creation if face recognition service is unavailable
                _context.Users.Remove(userData);
                await _context.SaveChangesAsync();
                
                return StatusCode(503, new { 
                    error = "Face recognition service unavailable", 
                    details = ex.Message 
                });
            }
            catch (Exception ex)
            {
                // Rollback user creation on any other error
                _context.Users.Remove(userData);
                await _context.SaveChangesAsync();
                
                return StatusCode(500, new { 
                    error = "An error occurred while registering face", 
                    details = ex.Message 
                });
            }

            return CreatedAtAction("GetUserData", new { id = userData.userID }, userData);
        }

        // DELETE: api/UserDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserData(int? id)
        {
            var userData = await _context.Users.FindAsync(id);
            if (userData == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserDataExists(int? id)
        {
            return _context.Users.Any(e => e.userID == id);
        }
    }
}
