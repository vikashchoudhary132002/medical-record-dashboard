using MedicalDashboardAPI.Data;
using MedicalDashboardAPI.DTOs;
using MedicalDashboardAPI.Models; // Add this line
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedicalDashboardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            ApplicationDbContext context,
            ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto registerDto)
        {
            try
            {
                // Simple validation
                if (string.IsNullOrEmpty(registerDto.Email))
                    return BadRequest("Email is required");

                if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                    return Conflict("Email already exists");

                // Create user (store password as plaintext per document's simple requirements)
                var user = new User
                {
                    FullName = registerDto.FullName,
                    Email = registerDto.Email,
                    Gender = registerDto.Gender,
                    PhoneNumber = registerDto.PhoneNumber,
                    Password = registerDto.Password // Plaintext storage (not for production)
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create session
                HttpContext.Session.SetInt32("UserId", user.Id);

                return Ok(new
                {
                    user.Id,
                    user.FullName,
                    user.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, "Registration failed");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            try
            {
                // Simple validation
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password);

                if (user == null)
                    return Unauthorized("Invalid credentials");

                // Create session
                HttpContext.Session.SetInt32("UserId", user.Id);

                return Ok(new
                {
                    user.Id,
                    user.FullName,
                    user.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, "Login failed");
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                // Check session
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null) return Unauthorized();

                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                return Ok(new
                {
                    user.Email,
                    user.Gender,
                    user.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching profile");
                return StatusCode(500, "Profile load failed");
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UserProfileUpdateDto updateDto)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null) return Unauthorized();

                var user = await _context.Users.FindAsync(userId);
                if (user == null) return NotFound();

                // Update only provided fields
                if (!string.IsNullOrEmpty(updateDto.Email))
                    user.Email = updateDto.Email;

                if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
                    user.PhoneNumber = updateDto.PhoneNumber;

                if (!string.IsNullOrEmpty(updateDto.Gender))
                    user.Gender = updateDto.Gender;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, "Update failed");
            }
        }
    }
}