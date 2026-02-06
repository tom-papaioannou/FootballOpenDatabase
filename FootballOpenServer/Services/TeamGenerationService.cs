using FootballOpenServer.Models.Teams;

namespace FootballOpenServer.Services
{
    public interface ITeamGenerationService
    {
        List<Team> GenerateTeamsForCompetition(int numberOfTeams = 20);
    }

    public class TeamGenerationService : ITeamGenerationService
    {
        private static readonly string[] TeamNames = new[]
        {
            "CanonRed FC", "Limani City", "Peter United", "Sea FC", "Ham Ham SC",
            "Metropolitan City", "Lester Country", "Levadia", "Notos FC", "Newcera",
            "Karpe DIEM Villa", "Southeastamptonica", "Crystal City", "Bright Future SC", "Likoi FC",
            "Burn-Burn FC", "Leeds United", "Notham Town", "Fulleriom", "Brenta",
            "Catalonica", "Real Athens", "Atletico Posidwnos", "Cella", "Vale King",
            "Sillogos Munich", "Dort Port", "Leipzia Team Soccer", "Eintracht Karpenisiou",
            "AC Lamias", "Inter Kavalas", "Romaioi United",
            "Paris-France Team", "Mona Basel", "Aias Salaminas",
            "Thermaikos", "Portogalia SC", "Sport Limaniou SC",
            "Celt Heroes", "Strong Heart FC", "Milky Way United", "Andromeda Team",
            "Stars Beyond", "Galaxias Elliniki Omada", "Fioreba Team"
        };

        public List<Team> GenerateTeamsForCompetition(int numberOfTeams = 20)
        {
            var teams = new List<Team>();
            var random = new Random();

            // Create a shuffled copy of team names to avoid duplicates
            var availableNames = TeamNames.ToList();
            
            for (int i = 0; i < numberOfTeams && availableNames.Count > 0; i++)
            {
                // Pick a random name from available names
                var randomIndex = random.Next(availableNames.Count);
                var teamName = availableNames[randomIndex];
                
                // Remove the name from available names to prevent duplicates
                availableNames.RemoveAt(randomIndex);

                var team = new Team
                {
                    TeamID = Guid.NewGuid(),
                    Name = teamName,
                    Competitions = new List<Models.Competitions.Competition>(),
                    Contracts = new List<Models.Contracts.Contract>()
                };

                teams.Add(team);
            }

            return teams;
        }
    }
}
