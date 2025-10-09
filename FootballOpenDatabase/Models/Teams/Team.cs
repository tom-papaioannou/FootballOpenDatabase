using FootballOpenDatabase.Models.Tournaments;

namespace FootballOpenDatabase.Models.Teams
{
    public class Team
    {
        public Guid TeamID { get; set; }

        public string? Name { get; set; }

        public ICollection<Tournament>? Tournaments { get; set; }
        public ICollection<Contract>? Contracts { get; set; }
    }
}
