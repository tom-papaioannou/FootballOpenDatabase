// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Servers;
using FootballOpenServer.Models.Teams;
using FootballOpenServer.Models.Users;
using FootballOpenServer.Models.World;
using Microsoft.EntityFrameworkCore;

public class FootballDbContext : DbContext
{
    public DbSet<Person> People { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Tactic> Tactics { get; set; }
    public DbSet<PlayerTactic> PlayerTactics { get; set; }
    public DbSet<PlayerTrainedPosition> PlayerTrainedPositions { get; set; }
    public DbSet<PlayerTrainedRole> PlayerTrainedRoles { get; set; }
    public DbSet<Competition> Competitions { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppUserClaim> AppUserClaims { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<PlayerStats> PlayerStats { get; set; }
    public DbSet<Nation> Nations { get; set; }
    public DbSet<Continent> Continents { get; set; }
    public DbSet<Server> Servers { get; set; }

    public FootballDbContext(DbContextOptions<FootballDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUser>()
            .HasMany(u => u.Claims)
            .WithOne(c => c.AppUser)
            .HasForeignKey(c => c.AppUserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<AppUserClaim>()
            .Property(c => c.Type)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<AppUserClaim>()
            .Property(c => c.Value)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.AppUser)
            .WithMany()
            .HasForeignKey(rt => rt.AppUserID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Person)
            .WithOne(p => p.AppUser)
            .HasForeignKey<AppUser>(u => u.PersonID)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PlayerTactic>()
            .HasOne(pt => pt.Player)
            .WithMany(p => p.PlayerTactics)
            .HasForeignKey(pt => pt.PlayerID);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Person)
            .WithMany(p => p.Contracts)
            .HasForeignKey(c => c.PersonID);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Team)
            .WithMany(t => t.Contracts)
            .HasForeignKey(c => c.TeamID);

        modelBuilder.Entity<PlayerStats>()
            .HasIndex(ps => ps.PlayerID)
            .IsUnique();

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Nation)
            .WithMany()
            .HasForeignKey(p => p.NationID)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Continent>()
            .HasMany(c => c.Nations)
            .WithOne(n => n.Continent)
            .HasForeignKey(n => n.ContinentID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Server>()
            .HasMany(s => s.Persons)
            .WithOne(p => p.Server)
            .HasForeignKey(s => s.ServerID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Server>()
            .HasMany(s => s.Competitions)
            .WithOne(p => p.Server)
            .HasForeignKey(s => s.ServerID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}