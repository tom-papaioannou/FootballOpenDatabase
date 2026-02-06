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
            "Arsenal", "Liverpool", "Manchester United", "Chelsea", "Tottenham",
            "Manchester City", "Leicester City", "Everton", "West Ham", "Newcastle",
            "Aston Villa", "Southampton", "Crystal Palace", "Brighton", "Wolves",
            "Burnley", "Leeds United", "Nottingham Forest", "Fulham", "Brentford",
            "Barcelona", "Real Madrid", "Atletico Madrid", "Sevilla", "Valencia",
            "Bayern Munich", "Borussia Dortmund", "RB Leipzig", "Bayer Leverkusen", "Eintracht Frankfurt",
            "Juventus", "AC Milan", "Inter Milan", "Napoli", "Roma",
            "Paris Saint-Germain", "Marseille", "Lyon", "Monaco", "Lille",
            "Ajax", "PSV Eindhoven", "Feyenoord", "AZ Alkmaar", "FC Twente",
            "Benfica", "Porto", "Sporting CP", "Braga", "Vitoria Guimaraes",
            "Celtic", "Rangers", "Hearts", "Hibernian", "Aberdeen",
            "Galatasaray", "Fenerbahce", "Besiktas", "Trabzonspor", "Basaksehir",
            "Anderlecht", "Club Brugge", "Standard Liege", "Genk", "Antwerp",
            "Shakhtar Donetsk", "Dynamo Kyiv", "Fiorentina", "Lazio", "Atalanta"
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
