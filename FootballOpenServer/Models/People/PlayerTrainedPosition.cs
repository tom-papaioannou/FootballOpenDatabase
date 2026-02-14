using System.ComponentModel.DataAnnotations;

namespace FootballOpenServer.Models.People
{
    public class PlayerTrainedPosition
    {
        public Guid PlayerTrainedPositionID { get; set; }
        public Guid PlayerID { get; set; }
        public PlayerPosition PlayerPosition { get; set; }

        [Range(1, 100)]
        public byte PlayerTrainedPositionAdaptation { get; set; }
    }
}
