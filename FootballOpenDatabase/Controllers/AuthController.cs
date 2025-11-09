using FootballOpenDatabase.Models.People;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FootballOpenDatabase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config) => _config = config;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            if (login.Username == "admin" && login.Password == "password")
            {
                string role = "Admin";
                var token = GenerateJwtToken(login.Username, role);
                return Ok(new { token, role });
            }
            else if (login.Username == "host" && login.Password == "password")
            {
                string role = "Host";
                var token = GenerateJwtToken(login.Username, role);
                return Ok(new { token, role });
            }
            else if (login.Username == "tom" && login.Password == "password")
            {
                string role = "User";
                var token = GenerateJwtToken(login.Username, role);
                return Ok(new { token, role });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role)
            };

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
}
