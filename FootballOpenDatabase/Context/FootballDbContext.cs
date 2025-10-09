using FootballOpenDatabase.Models.Teams;
using FootballOpenDatabase.Models.Tournaments;
using Microsoft.EntityFrameworkCore;

public class FootballDbContext : DbContext
{
    public DbSet<Tournament> Tournaments { get; set; }
    public DbSet<TournamentParent> TournamentParents { get; set; }
    public DbSet<Team> Teams { get; set; }

    public DbSet<Contract> Contracts { get; set; }

    public FootballDbContext(DbContextOptions<FootballDbContext> options)
        : base(options) { }
}