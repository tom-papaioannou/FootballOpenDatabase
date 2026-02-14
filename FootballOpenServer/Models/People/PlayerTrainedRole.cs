using System.ComponentModel.DataAnnotations;

namespace FootballOpenServer.Models.People
{
    public class PlayerTrainedRole
    {
        public Guid PlayerTrainedRoleID { get; set; }
        public Guid PlayerID { get; set; }
        public PlayerPosition PlayerPosition { get; set; }
        public PlayerRole PlayerRole { get; set; }

        [Range(1, 100)]
        public byte PlayerTrainedRoleAdaptation { get; set; }
    }
}
