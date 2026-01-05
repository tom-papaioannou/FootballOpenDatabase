namespace FootballOpenServer.Models.People
{
    public class PlayerTrainedPosition
    {
        public Guid PlayerTrainedPositionID { get; set; }
        public Guid PlayerID { get; set; }
        public PlayerPosition PlayerPosition { get; set; }
        public int PlayerTrainedPositionAdaptaption { get; set; }
    }
}
