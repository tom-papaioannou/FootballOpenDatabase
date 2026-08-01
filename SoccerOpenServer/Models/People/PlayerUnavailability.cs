using SoccerOpenServer.Models.Competitions;

namespace SoccerOpenServer.Models.People
{
    public class PlayerUnavailability
    {
        public Guid PlayerUnavailabilityID { get; set; }

        public Guid PersonID { get; set; }
        public Person Person { get; set; } = null!;

        public Guid? CompetitionID { get; set; }
        public Competition? Competition { get; set; }

        public PlayerUnavailabilityType Type { get; set; }

        public int MatchesRemaining { get; set; }
    }
}
