using FootballOpenServer.Models.Users;
using System.Text.Json.Serialization;

namespace FootballOpenServer.Models.People
{
    public class Person
    {
        public Guid PersonID { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public Guid? ContractID { get; set; }
        [JsonIgnore]
        public virtual Player? Player { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
