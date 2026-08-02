using SoccerOpenServer.Models.Teams;

namespace SoccerOpenServer.DTO.Teams
{
    public class TeamTacticPriorityDto
    {
        public Guid TeamTacticPriorityID { get; set; }
        public Guid PersonID { get; set; }
        public TeamTacticPriorityType Type { get; set; }
        public int Priority { get; set; }
    }
}
