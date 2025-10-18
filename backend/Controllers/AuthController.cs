using backend.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly IConfiguration _config;

        public AuthController(IMongoClient mongoClient, IConfiguration config)
        {
            var database = mongoClient.GetDatabase("DicoDb");
            _users = database.GetCollection<User>("Users");
            _config = config;
        }

    [HttpPost("register")]
    [AllowAnonymous]
        public IActionResult Register([FromBody] UserRegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email et mot de passe requis.");

            if (_users.Find(u => u.Email == dto.Email).Any())
                return BadRequest("Email déjà utilisé.");

            var user = new User
            {
                Email = dto.Email ?? string.Empty,
                PasswordHash = HashPassword(dto.Password ?? string.Empty),
                LanguagePairs = new List<string> { "fr-en" },
                Stats = new UserStats()
            };
            _users.InsertOne(user);
            return Ok();
        }

    [HttpPost("login")]
    [AllowAnonymous]
        public IActionResult Login([FromBody] UserLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email et mot de passe requis.");

            var user = _users.Find(u => u.Email == dto.Email).FirstOrDefault();
            if (user == null || !VerifyPassword(dto.Password ?? string.Empty, user.PasswordHash))
                return Unauthorized();

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private static string HashPassword(string password)
        {
            // Use BCrypt for secure password hashing
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            // Use BCrypt for secure password verification
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _config["Jwt:Key"] ?? string.Empty;
            var jwtIssuer = _config["Jwt:Issuer"] ?? string.Empty;
            var jwtAudience = _config["Jwt:Audience"] ?? string.Empty;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UserRegisterDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class UserLoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
