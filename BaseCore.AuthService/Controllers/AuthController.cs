using Microsoft.AspNetCore.Mvc;
using BaseCore.Common;
using BaseCore.Services.Authen;
using BaseCore.Entities;
using System;
using System.Threading.Tasks;

namespace BaseCore.AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        private const int TokenExpirationMinutes = 480; // 8 hours

        public AuthController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request" });

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Username and password are required" });

            var user = await _userService.Authenticate(request.Username, request.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            var token = TokenHelper.GenerateToken(
                _config["Jwt:SecretKey"],
                TokenExpirationMinutes,
                user.Id.ToString(),
                user.UserName,
                user.UserType == 1 ? "Admin" : "User"
            );

            return Ok(new
            {
                token = token,
                user = new
                {
                    id = user.Id,
                    name = user.Name,
                    username = user.UserName,
                    email = user.Email,
                    role = user.UserType == 1 ? "Admin" : "User"
                }
            });
        }

        // ================= REGISTER =================
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request" });

            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Username and password are required" });

            if (request.Password.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 characters" });

            try
            {
                var user = new User
                {
                    UserName = request.Username,
                    Name = request.Name ?? request.Username,
                    Email = request.Email,
                    Phone = string.IsNullOrWhiteSpace(request.Phone) ? "0000000000" : request.Phone,

                    Contact = "",
                    Position = "",
                    Image = "",

                    IsActive = true,
                    UserType = 0,
                    Created = DateTime.Now
                };

                var createdUser = await _userService.Create(user, request.Password);

                return Ok(new
                {
                    message = "Registration successful",
                    userId = createdUser.Id
                });
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;

                return BadRequest(new
                {
                    message = "Registration failed: " + msg
                });
            }
        }
    }

    // ================= DTO =================
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public int ExpiresIn { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
    }
}