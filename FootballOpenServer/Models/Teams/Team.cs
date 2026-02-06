using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Contracts;
using System.Text.Json.Serialization;

namespace FootballOpenServer.Models.Teams
{
    public class Team
    {
        public Guid TeamID { get; set; }

        public string? Name { get; set; }

        [JsonIgnore]
        public ICollection<Competition>? Competitions { get; set; }
        public ICollection<Contract>? Contracts { get; set; }
    }
}
