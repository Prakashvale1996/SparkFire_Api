using CrackersStore.API.Data;
using CrackersStore.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace CrackersStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterDto request)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email already registered" });
            }

            // Hash password
            string passwordHash = HashPassword(request.Password);

            // Create new user
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Phone = request.Phone,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            string token = GenerateToken(user);

            return Ok(new AuthResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Token = token
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginDto request)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            // Verify password
            if (!VerifyPassword(request.Password, user.PasswordHash))
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            // Generate token
            string token = GenerateToken(user);

            return Ok(new AuthResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Token = token
            });
        }

        [HttpPost("admin/login")]
        public async Task<ActionResult<AuthResponse>> AdminLogin(LoginDto request)
        {
            // Find admin user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == "Admin");

            if (user == null)
            {
                return BadRequest(new { message = "Invalid admin credentials" });
            }

            // Verify password
            if (!VerifyPassword(request.Password, user.PasswordHash))
            {
                return BadRequest(new { message = "Invalid admin credentials" });
            }

            // Generate token
            string token = GenerateToken(user);

            return Ok(new AuthResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                Token = token
            });
        }

        [HttpPost("create-admin")]
        public async Task<ActionResult<AuthResponse>> CreateAdmin([FromBody] RegisterDto request)
        {
            // Check if admin already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Email already registered" });
            }

            // Hash password
            string passwordHash = HashPassword(request.Password);

            // Create admin user
            var admin = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Phone = request.Phone,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(admin);
            await _context.SaveChangesAsync();

            // Generate token
            string token = GenerateToken(admin);

            return Ok(new AuthResponse
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email,
                Phone = admin.Phone,
                Role = admin.Role,
                Token = token
            });
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value ?? "SuperSecretKeyForJWTTokenGeneration12345678"));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
