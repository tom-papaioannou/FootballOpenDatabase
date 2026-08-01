using SoccerOpenServer.Models.Competitions;

namespace SoccerOpenServer.Models.People
{
    public class PlayerCompetitionDiscipline
    {
        public Guid PersonID { get; set; }
        public Person Person { get; set; } = null!;

        public Guid CompetitionID { get; set; }
        public Competition Competition { get; set; } = null!;

        public int YellowCards { get; set; }
    }
}
