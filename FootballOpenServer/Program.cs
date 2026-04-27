// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Servers;
using FootballOpenServer.Models.Users;
using FootballOpenServer.Models.World;
using FootballOpenServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FootballDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", p => p
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<ITeamGenerationService, TeamGenerationService>();
builder.Services.AddScoped<ITeamAccessService, TeamAccessService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DevCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FootballDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();
    var teamGenerationService = scope.ServiceProvider.GetRequiredService<ITeamGenerationService>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    // optional: ensure DB exists / migrations applied
    // await db.Database.MigrateAsync();

    var adminUsername = config["SeedAdmin:Username"] ?? "admin";
    var adminPassword = config["SeedAdmin:Password"] ?? "ChangeMe123!";
    var hostPassword = config["SeedHost:Password"] ?? "ChangeMe123!";
    var userPassword = config["SeedUser:Password"] ?? "ChangeMe123!";

    var adminExists = await db.AppUsers.AnyAsync(u => u.Username == adminUsername);

    if (!adminExists)
    {
        hasher.CreateHash(adminPassword, out var hash, out var salt);

        var admin = new AppUser
        {
            Username = adminUsername,
            PasswordHash = hash,
            PasswordSalt = salt,
            Claims = new List<AppUserClaim>
            {
                new AppUserClaim { Type = ClaimTypes.Role, Value = "Admin" },
                new AppUserClaim { Type = ClaimTypes.NameIdentifier, Value = adminUsername }
            }
        };

        db.AppUsers.Add(admin);
        await db.SaveChangesAsync();
    }

    // Create Europe
    Continent? europe = await db.Continents.FirstOrDefaultAsync(c => c.Name == "Europe");
    if (europe == null)
    {
        europe = new Continent { ContinentID = Guid.NewGuid(), Name = "Europe", Code = "EUR" };
        db.Continents.Add(europe);
        await db.SaveChangesAsync();
    }

    var europeanNations = new (string Name, string ISO2, string ISO3)[]
    {
        ("Greece",  "GR", "GRE"),
        ("England", "GB", "ENG"),
        ("Italy",   "IT", "ITA"),
        ("France",  "FR", "FRA"),
        ("Germany", "DE", "DEU")
    };

    var existingNationNames = await db.Nations
        .Where(n => europeanNations.Select(s => s.Name).Contains(n.Name))
        .Select(n => n.Name)
        .ToListAsync();

    foreach (var (name, iso2, iso3) in europeanNations)
    {
        if (!existingNationNames.Contains(name))
        {
            db.Nations.Add(new Nation
            {
                NationID = Guid.NewGuid(),
                Name = name,
                ISO2 = iso2,
                ISO3 = iso3,
                ContinentID = europe.ContinentID
            });
        }
    }

    // Create South America
    Continent? southAmerica = await db.Continents.FirstOrDefaultAsync(c => c.Name == "South America");
    if (southAmerica == null)
    {
        southAmerica = new Continent { ContinentID = Guid.NewGuid(), Name = "South America", Code = "SAM" };
        db.Continents.Add(southAmerica);
        await db.SaveChangesAsync();
    }

    var nationSeedsSouthAmerica = new (string Name, string ISO2, string ISO3)[]
    {
        ("Argentina", "AR", "ARG"),
        ("Brazil", "BR", "BRA")
    };

    var existingNationNamesSouthAmerica = await db.Nations
        .Where(n => nationSeedsSouthAmerica.Select(s => s.Name).Contains(n.Name))
        .Select(n => n.Name)
        .ToListAsync();

    foreach (var (name, iso2, iso3) in nationSeedsSouthAmerica)
    {
        if (!existingNationNamesSouthAmerica.Contains(name))
        {
            db.Nations.Add(new Nation
            {
                NationID = Guid.NewGuid(),
                Name = name,
                ISO2 = iso2,
                ISO3 = iso3,
                ContinentID = southAmerica.ContinentID
            });
        }
    }

    await db.SaveChangesAsync();

    // Create default server if none exist
    var serverExists = await db.Servers.AnyAsync();
    if (!serverExists)
    {
        db.Servers.Add(new Server { ServerID = Guid.NewGuid(), Name = "Main" });
        await db.SaveChangesAsync();
    }

    var mainServer = await db.Servers.FirstOrDefaultAsync(s => s.Name == "Main");

    // Create default host user if none exist
    var hostExists = await db.AppUsers.AnyAsync(u => u.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Host"));
    if (!hostExists && mainServer != null)
    {
        hasher.CreateHash(hostPassword, out var hostHash, out var hostSalt);
        Guid hostId = Guid.NewGuid();

        var hostUser = new AppUser
        {
            Id = hostId,
            Username = "host",
            PasswordHash = hostHash,
            PasswordSalt = hostSalt,
            Claims = new List<AppUserClaim>
            {
                new AppUserClaim { Type = ClaimTypes.Role, Value = "Host" },
                new AppUserClaim { Type = ClaimTypes.NameIdentifier, Value = hostId.ToString() }
            },
            Person = new Person
            {
                Name = "John",
                Surname = "Doe",
                DateOfBirth = new DateOnly(1970, 1, 1),
                PlaceOfBirth = "Athens",
                ServerID = mainServer.ServerID
            }
        };

        db.AppUsers.Add(hostUser);
        await db.SaveChangesAsync();
    }

    // Create default competition if none exist
    var competitionExists = await db.Competitions.AnyAsync();
    if (!competitionExists && mainServer != null)
    {
        var greece = await db.Nations.FirstOrDefaultAsync(n => n.Name == "Greece");
        if (greece != null)
        {
            var generatedTeams = await teamGenerationService.GenerateTeamsForCompetition(mainServer.ServerID, greece.NationID, 20);

            var competition = new Competition
            {
                CompetitionID = Guid.NewGuid(),
                CompetitionName = "Greek League 1",
                NationID = greece.NationID,
                CompetitionTeamsType = CompetitionTeamsType.Club,
                Priority = 1,
                CompetitionType = CompetitionType.League,
                Teams = generatedTeams,
                ServerID = mainServer.ServerID
            };

            db.Competitions.Add(competition);
            await db.SaveChangesAsync();
        }
    }

    // Create default regular user if none exist
    var userExists = await db.AppUsers.AnyAsync(u => u.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "User"));
    if (!userExists && mainServer != null)
    {
        hasher.CreateHash(userPassword, out var userHash, out var userSalt);
        Guid userId = Guid.NewGuid();

        var person = new Person
        {
            Name = "John",
            Surname = "Doe",
            DateOfBirth = new DateOnly(1970, 1, 1),
            PlaceOfBirth = "Athens",
            ServerID = mainServer.ServerID,
            StaffRole = StaffRole.Manager
        };

        var regularUser = new AppUser
        {
            Id = userId,
            Username = "user",
            PasswordHash = userHash,
            PasswordSalt = userSalt,
            Claims = new List<AppUserClaim>
            {
                new AppUserClaim { Type = ClaimTypes.Role, Value = "User" },
                new AppUserClaim { Type = ClaimTypes.NameIdentifier, Value = userId.ToString() }
            },
            Person = person
        };

        var now = DateTime.UtcNow;
        var availableTeams = await db.Teams
            .Where(t => !db.People.Any(p => p.StaffRole == StaffRole.Manager && p.Contracts.Any(c => c.TeamID == t.TeamID && (c.EndDate == null || c.EndDate > DateOnly.FromDateTime(now)))))
            .ToListAsync();

        if (availableTeams.Any())
        {
            var randomTeam = availableTeams[Random.Shared.Next(availableTeams.Count)];

            var contract = new Contract
            {
                Person = person,
                Team = randomTeam,
                StartDate = DateOnly.FromDateTime(now),
                EndDate = DateOnly.FromDateTime(now.AddYears(1))
            };

            db.Contracts.Add(contract);
            randomTeam.AppUserID = userId;
            db.Teams.Update(randomTeam);
        }

        db.AppUsers.Add(regularUser);
        await db.SaveChangesAsync();
    }
}

app.Run();
