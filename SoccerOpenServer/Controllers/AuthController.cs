// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using SoccerOpenServer.Models.Contracts;
using SoccerOpenServer.Models.People;
using SoccerOpenServer.Models.Users;
using SoccerOpenServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SoccerOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const string HostRole = "Host";
        private const string UserRole = "User";

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

            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            var refreshToken = CreateRefreshToken();
            var refreshExpires = DateTime.UtcNow.AddDays(14);

            _db.RefreshTokens.Add(new RefreshToken
            {
                AppUserID = user.Id,
                TokenHash = Sha256(refreshToken),
                ExpiresUtc = refreshExpires
            });

            await _db.SaveChangesAsync();

            SetRefreshCookie(refreshToken, refreshExpires);

            var accessToken = GenerateJwtToken(user.Id.ToString(), user.Claims);
            return Ok(new { token = accessToken, role });
        }

        [HttpPost("registerByAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterByAdmin([FromBody] RegisterDto dto)
        {
            var username = dto.Username.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and password are required.");

            if (username.Length > 50) return BadRequest("Username is too long.");

            var serverExists = await _db.Servers.AnyAsync(s => s.ServerID == dto.ServerID);
            if (!serverExists) return BadRequest("Server does not exist.");

            var exists = await _db.AppUsers.AnyAsync(u => u.Username == username);
            if (exists) return Conflict("Username already exists.");

            _passwordHasher.CreateHash(dto.Password, out var hash, out var salt);

            Guid newId = Guid.NewGuid();

            var user = new AppUser
            {
                Id = newId,
                Username = username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Claims = new List<AppUserClaim>
                {
                    new AppUserClaim { Type = ClaimTypes.Role, Value = HostRole },
                    new AppUserClaim { Type = ClaimTypes.NameIdentifier, Value = newId.ToString() }
                }
            };

            Person person = new Person
            {
                Name = "John",
                Surname = "Doe",
                DateOfBirth = new DateOnly(1970, 1, 1),
                PlaceOfBirth = "Athens",
                ServerID = dto.ServerID
            };
            user.Person = person;

            _db.AppUsers.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not register user.");
            }

            var token = GenerateJwtToken(user.Id.ToString(), user.Claims);
            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            return Ok(new { token, role });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var username = dto.Username.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and password are required.");

            if (username.Length > 50) return BadRequest("Username is too long.");

            var exists = await _db.AppUsers.AnyAsync(u => u.Username == username);
            if (exists) return Conflict("Username already exists.");

            _passwordHasher.CreateHash(dto.Password, out var hash, out var salt);

            Guid newId = Guid.NewGuid();

            var user = new AppUser
            {
                Id = newId,
                Username = username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Claims = new List<AppUserClaim>
                {
                    new AppUserClaim { Type = ClaimTypes.Role, Value = UserRole },
                    new AppUserClaim { Type = ClaimTypes.NameIdentifier, Value = newId.ToString() }
                }
            };

            int randomWeight = Random.Shared.Next(75, 95);
            int randomHeight = Random.Shared.Next(175, 195);

            Person person = new Person
            {
                Name = "John",
                Surname = "Doe",
                DateOfBirth = new DateOnly(1970, 1, 1),
                PlaceOfBirth = "Athens",
                ServerID = dto.ServerID,
                StaffRole = StaffRole.Manager,
                Weight = randomWeight,
                Height = randomHeight
            };

            user.Person = person;
            var now = DateTime.UtcNow;

            var availableTeams = await _db.Teams
                .Where(t => !_db.People.Any(p => p.StaffRole == StaffRole.Manager && p.Contracts.Any(c => c.TeamID == t.TeamID && (c.EndDate == null || c.EndDate > DateOnly.FromDateTime(now)))))
                .ToListAsync();

            // If there's at least one available team, assign a random one
            if (availableTeams.Any())
            {
                var randomTeam = availableTeams[Random.Shared.Next(availableTeams.Count)];

                var contract = new Contract
                {
                    Person = person,
                    Team = randomTeam,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))
                };

                _db.Contracts.Add(contract);

                randomTeam.AppUserID = user.Id;
                _db.Teams.Update(randomTeam);
            }

            _db.AppUsers.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Could not register user.");
            }

            var token = GenerateJwtToken(user.Id.ToString(), user.Claims);
            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            return Ok(new { token, role });
        }

        private string GenerateJwtToken(string Id, IEnumerable<AppUserClaim> dbClaims)
        {
            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing key (Jwt:Key) is missing.");

            var keyBytes = Convert.FromBase64String(keyString);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, Id),
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

        private void SetRefreshCookie(string refreshToken, DateTime expiresUtc)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,          // true in https
                SameSite = SameSiteMode.None, // needed for Angular on different origin
                Expires = expiresUtc
            });
        }

        private static string CreateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private static string Sha256(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized("Missing refresh token.");

            var tokenHash = Sha256(refreshToken);

            var stored = await _db.RefreshTokens
                .Include(rt => rt.AppUser)
                .SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash);

            if (stored == null || !stored.IsActive)
                return Unauthorized("Invalid refresh token.");

            var user = await _db.AppUsers
                .Include(u => u.Claims)
                .SingleAsync(u => u.Id == stored.AppUserID);

            var newRefresh = CreateRefreshToken();
            var newRefreshHash = Sha256(newRefresh);
            var newExpires = DateTime.UtcNow.AddDays(14);

            stored.RevokedUtc = DateTime.UtcNow;
            stored.ReplacedByTokenHash = newRefreshHash;

            _db.RefreshTokens.Add(new RefreshToken
            {
                AppUserID = user.Id,
                TokenHash = newRefreshHash,
                ExpiresUtc = newExpires
            });

            await _db.SaveChangesAsync();

            SetRefreshCookie(newRefresh, newExpires);

            var accessToken = GenerateJwtToken(user.Id.ToString(), user.Claims);
            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";

            return Ok(new { token = accessToken, role });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var tokenHash = Sha256(refreshToken);
                var stored = await _db.RefreshTokens.SingleOrDefaultAsync(rt => rt.TokenHash == tokenHash);
                if (stored != null && stored.IsActive)
                {
                    stored.RevokedUtc = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
            }

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None,
                HttpOnly = true
            });

            return Ok();
        }

    }

    public record LoginDto(string Username, string Password);

    public record RegisterDto(string Username, string Password, Guid ServerID);
}
