using FootballOpenServer.Models.Users;
using FootballOpenServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FootballOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly FootballDbContext _db;
        public AuthController(
            IConfiguration config,
            IPasswordHasherService passwordHasher,
            FootballDbContext db
            )
        {
            _config = config;
            _passwordHasher = passwordHasher;
            _db = db;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var username = dto.Username.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and password are required.");

            var user = await _db.AppUsers
                .Include(u => u.Claims)
                .SingleOrDefaultAsync(u => u.Username == username);

            // Don't reveal whether username exists
            if (user == null) return Unauthorized("Invalid credentials.");

            if (!_passwordHasher.Verify(dto.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid credentials.");

            var token = GenerateJwtToken(user.Username, user.Claims);
            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            return Ok(new { token, role });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var username = dto.Username.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and password are required.");

            if (username.Length > 50) return BadRequest("Username is too long.");

            var exists = await _db.AppUsers.AnyAsync(u => u.Username == username);
            if (exists) return Conflict("Username already exists.");

            _passwordHasher.CreateHash(dto.Password, out var hash, out var salt);

            var user = new AppUser
            {
                Username = username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Claims = new List<AppUserClaim>
                {
                    new AppUserClaim { Type = ClaimTypes.Role, Value = dto.Role },
                    new AppUserClaim { Type = ClaimTypes.NameIdentifier, Value = username }
                }
            };

            _db.AppUsers.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not register user.");
            }

            var token = GenerateJwtToken(user.Username, user.Claims);
            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            return Ok(new { token, role });
            //return CreatedAtAction(nameof(Me), new { }, new { token, role });
        }

        private string GenerateJwtToken(string username, IEnumerable<AppUserClaim> dbClaims)
        {
            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing key (Jwt:Key) is missing.");

            var keyBytes = Convert.FromBase64String(keyString);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Add claims stored in DB (Role, etc.)
            claims.AddRange(dbClaims.Select(c => new Claim(c.Type, c.Value)));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public record LoginDto(string Username, string Password);

    public record RegisterDto(string Username, string Password, string Role);
}
