// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.Models.Teams;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Contracts;

namespace FootballOpenServer.Services
{
    public interface ITeamGenerationService
    {
        List<Team> GenerateTeamsForCompetition(Guid? serverID, int numberOfTeams = 20);
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

        private static readonly string[] Cities = new[]
        {
            "London", "Paris", "Berlin", "Madrid", "Rome", "Athens", "Amsterdam", "Vienna",
            "Brussels", "Copenhagen", "Dublin", "Helsinki", "Lisbon", "Oslo", "Prague",
            "Stockholm", "Warsaw", "Budapest", "Bucharest", "Sofia", "Zagreb", "Belgrade",
            "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego",
            "Dallas", "San Jose", "Austin", "Jacksonville", "San Francisco", "Seattle", "Denver", "Washington",
            "Boston", "Nashville", "Detroit", "Portland", "Las Vegas", "Miami", "Atlanta", "Minneapolis",
            "Tokyo", "Beijing", "Shanghai", "Delhi", "Mumbai", "Seoul", "Jakarta", "Manila",
            "Bangkok", "Ho Chi Minh City", "Hong Kong", "Singapore", "Kuala Lumpur", "Taipei", "Osaka", "Karachi",
            "Sydney", "Melbourne", "Brisbane", "Perth", "Auckland", "Wellington", "Adelaide", "Canberra",
            "Sao Paulo", "Rio de Janeiro", "Buenos Aires", "Lima", "Bogota", "Santiago", "Caracas", "Mexico City",
            "Toronto", "Montreal", "Vancouver", "Calgary", "Ottawa", "Edmonton", "Winnipeg", "Quebec City",
            "Cairo", "Lagos", "Nairobi", "Johannesburg", "Cape Town", "Casablanca", "Algiers", "Tunis",
            "Istanbul", "Dubai", "Tel Aviv", "Riyadh", "Doha", "Abu Dhabi"
        };

        private static readonly Dictionary<PlayerPosition, PlayerPosition[]> _positionTripletes = new()
        {
            // Center-Back triplete
            { PlayerPosition.RightCenterBack,            new[] { PlayerPosition.CentralCenterBack,        PlayerPosition.LeftCenterBack } },
            { PlayerPosition.CentralCenterBack,          new[] { PlayerPosition.RightCenterBack,          PlayerPosition.LeftCenterBack } },
            { PlayerPosition.LeftCenterBack,             new[] { PlayerPosition.RightCenterBack,          PlayerPosition.CentralCenterBack } },
            // Defensive-Midfielder triplete
            { PlayerPosition.RightDefensiveMidfielder,   new[] { PlayerPosition.CentralDefensiveMidfielder, PlayerPosition.LeftDefensiveMidfielder } },
            { PlayerPosition.CentralDefensiveMidfielder, new[] { PlayerPosition.RightDefensiveMidfielder,   PlayerPosition.LeftDefensiveMidfielder } },
            { PlayerPosition.LeftDefensiveMidfielder,    new[] { PlayerPosition.RightDefensiveMidfielder,   PlayerPosition.CentralDefensiveMidfielder } },
            // Center-Midfielder triplete
            { PlayerPosition.RightCenterMidfielder,      new[] { PlayerPosition.CentralCenterMidfielder,  PlayerPosition.LeftCenterMidfielder } },
            { PlayerPosition.CentralCenterMidfielder,    new[] { PlayerPosition.RightCenterMidfielder,    PlayerPosition.LeftCenterMidfielder } },
            { PlayerPosition.LeftCenterMidfielder,       new[] { PlayerPosition.RightCenterMidfielder,    PlayerPosition.CentralCenterMidfielder } },
            // Attacking-Midfielder triplete
            { PlayerPosition.RightAttackingMidfielder,   new[] { PlayerPosition.CentralAttackingMidfielder, PlayerPosition.LeftAttackingMidfielder } },
            { PlayerPosition.CentralAttackingMidfielder, new[] { PlayerPosition.RightAttackingMidfielder,   PlayerPosition.LeftAttackingMidfielder } },
            { PlayerPosition.LeftAttackingMidfielder,    new[] { PlayerPosition.RightAttackingMidfielder,   PlayerPosition.CentralAttackingMidfielder } },
            // Striker triplete
            { PlayerPosition.RightStriker,               new[] { PlayerPosition.CentralStriker,           PlayerPosition.LeftStriker } },
            { PlayerPosition.CentralStriker,             new[] { PlayerPosition.RightStriker,             PlayerPosition.LeftStriker } },
            { PlayerPosition.LeftStriker,                new[] { PlayerPosition.RightStriker,             PlayerPosition.CentralStriker } },
        };

        private readonly FootballDbContext _context;

        public TeamGenerationService(FootballDbContext context)
        {
            _context = context;
        }

        public List<Team> GenerateTeamsForCompetition(Guid? serverID,int numberOfTeams = 20)
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

                // Generate primary tactic for the team
                var primaryTactic = new Tactic
                {
                    TacticID = Guid.NewGuid(),
                    TeamID = team.TeamID,
                    Name = "Primary Tactic",
                    Formation = Formation.Four_Four_Two,
                    isMain = true
                };
                _context.Tactics.Add(primaryTactic);

                // Store player IDs for later position assignment
                var teamPlayerIDs = new List<Guid>();

                // Generate 30 people for each team
                for (int j = 0; j < 30; j++)
                {
                    var personID = Guid.NewGuid();

                    // Create Person
                    var person = new Person
                    {
                        PersonID = personID,
                        Name = FirstNames[random.Next(FirstNames.Length)],
                        Surname = LastNames[random.Next(LastNames.Length)],
                        DateOfBirth = DateTime.Now.AddYears(-random.Next(18, 35)).AddDays(-random.Next(0, 365)),
                        PlaceOfBirth = Cities[random.Next(Cities.Length)],
                        ServerID = serverID,
                        PlayerTrainedPositions = new List<PlayerTrainedPosition>(),
                        PlayerTrainedRoles = new List<PlayerTrainedRole>()
                    };

                    // Create Contract with random end date (June 30, random year 2026-2030)
                    var contractEndYear = random.Next(2026, 2031); // 2031 is exclusive, so 2026-2030
                    var contract = new Contract
                    {
                        ContractID = Guid.NewGuid(),
                        PersonID = personID,
                        TeamID = team.TeamID,
                        StartDate = DateTime.UtcNow,
                        EndDate = new DateTime(contractEndYear, 6, 30),
                        Role = Role.Player
                    };

                    // Create PlayerStats with random values between 1 and 100
                    var playerStats = new PlayerStats
                    {
                        PlayerStatsID = Guid.NewGuid(),
                        PersonID = personID,
                        Shooting = (byte)random.Next(1, 101),
                        Passing = (byte)random.Next(1, 101),
                        Crossing = (byte)random.Next(1, 101),
                        Tackling = (byte)random.Next(1, 101),
                        Dribbling = (byte)random.Next(1, 101),
                        Control = (byte)random.Next(1, 101),
                        Kicking = (byte)random.Next(1, 101),
                        Goalkeeping = (byte)random.Next(1, 101),
                        Teamwork = (byte)random.Next(1, 101),
                        Creativity = (byte)random.Next(1, 101),
                        Decisions = (byte)random.Next(1, 101),
                        Positioning = (byte)random.Next(1, 101),
                        Speed = (byte)random.Next(1, 101),
                        Acceleration = (byte)random.Next(1, 101),
                        Strength = (byte)random.Next(1, 101),
                        Jumping = (byte)random.Next(1, 101),
                        Stamina = (byte)random.Next(1, 101)
                    };

                    // Generate PlayerTrainedPositions
                    var trainedPositions = GeneratePlayerTrainedPositions(random, personID);
                    
                    // Generate PlayerTrainedRoles for each PlayerTrainedPosition
                    var trainedRoles = new List<PlayerTrainedRole>();
                    foreach (var trainedPosition in trainedPositions)
                    {
                        var rolesForPosition = GeneratePlayerTrainedRoles(random, personID, trainedPosition.PlayerPosition);
                        trainedRoles.AddRange(rolesForPosition);
                    }

                    // Add entities to database context
                    _context.People.Add(person);
                    _context.Contracts.Add(contract);
                    _context.PlayerStats.Add(playerStats);
                    _context.PlayerTrainedPositions.AddRange(trainedPositions);
                    _context.PlayerTrainedRoles.AddRange(trainedRoles);

                    // Store player ID for position assignment
                    teamPlayerIDs.Add(personID);
                }

                // Auto-assign players to positions in the primary tactic
                AssignPlayersToFormation(primaryTactic.TacticID, teamPlayerIDs, primaryTactic.Formation, random);

                teams.Add(team);
            }

            return teams;
        }

        private List<PlayerTrainedPosition> GeneratePlayerTrainedPositions(Random random, Guid personID)
        {
            var trainedPositions = new List<PlayerTrainedPosition>();
            
            // Get all valid player positions (exclude None)
            var validPositions = Enum.GetValues(typeof(PlayerPosition))
                .Cast<PlayerPosition>()
                .Where(p => p != PlayerPosition.None)
                .ToList();

            // First trained position (80-100 adaptaption)
            var firstPosition = validPositions[random.Next(validPositions.Count)];
            var firstAdaptation = (byte)random.Next(80, 101); // 80-100 inclusive
            trainedPositions.Add(new PlayerTrainedPosition
            {
                PlayerTrainedPositionID = Guid.NewGuid(),
                PersonID = personID,
                PlayerPosition = firstPosition,
                PlayerTrainedPositionAdaptation = firstAdaptation
            });

            // If firstPosition belongs to a triplete, also add the other 2 companion positions
            if (_positionTripletes.TryGetValue(firstPosition, out var firstCompanions))
            {
                foreach (var companion in firstCompanions)
                {
                    trainedPositions.Add(new PlayerTrainedPosition
                    {
                        PlayerTrainedPositionID = Guid.NewGuid(),
                        PersonID = personID,
                        PlayerPosition = companion,
                        PlayerTrainedPositionAdaptation = firstAdaptation
                    });
                }
            }

            // 15% chance for second trained position (50-80 adaptaption)
            if (random.Next(100) < 15)
            {
                // Make sure second position is different from all already-added positions
                var addedPositions = trainedPositions.Select(tp => tp.PlayerPosition).ToHashSet();
                var availablePositions = validPositions.Where(p => !addedPositions.Contains(p)).ToList();
                if (availablePositions.Any())
                {
                    var secondPosition = availablePositions[random.Next(availablePositions.Count)];
                    var secondAdaptation = (byte)random.Next(50, 81); // 50-80 inclusive
                    trainedPositions.Add(new PlayerTrainedPosition
                    {
                        PlayerTrainedPositionID = Guid.NewGuid(),
                        PersonID = personID,
                        PlayerPosition = secondPosition,
                        PlayerTrainedPositionAdaptation = secondAdaptation
                    });

                    // If secondPosition belongs to a triplete, also add the other 2 companion positions
                    if (_positionTripletes.TryGetValue(secondPosition, out var secondCompanions))
                    {
                        foreach (var companion in secondCompanions)
                        {
                            if (!addedPositions.Contains(companion))
                            {
                                trainedPositions.Add(new PlayerTrainedPosition
                                {
                                    PlayerTrainedPositionID = Guid.NewGuid(),
                                    PersonID = personID,
                                    PlayerPosition = companion,
                                    PlayerTrainedPositionAdaptation = secondAdaptation
                                });
                            }
                        }
                    }
                }
            }

            return trainedPositions;
        }

        private List<PlayerTrainedRole> GeneratePlayerTrainedRoles(Random random, Guid personID, PlayerPosition position)
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
                PersonID = personID,
                PlayerPosition = position,
                PlayerRole = firstRole,
                PlayerTrainedRoleAdaptation = (byte)random.Next(80, 101) // 80-100 inclusive
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
                        PersonID = personID,
                        PlayerPosition = position,
                        PlayerRole = secondRole,
                        PlayerTrainedRoleAdaptation = (byte)random.Next(50, 81) // 50-80 inclusive
                    });
                }
            }

            return trainedRoles;
        }

        private void AssignPlayersToFormation(Guid tacticID, List<Guid> teamPlayerIDs, Formation? formation, Random random)
        {
            if (formation == Formation.Four_Four_Two)
            {
                AssignPlayersToFourFourTwo(tacticID, teamPlayerIDs, random);
            }
            // Add more formations here as needed
        }

        private void AssignPlayersToFourFourTwo(Guid tacticID, List<Guid> teamPlayerIDs, Random random)
        {
            var assignedPlayerIDs = new HashSet<Guid>();

            // 1. Assign Goalkeeper
            var goalkeeper = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.Goalkeeper, random);
            CreatePlayerTactic(tacticID, goalkeeper, PlayerPosition.Goalkeeper, PlayerRole.Goalkeeper);
            assignedPlayerIDs.Add(goalkeeper);

            // 2. Assign Defenders (1 Left, 2 Center, 1 Right)
            var leftBack = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftBack, random);
            CreatePlayerTactic(tacticID, leftBack, PlayerPosition.LeftBack, PlayerRole.FullBack);
            assignedPlayerIDs.Add(leftBack);

            var centerBack1 = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightCenterBack, random);
            CreatePlayerTactic(tacticID, centerBack1, PlayerPosition.RightCenterBack, PlayerRole.CenterBack);
            assignedPlayerIDs.Add(centerBack1);

            var centerBack2 = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftCenterBack, random);
            CreatePlayerTactic(tacticID, centerBack2, PlayerPosition.LeftCenterBack, PlayerRole.CenterBack);
            assignedPlayerIDs.Add(centerBack2);

            var rightBack = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightBack, random);
            CreatePlayerTactic(tacticID, rightBack, PlayerPosition.RightBack, PlayerRole.FullBack);
            assignedPlayerIDs.Add(rightBack);

            // 3. Assign Midfielders (1 Left, 2 Center, 1 Right)
            var leftMidfielder = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftMidfielder, random);
            CreatePlayerTactic(tacticID, leftMidfielder, PlayerPosition.LeftMidfielder, PlayerRole.WideMidfielder);
            assignedPlayerIDs.Add(leftMidfielder);

            var centralMidfielder1 = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightCenterMidfielder, random);
            CreatePlayerTactic(tacticID, centralMidfielder1, PlayerPosition.RightCenterMidfielder, PlayerRole.CentralMidfielder);
            assignedPlayerIDs.Add(centralMidfielder1);

            var centralMidfielder2 = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftCenterMidfielder, random);
            CreatePlayerTactic(tacticID, centralMidfielder2, PlayerPosition.LeftCenterMidfielder, PlayerRole.CentralMidfielder);
            assignedPlayerIDs.Add(centralMidfielder2);

            var rightMidfielder = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightMidfielder, random);
            CreatePlayerTactic(tacticID, rightMidfielder, PlayerPosition.RightMidfielder, PlayerRole.WideMidfielder);
            assignedPlayerIDs.Add(rightMidfielder);

            // 4. Assign Forwards (2 Strikers)
            var striker1 = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightStriker, random);
            CreatePlayerTactic(tacticID, striker1, PlayerPosition.RightStriker, PlayerRole.AdvancedForward);
            assignedPlayerIDs.Add(striker1);

            var striker2 = FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftStriker, random);
            CreatePlayerTactic(tacticID, striker2, PlayerPosition.LeftStriker, PlayerRole.AdvancedForward);
            assignedPlayerIDs.Add(striker2);
        }

        private Guid FindBestPlayerForPosition(List<Guid> teamPlayerIDs, HashSet<Guid> assignedPlayerIDs, PlayerPosition desiredPosition, Random random)
        {
            // Get all unassigned players from the team
            var availablePlayers = teamPlayerIDs.Where(id => !assignedPlayerIDs.Contains(id)).ToList();

            if (!availablePlayers.Any())
            {
                // This shouldn't happen with 30 players and 11 positions, but return random if it does
                return teamPlayerIDs[random.Next(teamPlayerIDs.Count)];
            }

            // Find players trained for this position by querying PlayerTrainedRoles
            var playersWithTrainedRoles = _context.PlayerTrainedRoles
                .Where(ptr => availablePlayers.Contains(ptr.PersonID) && ptr.PlayerPosition == desiredPosition)
                .OrderByDescending(ptr => ptr.PlayerTrainedRoleAdaptation)
                .ToList();

            if (playersWithTrainedRoles.Any())
            {
                // Return the player with the highest adaptation for this position
                return playersWithTrainedRoles.First().PersonID;
            }

            // If no player is trained for this position, return a random available player
            return availablePlayers[random.Next(availablePlayers.Count)];
        }

        private void CreatePlayerTactic(Guid tacticID, Guid personID, PlayerPosition position, PlayerRole role)
        {
            var playerTactic = new PlayerTactic
            {
                PlayerTacticID = Guid.NewGuid(),
                TacticID = tacticID,
                PersonID = personID,
                PlayerPosition = position,
                PlayerRole = role,
                SquadUnit = SquadUnit.Starting,
                SubstituteOrder = null
            };

            _context.PlayerTactics.Add(playerTactic);
        }
    }
}
