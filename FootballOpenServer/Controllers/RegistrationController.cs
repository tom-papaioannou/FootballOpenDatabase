// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.DTO.Registration;
using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Users;
using FootballOpenServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FootballOpenServer.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private const int UsernameMaxLength = 50;
        private const int EmailMaxLength = 256;
        private const int PasswordMinLength = 10;
        private const string UserRole = "User";

        private readonly IConfiguration _config;
        private readonly FootballDbContext _db;
        private readonly IPasswordHasherService _passwordHasher;

        public RegistrationController(
            IConfiguration config,
            FootballDbContext db,
            IPasswordHasherService passwordHasher)
        {
            _config = config;
            _db = db;
            _passwordHasher = passwordHasher;
        }

        [HttpGet("check-username")]
        public async Task<ActionResult<RegistrationAvailabilityDTO>> CheckUsername([FromQuery] string username)
        {
            var normalizedUsername = Normalize(username);
            var errors = ValidateUsername(normalizedUsername);

            if (errors.Count > 0)
            {
                return BadRequest(new RegistrationAvailabilityDTO
                {
                    IsAvailable = false,
                    Message = errors[0]
                });
            }

            var exists = await _db.AppUsers.AnyAsync(u => u.Username == normalizedUsername);
            return Ok(new RegistrationAvailabilityDTO
            {
                IsAvailable = !exists,
                Message = exists ? "This username is already taken" : "Username is available"
            });
        }

        [HttpGet("check-email")]
        public async Task<ActionResult<RegistrationAvailabilityDTO>> CheckEmail([FromQuery] string email)
        {
            var normalizedEmail = Normalize(email);
            var errors = ValidateEmail(normalizedEmail);

            if (errors.Count > 0)
            {
                return BadRequest(new RegistrationAvailabilityDTO
                {
                    IsAvailable = false,
                    Message = errors[0]
                });
            }

            var exists = await _db.AppUsers.AnyAsync(u => u.Email == normalizedEmail);
            return Ok(new RegistrationAvailabilityDTO
            {
                IsAvailable = !exists,
                Message = exists ? "An account already exists with this email" : "Email is available"
            });
        }

        [HttpPost("complete")]
        public async Task<ActionResult<CompleteRegistrationResponseDTO>> Complete([FromBody] CompleteRegistrationRequestDTO request)
        {
            var username = Normalize(request.Username);
            var email = Normalize(request.Email);
            var validationErrors = ValidateCompleteRequest(request, username, email);

            if (validationErrors.Count > 0)
            {
                return RegistrationValidationProblem(validationErrors);
            }

            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var uniquenessErrors = new Dictionary<string, string[]>();
            if (await _db.AppUsers.AnyAsync(u => u.Username == username))
            {
                AddError(uniquenessErrors, "username", "This username is already taken");
            }

            if (await _db.AppUsers.AnyAsync(u => u.Email == email))
            {
                AddError(uniquenessErrors, "email", "An account already exists with this email");
            }

            if (uniquenessErrors.Count > 0)
            {
                return Conflict(new RegistrationConflictDTO
                {
                    Code = "AccountUnavailable",
                    Message = "Some account details are no longer available.",
                    Errors = uniquenessErrors
                });
            }

            var serverExists = await _db.Servers.AnyAsync(s => s.ServerID == request.ServerID);
            if (!serverExists)
            {
                AddError(validationErrors, "serverID", "Selected server does not exist.");
                return RegistrationValidationProblem(validationErrors);
            }

            var selectedTeam = await _db.Teams
                .AsNoTracking()
                .Where(t =>
                    t.TeamID == request.TeamID &&
                    t.Competitions.Any(c =>
                        c.ServerID == request.ServerID &&
                        c.NationID == request.NationID &&
                        c.Priority == 2))
                .Select(t => new
                {
                    TeamID = t.TeamID,
                    TeamName = t.Name,

                    CompetitionID = t.Competitions
                        .Where(c =>
                            c.ServerID == request.ServerID &&
                            c.NationID == request.NationID &&
                            c.Priority == 2)
                        .Select(c => c.CompetitionID)
                        .FirstOrDefault(),

                    CompetitionName = t.Competitions
                        .Where(c =>
                            c.ServerID == request.ServerID &&
                            c.NationID == request.NationID &&
                            c.Priority == 2)
                        .Select(c => c.CompetitionName)
                        .FirstOrDefault(),

                    NationID = t.Competitions
                        .Where(c =>
                            c.ServerID == request.ServerID &&
                            c.NationID == request.NationID &&
                            c.Priority == 2)
                        .Select(c => c.NationID)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (selectedTeam == null)
            {
                AddError(validationErrors, "teamID", "Selected team is not available for registration on this server.");
                return RegistrationValidationProblem(validationErrors);
            }

            var isAlreadyManaged = await _db.Teams
                .AnyAsync(t => t.TeamID == request.TeamID && t.AppUserID != null);

            if (isAlreadyManaged)
            {
                return TeamUnavailableConflict();
            }

            var userID = Guid.NewGuid();
            _passwordHasher.CreateHash(request.Password, out var hash, out var salt);

            var person = new Person
            {
                Name = "John",
                Surname = "Doe",
                DateOfBirth = new DateOnly(1970, 1, 1),
                PlaceOfBirth = "Athens",
                ServerID = request.ServerID,
                StaffRole = StaffRole.Manager,
                Weight = Random.Shared.Next(75, 95),
                Height = Random.Shared.Next(175, 195)
            };

            var user = new AppUser
            {
                Id = userID,
                Username = username,
                Email = email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Person = person,
                Claims = new List<AppUserClaim>
                {
                    new AppUserClaim { Type = ClaimTypes.Role, Value = UserRole },
                    new AppUserClaim { Type = ClaimTypes.NameIdentifier, Value = userID.ToString() }
                }
            };

            _db.AppUsers.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return AccountConflictFromDbException(ex);
            }

            var claimedRows = await _db.Teams
                .Where(t => t.TeamID == request.TeamID && t.AppUserID == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.AppUserID, userID));

            if (claimedRows != 1)
            {
                await transaction.RollbackAsync();
                return TeamUnavailableConflict();
            }

            _db.Contracts.Add(new Contract
            {
                PersonID = person.PersonID,
                TeamID = request.TeamID,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = null,
                Role = Role.Staff
            });

            var refreshToken = CreateRefreshToken();
            var refreshExpires = DateTime.UtcNow.AddDays(14);
            _db.RefreshTokens.Add(new RefreshToken
            {
                AppUserID = userID,
                TokenHash = Sha256(refreshToken),
                ExpiresUtc = refreshExpires
            });

            try
            {
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                return BadRequest("Could not complete registration.");
            }

            SetRefreshCookie(refreshToken, refreshExpires);

            return Ok(new CompleteRegistrationResponseDTO
            {
                Token = GenerateJwtToken(userID.ToString(), user.Claims),
                Role = UserRole,
                ServerID = request.ServerID,
                TeamID = request.TeamID,
                TeamName = selectedTeam.TeamName
            });
        }

        private static Dictionary<string, string[]> ValidateCompleteRequest(
            CompleteRegistrationRequestDTO request,
            string username,
            string email)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var message in ValidateUsername(username))
            {
                AddError(errors, "username", message);
            }

            foreach (var message in ValidateEmail(email))
            {
                AddError(errors, "email", message);
            }

            foreach (var message in ValidatePassword(request.Password))
            {
                AddError(errors, "password", message);
            }

            if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                AddError(errors, "confirmPassword", "Confirm password is required.");
            }
            else if (request.Password != request.ConfirmPassword)
            {
                AddError(errors, "confirmPassword", "Passwords do not match.");
            }

            if (request.ServerID == Guid.Empty)
            {
                AddError(errors, "serverID", "Server is required.");
            }

            if (request.NationID == Guid.Empty)
            {
                AddError(errors, "nationID", "Nation is required.");
            }

            if (request.TeamID == Guid.Empty)
            {
                AddError(errors, "teamID", "Team is required.");
            }

            return errors;
        }

        private static List<string> ValidateUsername(string username)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
            {
                errors.Add("Username is required.");
            }
            else if (username.Length > UsernameMaxLength)
            {
                errors.Add($"Username must be {UsernameMaxLength} characters or fewer.");
            }

            return errors;
        }

        private static List<string> ValidateEmail(string email)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("Email is required.");
            }
            else
            {
                if (email.Length > EmailMaxLength)
                {
                    errors.Add($"Email must be {EmailMaxLength} characters or fewer.");
                }

                if (!new EmailAddressAttribute().IsValid(email))
                {
                    errors.Add("Email must be a valid email address.");
                }
            }

            return errors;
        }

        private static List<string> ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password is required.");
                return errors;
            }

            if (password.Length < PasswordMinLength)
            {
                errors.Add($"Password must be at least {PasswordMinLength} characters.");
            }

            if (!password.Any(char.IsUpper))
            {
                errors.Add("Password must contain at least one uppercase letter.");
            }

            if (!password.Any(char.IsLower))
            {
                errors.Add("Password must contain at least one lowercase letter.");
            }

            if (!password.Any(char.IsDigit))
            {
                errors.Add("Password must contain at least one number.");
            }

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
            {
                errors.Add("Password must contain at least one symbol or special character.");
            }

            return errors;
        }

        private ObjectResult AccountConflictFromDbException(DbUpdateException exception)
        {
            var message = exception.InnerException?.Message ?? exception.Message;
            var errors = new Dictionary<string, string[]>();

            if (message.Contains("Username", StringComparison.OrdinalIgnoreCase))
            {
                AddError(errors, "username", "This username is already taken");
            }

            if (message.Contains("Email", StringComparison.OrdinalIgnoreCase))
            {
                AddError(errors, "email", "An account already exists with this email");
            }

            if (errors.Count == 0)
            {
                AddError(errors, "username", "Username is no longer available.");
                AddError(errors, "email", "Email is no longer available.");
            }

            return Conflict(new RegistrationConflictDTO
            {
                Code = "AccountUnavailable",
                Message = "Some account details are no longer available.",
                Errors = errors
            });
        }

        private ConflictObjectResult TeamUnavailableConflict()
        {
            return Conflict(new RegistrationConflictDTO
            {
                Code = "TeamUnavailable",
                Message = "This team was just selected by another manager. Please choose another team."
            });
        }

        private BadRequestObjectResult RegistrationValidationProblem(Dictionary<string, string[]> errors)
        {
            return BadRequest(new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            });
        }

        private string GenerateJwtToken(string id, IEnumerable<AppUserClaim> dbClaims)
        {
            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT signing key (Jwt:Key) is missing.");

            var keyBytes = Convert.FromBase64String(keyString);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

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
                Secure = true,
                SameSite = SameSiteMode.None,
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

        private static string Normalize(string? value)
        {
            return value?.Trim() ?? string.Empty;
        }

        private static void AddError(Dictionary<string, string[]> errors, string key, string message)
        {
            if (!errors.TryGetValue(key, out var existingMessages))
            {
                errors[key] = new[] { message };
                return;
            }

            errors[key] = existingMessages.Concat(new[] { message }).ToArray();
        }
    }
}
