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
    public DbSet<PlayerStats> PlayerStats { get; set; }
    public DbSet<Nation> Nations { get; set; }
    public DbSet<Continent> Continents { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<Kit> Kits { get; set; }
    public DbSet<CompetitionTable> CompetitionTables { get; set; }
    public DbSet<PersonHealthAndFitness> PersonHealthAndFitnesses { get; set; }
    public DbSet<CupRound> CupRounds { get; set; }
    public DbSet<CupTie> CupTies { get; set; }
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

        modelBuilder.Entity<AppUser>()
            .Property(u => u.Email)
            .HasMaxLength(256);

        modelBuilder.Entity<AppUser>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

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
            .HasOne(pt => pt.Person)
            .WithMany(p => p.PlayerTactics)
            .HasForeignKey(pt => pt.PersonID);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Person)
            .WithMany(p => p.Contracts)
            .HasForeignKey(c => c.PersonID);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Team)
            .WithMany(t => t.Contracts)
            .HasForeignKey(c => c.TeamID);

        modelBuilder.Entity<PlayerStats>()
            .HasIndex(ps => ps.PersonID)
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

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Server)
            .WithMany(s => s.Persons)
            .HasForeignKey(p => p.ServerID)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Competition>()
            .HasOne(c => c.Server)
            .WithMany(s => s.Competitions)
            .HasForeignKey(c => c.ServerID)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.AppUser)
            .WithMany()
            .HasForeignKey(t => t.AppUserID)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Team>()
            .HasIndex(t => t.AppUserID)
            .IsUnique()
            .HasFilter("[AppUserID] IS NOT NULL");

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Stadium)
            .WithOne(s => s.Team)
            .HasForeignKey<Team>(t => t.StadiumID);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Kit)
            .WithOne(k => k.Team)
            .HasForeignKey<Team>(t => t.KitID);

        modelBuilder.Entity<Person>()
            .HasOne(p => p.HealthAndFitness)
            .WithOne(h => h.Person)
            .HasForeignKey<PersonHealthAndFitness>(h => h.PersonID)
            .IsRequired();

        modelBuilder.Entity<CupRound>()
            .HasKey(x => x.CupRoundID);

        modelBuilder.Entity<CupRound>()
            .HasOne(x => x.Competition)
            .WithMany()
            .HasForeignKey(x => x.CompetitionID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CupTie>()
            .HasKey(x => x.CupTieID);

        modelBuilder.Entity<CupTie>()
            .HasOne(x => x.CupRound)
            .WithMany(x => x.Ties)
            .HasForeignKey(x => x.CupRoundID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CupTie>()
            .HasOne<CupTie>()
            .WithMany()
            .HasForeignKey(x => x.NextCupTieID)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CupRound>()
            .HasIndex(x => new { x.CompetitionID, x.RoundNumber })
            .IsUnique();

        modelBuilder.Entity<CupTie>()
            .HasIndex(x => new { x.CupRoundID, x.TieNumber })
            .IsUnique();
    }
}