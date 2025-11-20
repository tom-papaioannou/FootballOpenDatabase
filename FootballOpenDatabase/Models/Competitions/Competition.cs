using FootballOpenDatabase.Models.Teams;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballOpenDatabase.Models.Competitions
{
    public class Competition
    {
        public Guid CompetitionID { get; set; }

        public string? CompetitionName { get; set; }

        public Guid ParentID { get; set; }

        [ForeignKey("ParentID")]
        public virtual CompetitionParent? CompetitionParent { get; set; }

        public CompetitionTeamsType CompetitionTeamsType { get; set; }

        public int Priority { get; set; } // Lower number means higher League/Cup in the hierarchy

        public CompetitionType CompetitionType { get; set; }

        public ICollection<Team> Teams { get; set; }
    }
}
