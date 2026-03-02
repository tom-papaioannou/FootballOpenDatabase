using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.People;

namespace FootballOpenServer.Models.Servers
{
    public class Server
    {
        public Guid ServerID { get; set; }
        public string Name { get; set; }
        public List<Person> Persons { get; set; }
        public List<Competition> Competitions { get; set; }
    }
}
