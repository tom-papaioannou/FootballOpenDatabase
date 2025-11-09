namespace FootballOpenDatabase.Models.People
{
    public class PlayerTrainedRole
    {
        public Guid PlayerTrainedRoleID { get; set; }
        public Guid PlayerID { get; set; }
        public PlayerPosition PlayerPosition { get; set; }
        public PlayerRole PlayerRole { get; set; }

        public int PlayerTrainedRoleAdaptaption { get; set; }
    }
}
