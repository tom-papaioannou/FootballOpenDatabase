namespace FootballOpenServer.Models.Competitions
{
    public class CreateCompetitionRequest
    {
        public string? CompetitionName { get; set; }
        public Guid ParentID { get; set; }
        public CompetitionTeamsType CompetitionTeamsType { get; set; }
        public int Priority { get; set; }
        public CompetitionType CompetitionType { get; set; }
    }
}
