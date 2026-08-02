using SoccerOpenServer.Models.Teams;

namespace SoccerOpenServer.DTO.Teams
{
    public class UpdateTeamTacticPrioritiesRequest
    {
        public TeamTacticPriorityType Type { get; set; }
        public List<Guid> PersonIDs { get; set; } = [];
    }
}
