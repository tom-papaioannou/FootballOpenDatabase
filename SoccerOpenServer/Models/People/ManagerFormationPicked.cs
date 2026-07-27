using SoccerOpenServer.Models.Teams;

namespace SoccerOpenServer.Models.People
{
    public class ManagerFormationPicked
    {
        public Guid ManagerFormationPickedID { get; set; }
        public Guid PersonID { get; set; }
        public Person Person { get; set; } = null!;
        public Formation Formation { get; set; }
        public int TimesPicked { get; set; } = 0;
    }
}
