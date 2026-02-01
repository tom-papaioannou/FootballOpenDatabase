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
    }
}