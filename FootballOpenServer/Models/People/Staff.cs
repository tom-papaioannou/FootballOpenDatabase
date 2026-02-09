using FootballOpenServer.Models.Teams;

namespace FootballOpenServer.Models.People
{
    public class Staff
    {
        public int StaffID { get; set; }
        public Guid PersonID { get; set; }
        public virtual Person? Person { get; set; }
        public StaffRole StaffRole { get; set; }
    }
}
