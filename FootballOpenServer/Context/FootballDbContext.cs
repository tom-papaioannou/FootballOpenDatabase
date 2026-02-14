using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Teams;
using FootballOpenServer.Models.Users;
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
    public DbSet<CompetitionParent> CompetitionParents { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppUserClaim> AppUserClaims { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
    public DbSet<Staff> Staffs { get; set; }

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
    }
}