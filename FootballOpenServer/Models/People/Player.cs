namespace FootballOpenServer.Models.People
{
    public class Player
    {
        public Guid PlayerID { get; set; }
        public Guid PersonID { get; set; }
        public virtual Person? Person { get; set; }
        public ICollection<PlayerTrainedPosition> PlayerTrainedPositions { get; set; }
        public ICollection<PlayerTrainedRole> PlayerTrainedRoles { get; set; }
    }
}
