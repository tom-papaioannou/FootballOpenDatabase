using FootballOpenServer.Models.People;

namespace FootballOpenServer.Models.Teams
{
    public class PlayerTactic
    {
        public Guid PlayerTacticID { get; set; }
        public Guid TacticID { get; set; }
        public Guid PlayerID { get; set; }
        public PlayerPosition PlayerPosition { get; set; }
        public PlayerRole PlayerRole { get; set; }
    }
}
