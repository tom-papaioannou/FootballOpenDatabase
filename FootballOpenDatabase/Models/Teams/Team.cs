using FootballOpenDatabase.Models.Contracts;
using FootballOpenDatabase.Models.Competitions;

namespace FootballOpenDatabase.Models.Teams
{
    public class Team
    {
        public Guid TeamID { get; set; }

        public string? Name { get; set; }

        public ICollection<Competition>? Competitions { get; set; }
        public ICollection<Contract>? Contracts { get; set; }
    }
}
