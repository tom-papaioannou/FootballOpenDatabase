using FootballOpenServer.Models.Teams;

namespace FootballOpenServer.DTO.Teams
{
    public class TeamInformationDTO
    {
        public Guid TeamID { get; set; }
        public string? Name { get; set; }
        public StadiumDTO? Stadium { get; set; }
        public Kit? Kit { get; set; }
    }
}
