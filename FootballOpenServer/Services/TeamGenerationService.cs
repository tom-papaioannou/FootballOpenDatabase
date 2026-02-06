using FootballOpenServer.Models.Teams;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Contracts;

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

        private static readonly string[] FirstNames = new[]
        {
            "James", "John", "Robert", "Michael", "William", "David", "Richard", "Joseph",
            "Thomas", "Charles", "Christopher", "Daniel", "Matthew", "Anthony", "Mark", "Donald",
            "Steven", "Paul", "Andrew", "Joshua", "Kenneth", "Kevin", "Brian", "George",
            "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Jacob", "Gary",
            "Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon",
            "Benjamin", "Samuel", "Raymond", "Gregory", "Alexander", "Patrick", "Jack", "Dennis"
        };

        private static readonly string[] LastNames = new[]
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
            "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas",
            "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White",
            "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young",
            "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
            "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell"
        };

        private readonly FootballDbContext _context;

        public TeamGenerationService(FootballDbContext context)
        {
            _context = context;
        }

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

                // Generate 30 people for each team
                for (int j = 0; j < 30; j++)
                {
                    var personID = Guid.NewGuid();
                    var playerID = Guid.NewGuid();

                    // Create Person
                    var person = new Person
                    {
                        PersonID = personID,
                        Name = FirstNames[random.Next(FirstNames.Length)],
                        Surname = LastNames[random.Next(LastNames.Length)],
                        DateOfBirth = DateTime.Now.AddYears(-random.Next(18, 35)).AddDays(-random.Next(0, 365))
                    };

                    // Create Contract with random end date (June 30, random year 2026-2030)
                    var contractEndYear = random.Next(2026, 2031); // 2031 is exclusive, so 2026-2030
                    var contract = new Contract
                    {
                        ContractID = Guid.NewGuid(),
                        PersonID = personID,
                        TeamID = team.TeamID,
                        StartDate = DateTime.Now,
                        EndDate = new DateTime(contractEndYear, 6, 30),
                        Role = Role.Player
                    };

                    // Create Player
                    var player = new Player
                    {
                        PlayerID = playerID,
                        PersonID = personID,
                        Person = person,
                        PlayerTrainedPositions = new List<PlayerTrainedPosition>(),
                        PlayerTrainedRoles = new List<PlayerTrainedRole>()
                    };

                    // Generate PlayerTrainedPositions
                    var trainedPositions = GeneratePlayerTrainedPositions(random, playerID);
                    
                    // Generate PlayerTrainedRoles for each PlayerTrainedPosition
                    var trainedRoles = new List<PlayerTrainedRole>();
                    foreach (var trainedPosition in trainedPositions)
                    {
                        var rolesForPosition = GeneratePlayerTrainedRoles(random, playerID, trainedPosition.PlayerPosition);
                        trainedRoles.AddRange(rolesForPosition);
                    }

                    // Add entities to database context
                    _context.People.Add(person);
                    _context.Contracts.Add(contract);
                    _context.Players.Add(player);
                    _context.PlayerTrainedPositions.AddRange(trainedPositions);
                    _context.PlayerTrainedRoles.AddRange(trainedRoles);
                }

                teams.Add(team);
            }

            return teams;
        }

        private List<PlayerTrainedPosition> GeneratePlayerTrainedPositions(Random random, Guid playerID)
        {
            var trainedPositions = new List<PlayerTrainedPosition>();
            
            // Get all valid player positions (exclude None)
            var validPositions = Enum.GetValues(typeof(PlayerPosition))
                .Cast<PlayerPosition>()
                .Where(p => p != PlayerPosition.None)
                .ToList();

            // First trained position (80-100 adaptaption)
            var firstPosition = validPositions[random.Next(validPositions.Count)];
            trainedPositions.Add(new PlayerTrainedPosition
            {
                PlayerTrainedPositionID = Guid.NewGuid(),
                PlayerID = playerID,
                PlayerPosition = firstPosition,
                PlayerTrainedPositionAdaptaption = random.Next(80, 101) // 80-100 inclusive
            });

            // 15% chance for second trained position (50-80 adaptaption)
            if (random.Next(100) < 15)
            {
                // Make sure second position is different from first
                var availablePositions = validPositions.Where(p => p != firstPosition).ToList();
                if (availablePositions.Any())
                {
                    var secondPosition = availablePositions[random.Next(availablePositions.Count)];
                    trainedPositions.Add(new PlayerTrainedPosition
                    {
                        PlayerTrainedPositionID = Guid.NewGuid(),
                        PlayerID = playerID,
                        PlayerPosition = secondPosition,
                        PlayerTrainedPositionAdaptaption = random.Next(50, 81) // 50-80 inclusive
                    });
                }
            }

            return trainedPositions;
        }

        private List<PlayerTrainedRole> GeneratePlayerTrainedRoles(Random random, Guid playerID, PlayerPosition position)
        {
            var trainedRoles = new List<PlayerTrainedRole>();
            
            // Get all valid player roles (exclude None)
            var validRoles = Enum.GetValues(typeof(PlayerRole))
                .Cast<PlayerRole>()
                .Where(r => r != PlayerRole.None)
                .ToList();

            // First trained role (80-100 adaptaption)
            var firstRole = validRoles[random.Next(validRoles.Count)];
            trainedRoles.Add(new PlayerTrainedRole
            {
                PlayerTrainedRoleID = Guid.NewGuid(),
                PlayerID = playerID,
                PlayerPosition = position,
                PlayerRole = firstRole,
                PlayerTrainedRoleAdaptaption = random.Next(80, 101) // 80-100 inclusive
            });

            // 15% chance for second trained role (50-80 adaptaption)
            if (random.Next(100) < 15)
            {
                // Make sure second role is different from first
                var availableRoles = validRoles.Where(r => r != firstRole).ToList();
                if (availableRoles.Any())
                {
                    var secondRole = availableRoles[random.Next(availableRoles.Count)];
                    trainedRoles.Add(new PlayerTrainedRole
                    {
                        PlayerTrainedRoleID = Guid.NewGuid(),
                        PlayerID = playerID,
                        PlayerPosition = position,
                        PlayerRole = secondRole,
                        PlayerTrainedRoleAdaptaption = random.Next(50, 81) // 50-80 inclusive
                    });
                }
            }

            return trainedRoles;
        }
    }
}
