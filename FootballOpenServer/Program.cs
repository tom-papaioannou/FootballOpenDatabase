// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.Models.Users;
using FootballOpenServer.Models.World;
using FootballOpenServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;


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
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    // optional: ensure DB exists / migrations applied
    // await db.Database.MigrateAsync();

    var adminUsername = config["SeedAdmin:Username"] ?? "admin";
    var adminPassword = config["SeedAdmin:Password"] ?? "ChangeMe123!";

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

    // Seed Europe continent
    var europe = await db.Continents.FirstOrDefaultAsync(c => c.Name == "Europe");
    if (europe == null)
    {
        europe = new Continent { ContinentID = Guid.NewGuid(), Name = "Europe", Code = "EUR" };
        db.Continents.Add(europe);
        await db.SaveChangesAsync();
    }

    // Seed nations for Europe
    var nationSeeds = new Dictionary<string, string>
    {
        { "Greece",  "GR" },
        { "England", "GB" },
        { "Italy",   "IT" },
        { "France",  "FR" },
        { "Germany", "DE" }
    };

    var existingNationNames = await db.Nations
        .Where(n => nationSeeds.Keys.Contains(n.Name))
        .Select(n => n.Name)
        .ToListAsync();

    foreach (var (name, iso2) in nationSeeds)
    {
        if (!existingNationNames.Contains(name))
        {
            db.Nations.Add(new Nation
            {
                NationID = Guid.NewGuid(),
                Name = name,
                ISO2 = iso2,
                ContinentID = europe.ContinentID
            });
        }
    }
    await db.SaveChangesAsync();
}

app.Run();
