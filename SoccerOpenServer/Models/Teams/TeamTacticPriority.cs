using SoccerOpenServer.Models.People;

namespace SoccerOpenServer.Models.Teams
{
    public class TeamTacticPriority
    {
        public Guid TeamTacticPriorityID { get; set; }
        public Guid TeamID { get; set; }
        public Team Team { get; set; } = null!;
        public TeamTacticPriorityType Type { get; set; }
        public Guid PersonID { get; set; }
        public Person Person { get; set; } = null!;
        public int Priority { get; set; }
    }
}
