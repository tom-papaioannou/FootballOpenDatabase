using FootballOpenDatabase.Models.Teams;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballOpenDatabase.Models.Tournaments
{
    public class Tournament
    {
        public Guid TournamentID { get; set; }

        public string? TournamentName { get; set; }

        public Guid ParentID { get; set; }

        [ForeignKey("ParentID")]
        public virtual TournamentParent? TournamentParent { get; set; }

        public TournamentTeamsType TournamentTeamsType { get; set; }

        public int Priority { get; set; } // Lower number means higher League/Cup in the hierarchy

        public TournamentType TournamentType { get; set; }

        public ICollection<Team> Teams { get; set; }
    }
}
