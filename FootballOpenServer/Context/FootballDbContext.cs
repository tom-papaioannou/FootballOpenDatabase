using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Teams;
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

    public FootballDbContext(DbContextOptions<FootballDbContext> options)
        : base(options) { }
}