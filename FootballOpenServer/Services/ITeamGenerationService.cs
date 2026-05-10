// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.Models.Teams;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.World;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenServer.Services
{
    public interface ITeamGenerationService
    {
       Task<List<Team>> GenerateTeamsForCompetition(Guid? serverID, Guid? nationID, int numberOfTeams = 20, int priority = 1);
       Task AssignPlayersToGeneratedTeams(IEnumerable<Guid> teamIDs);
    }

    public class TeamGenerationService : ITeamGenerationService
    {
        private static readonly Dictionary<string, NationGenerationData> GenerationDataByNation = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Greece"] = new(
                PriorityOneTeamNames:
                [
                    "Athinaikos FC", "Piraeus Harbor", "Thessalia Union", "Patraikos", "Crete Mariners",
                    "Olympia Stars", "Aegean Wave", "Macedonia Eagles", "Epirus Gate", "Sparta Forge",
                    "Rhodes Knights", "Larissa Storm", "Corinth Shield", "Delphi Oracle", "Volos Tide",
                    "Kavala North", "Arcadia Greens", "Ioannina Peak", "Kalamata Olive", "Naxos Blue"
                ],
                PriorityTwoTeamNames:
                [
                    "Attica Rovers", "Pella United", "Achaia Athletic", "Messinia Town", "Thrace Wanderers",
                    "Samos Harbor", "Chios Mariners", "Aitolia FC", "Laconia Stars", "Argos Vale",
                    "Kos Islanders", "Drama Falcons", "Xanthi Forge", "Serres Dynamo", "Preveza Coast",
                    "Trikala Union", "Korinthos City", "Mykonos Cyclones", "Paros Tide", "Lesvos Albion"
                ],
                FirstNames:
                [
                    "Giorgos", "Yannis", "Nikos", "Dimitris", "Kostas", "Panagiotis", "Vasilis", "Thanasis",
                    "Christos", "Andreas", "Stelios", "Manolis", "Sotiris", "Lefteris", "Michalis", "Spyros",
                    "Petros", "Antonis", "Theodoros", "Alexandros"
                ],
                LastNames:
                [
                    "Papadopoulos", "Nikolaidis", "Georgiou", "Dimitriou", "Konstantinou", "Ioannidis", "Vasileiou", "Christodoulou",
                    "Antonopoulos", "Theodorou", "Stavridis", "Manolakis", "Karagiannis", "Papanikolaou", "Kotsakis", "Roussos",
                    "Mavridis", "Tzimas", "Laskaridis", "Economou"
                ],
                Cities:
                [
                    "Athens", "Piraeus", "Thessaloniki", "Patras", "Heraklion",
                    "Larissa", "Volos", "Ioannina", "Kalamata", "Kavala"
                ]),
            ["England"] = new(
                PriorityOneTeamNames:
                [
                    "Northbridge FC", "London Borough", "Mersey Albion", "Yorkshire County", "Bristol Harbors",
                    "Westford United", "Eastmoor Town", "Kingsport Athletic", "Rivergate FC", "Lancaster Vale",
                    "Stonechester", "Southwick City", "Crown Anchor FC", "Redminster", "Oakfield Rovers",
                    "Brightmere", "Ironbridge FC", "Wellington Heath", "Norcastle", "Greenford Athletic"
                ],
                PriorityTwoTeamNames:
                [
                    "Ashford Town", "Derwent FC", "Midshire Athletic", "Portsmouth Vale", "Chesterfield Rovers",
                    "Blackpool Sands", "Reading Borough", "Trentham United", "Hartford City", "Windsor Albion",
                    "Somerset County", "Rutland FC", "Devonport Mariners", "Sunderland Forge", "Camden Wanderers",
                    "Oxford Heath", "Norwich Crown", "Plymouth Docks", "Durham Stars", "Canterbury Gate"
                ],
                FirstNames:
                [
                    "James", "Oliver", "George", "Harry", "Jack", "Charlie", "Thomas", "William",
                    "Henry", "Alfie", "Noah", "Finley", "Joshua", "Daniel", "Samuel", "Edward",
                    "Jacob", "Alexander", "Max", "Joseph"
                ],
                LastNames:
                [
                    "Smith", "Johnson", "Taylor", "Brown", "Wilson", "Evans", "Thomas", "Roberts",
                    "Walker", "White", "Hughes", "Edwards", "Green", "Hall", "Turner", "Carter",
                    "Phillips", "Mitchell", "Baker", "Campbell"
                ],
                Cities:
                [
                    "London", "Manchester", "Liverpool", "Birmingham", "Leeds",
                    "Bristol", "Newcastle", "Sheffield", "Nottingham", "Leicester"
                ]),
            ["Italy"] = new(
                PriorityOneTeamNames:
                [
                    "Roma Aurea", "Milano Navigli", "Torino Bulls", "Napoli Mare", "Firenze Viola",
                    "Genova Lanterns", "Bologna Towers", "Verona Arena", "Parma Ducale", "Palermo Sole",
                    "Bari Levante", "Pisa Mariners", "Modena Gialli", "Siena Stallions", "Trieste Port",
                    "Perugia Hill", "Cagliari Wind", "Salerno Granata", "Ravenna Pines", "Udine Stars"
                ],
                PriorityTwoTeamNames:
                [
                    "Lecce Barocco", "Como Lago", "Taranto Ionio", "Mantova Virgil", "Padova Veneto",
                    "Vicenza Bianco", "Livorno Porto", "Ferrara Este", "Ancona Adriatico", "Cosenza Rossa",
                    "Foggia Tavoliere", "Pescara Delfini", "Arezzo Rosso", "Lucca Mura", "Novara Piemonte",
                    "Cremona Violini", "Trapani Vento", "Messina Stretto", "Catanzaro Aquile", "Brescia Leonessa"
                ],
                FirstNames:
                [
                    "Luca", "Marco", "Giovanni", "Francesco", "Alessandro", "Matteo", "Andrea", "Giuseppe",
                    "Antonio", "Stefano", "Paolo", "Davide", "Simone", "Roberto", "Federico", "Lorenzo",
                    "Nicolo", "Salvatore", "Daniele", "Enrico"
                ],
                LastNames:
                [
                    "Rossi", "Russo", "Ferrari", "Esposito", "Bianchi", "Romano", "Colombo", "Ricci",
                    "Marino", "Greco", "Bruno", "Gallo", "Conti", "DeLuca", "Moretti", "Barbieri",
                    "Lombardi", "Fontana", "Caruso", "Vitale"
                ],
                Cities:
                [
                    "Rome", "Milan", "Naples", "Turin", "Florence",
                    "Genoa", "Bologna", "Verona", "Palermo", "Bari"
                ]),
            ["France"] = new(
                PriorityOneTeamNames:
                [
                    "Paris Lumiere", "Lyonnais FC", "Marseille Bleu", "Bordeaux Vignes", "Lille Nord",
                    "Nice Azur", "Nantes Loire", "Toulouse Garonne", "Monaco Rouge", "Rennes Armor",
                    "Strasbourg Etoile", "Montpellier Herault", "Grenoble Alpes", "Reims Champagne", "Saint Etienne Vert",
                    "Le Havre Ocean", "Caen Normand", "Metz Lorraine", "Lens Sang Or", "Dijon Bourgogne"
                ],
                PriorityTwoTeamNames:
                [
                    "Rouen Seine", "Tours Loire", "Amiens Picardie", "Brest Oceanique", "Angers Maine",
                    "Clermont Auvergne", "Nancy Lorraine", "Orleans Loiret", "Mulhouse Alsace", "Poitiers Vienne",
                    "Avignon Rhone", "Perpignan Catalan", "Limoges Porcelaine", "Annecy Lac", "Troyes Aube",
                    "Le Mans Sarthe", "Valence Drome", "Besancon Doubs", "Lorient Bretagne", "Pau Pyrenees"
                ],
                FirstNames:
                [
                    "Jean", "Pierre", "Michel", "Antoine", "Nicolas", "Julien", "Thomas", "Alexandre",
                    "Maxime", "Lucas", "Hugo", "Baptiste", "Adrien", "Arthur", "Mathis", "Quentin",
                    "Romain", "Florian", "Theo", "Victor"
                ],
                LastNames:
                [
                    "Martin", "Bernard", "Thomas", "Petit", "Robert", "Richard", "Durand", "Dubois",
                    "Moreau", "Laurent", "Simon", "Michel", "Lefevre", "Leroy", "Roux", "David",
                    "Bertrand", "Morel", "Fournier", "Girard"
                ],
                Cities:
                [
                    "Paris", "Lyon", "Marseille", "Bordeaux", "Lille",
                    "Nice", "Nantes", "Toulouse", "Strasbourg", "Montpellier"
                ]),
            ["Germany"] = new(
                PriorityOneTeamNames:
                [
                    "Berlin Adler", "Munich Isar", "Hamburg Harbor", "Cologne Dom", "Dortheim FC",
                    "Leipzig Roten", "Stuttgart Engine", "Bremen Weser", "Frankfurt Main", "Dresden Elbe",
                    "Hanover Horses", "Nuremberg Castle", "Essen Steel", "Kiel Baltic", "Freiburg Forest",
                    "Augsburg Gate", "Mainz Carnival", "Rostock Coast", "Bonn Capitals", "Wolfsburg Motors"
                ],
                PriorityTwoTeamNames:
                [
                    "Bochum Ruhr", "Karlsruhe Baden", "Lubeck Hanse", "Regensburg Danube", "Aachen Gate",
                    "Magdeburg Elbe", "Kassel Hessen", "Ulm Spatzen", "Bielefeld Armin", "Saarbrucken Coal",
                    "Jena Optics", "Erfurt Garden", "Potsdam Crown", "Chemnitz Forge", "Mannheim Harbor",
                    "Osnabruck Bridge", "Heidelberg Neckar", "Koblenz Rhine", "Furth Clover", "Oldenburg North"
                ],
                FirstNames:
                [
                    "Lukas", "Leon", "Felix", "Jonas", "Paul", "Maximilian", "Tim", "Julian",
                    "Nico", "Tobias", "Florian", "Marcel", "Daniel", "Christian", "Patrick", "Alexander",
                    "Stefan", "Martin", "Kevin", "Johannes"
                ],
                LastNames:
                [
                    "Muller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker",
                    "Hoffmann", "Schulz", "Koch", "Bauer", "Richter", "Klein", "Wolf", "Schroder",
                    "Neumann", "Schwarz", "Zimmermann", "Krause"
                ],
                Cities:
                [
                    "Berlin", "Munich", "Hamburg", "Cologne", "Frankfurt",
                    "Stuttgart", "Dortmund", "Leipzig", "Dresden", "Bremen"
                ]),
            ["United States"] = new(
                PriorityOneTeamNames:
                [
                    "Liberty FC", "Pacific Sound", "Chicago Blaze", "Texas Lone Stars", "Boston Harbor",
                    "Atlanta Peaks", "Seattle Emerald", "Phoenix Heat", "Denver Summit", "Miami Atlantic",
                    "Detroit Motors", "Nashville Rhythm", "Portland Pines", "San Diego Surf", "Dallas Rangers",
                    "Houston Comets", "Orlando Suns", "Cleveland Iron", "Minneapolis North", "Philadelphia Bell"
                ],
                PriorityTwoTeamNames:
                [
                    "Sacramento Gold", "Charlotte Crown", "Columbus Crewmen", "St Louis Gateway", "Cincinnati River",
                    "Tampa Bay Storm", "Baltimore Forge", "Austin Oaks", "Las Vegas Lights", "Kansas City Plains",
                    "Indianapolis Circle", "Pittsburgh Steel", "Raleigh Capital", "Milwaukee Lake", "Memphis Blues",
                    "New Orleans Jazz", "Salt Lake Peaks", "Richmond United", "Buffalo Snow", "Albuquerque Sol"
                ],
                FirstNames:
                [
                    "Michael", "Christopher", "Matthew", "Joshua", "Andrew", "David", "John", "Joseph",
                    "Anthony", "Nicholas", "Tyler", "Brandon", "Ryan", "Justin", "Kevin", "Jason",
                    "Zachary", "Christian", "Austin", "Logan"
                ],
                LastNames:
                [
                    "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
                    "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Moore",
                    "Jackson", "Martin", "Lee", "Perez"
                ],
                Cities:
                [
                    "New York", "Los Angeles", "Chicago", "Houston", "Phoenix",
                    "Philadelphia", "San Antonio", "San Diego", "Dallas", "Seattle"
                ]),
            ["Canada"] = new(
                PriorityOneTeamNames:
                [
                    "Toronto Maple FC", "Montreal Nord", "Vancouver Tides", "Ottawa Capital", "Calgary Peaks",
                    "Edmonton Aurora", "Winnipeg Plains", "Quebec Citadelle", "Halifax Harbor", "Saskatoon Prairie",
                    "Hamilton Steel", "Victoria Island", "London Ontario FC", "Regina Crown", "Kelowna Lake",
                    "Moncton Acadie", "Sherbrooke Rouge", "Sudbury Nickel", "Kingston Limestone", "Saint Johns Atlantic"
                ],
                PriorityTwoTeamNames:
                [
                    "Trois Rivieres Bleu", "Laval Metro", "Brandon Wheat", "Red Deer Summit", "Whitehorse North",
                    "Yellowknife Aurora", "Prince George Timber", "Kamloops Heat", "Windsor Border", "Gatineau Hull",
                    "Sarnia Rapids", "Moose Jaw Prairie", "Thunder Bay Lake", "Charlottetown Crown", "Fredericton River",
                    "North Bay Trappers", "St Catharines Canal", "Medicine Hat Gas", "Lethbridge Foothills", "Saguenay Fjord"
                ],
                FirstNames:
                [
                    "Liam", "Noah", "Ethan", "Lucas", "William", "Benjamin", "Logan", "Nathan",
                    "Samuel", "Jacob", "Thomas", "Charles", "Owen", "Jack", "Adam", "Julien",
                    "Antoine", "Gabriel", "Felix", "Connor"
                ],
                LastNames:
                [
                    "Smith", "Tremblay", "Gagnon", "Roy", "Cote", "Bouchard", "Martin", "Lefebvre",
                    "Wilson", "Johnson", "Campbell", "MacDonald", "Anderson", "Clark", "Reid", "Stewart",
                    "Fraser", "Murray", "Levesque", "Brown"
                ],
                Cities:
                [
                    "Toronto", "Montreal", "Vancouver", "Calgary", "Ottawa",
                    "Edmonton", "Winnipeg", "Quebec City", "Halifax", "Victoria"
                ]),
            ["Mexico"] = new(
                PriorityOneTeamNames:
                [
                    "Azteca Sur", "Monterrey Cerro", "Guadalajara Sol", "Puebla Blanca", "Tijuana Frontera",
                    "Veracruz Puerto", "Merida Maya", "Toluca Nevado", "Leon Bajio", "Queretaro Campanas",
                    "Oaxaca Monte", "Cancun Caribe", "Chiapas Selva", "Juarez Norte", "Culiacan Pacifico",
                    "Aguascalientes Ferro", "Morelia Cantera", "Durango Sierra", "Tepic Nayar", "San Luis Altiplano"
                ],
                PriorityTwoTeamNames:
                [
                    "Zacatecas Plata", "Tampico Marea", "Celaya Toros", "Irapuato Fresa", "Mazatlan Ola",
                    "Villahermosa Grijalva", "Colima Fuego", "Cuernavaca Primavera", "Tlaxcala Volcan", "Campeche Muralla",
                    "La Paz Baja", "Hermosillo Desierto", "Torreon Laguna", "Pachuca Mineros", "Ensenada Costa",
                    "Matamoros Frontera", "Saltillo Norte", "Tuxtla Jaguar", "Chihuahua Sierra", "Acapulco Dorado"
                ],
                FirstNames:
                [
                    "Jose", "Juan", "Luis", "Carlos", "Miguel", "Alejandro", "Francisco", "Diego",
                    "Ricardo", "Antonio", "Manuel", "Fernando", "Jesus", "Roberto", "Pedro", "Javier",
                    "Raul", "Hector", "Andres", "Emiliano"
                ],
                LastNames:
                [
                    "Hernandez", "Gonzalez", "Lopez", "Martinez", "Rodriguez", "Perez", "Sanchez", "Ramirez",
                    "Cruz", "Flores", "Gomez", "Diaz", "Reyes", "Morales", "Ortiz", "Castillo",
                    "Rojas", "Navarro", "Vargas", "Mendoza"
                ],
                Cities:
                [
                    "Mexico City", "Guadalajara", "Monterrey", "Puebla", "Tijuana",
                    "Merida", "Veracruz", "Leon", "Queretaro", "Juarez"
                ]),
            ["Argentina"] = new(
                PriorityOneTeamNames:
                [
                    "Buenos Aires Sur", "Cordoba Central", "Rosario Norte", "Mendoza Andes", "La Plata Estrella",
                    "Mar del Plata Port", "Tucuman Azucar", "Salta Valles", "Santa Fe Union", "Neuquen Patagonia",
                    "Bahia Blanca Wind", "San Juan Cuyo", "Corrientes River", "Posadas Misiones", "Parana Litoral",
                    "Jujuy Altura", "San Luis Sierra", "Comodoro Coast", "Rio Cuarto Celeste", "Catamarca Sol"
                ],
                PriorityTwoTeamNames:
                [
                    "Chaco Rojo", "Formosa Norte", "Trelew Patagonia", "Rawson Atlántico", "Junin Verde",
                    "Olavarria Cemento", "Rafaela Leche", "Moron Oeste", "Quilmes Cerveceros", "Tandil Sierras",
                    "San Rafael Sur", "Concordia Rio", "Gualeguay Blue", "Pergamino Fields", "Resistencia Litoral",
                    "Villa Maria Unido", "Rio Gallegos Hielo", "Ushuaia Austral", "Santiago Estero Sol", "La Rioja Andina"
                ],
                FirstNames:
                [
                    "Juan", "Bautista", "Santiago", "Matias", "Nicolas", "Franco", "Agustin", "Tomas",
                    "Facundo", "Lautaro", "Luciano", "Martin", "Diego", "Federico", "Gonzalo", "Ramiro",
                    "Emiliano", "Ignacio", "Bruno", "Maximo"
                ],
                LastNames:
                [
                    "Gonzalez", "Rodriguez", "Gomez", "Fernandez", "Lopez", "Diaz", "Martinez", "Perez",
                    "Garcia", "Sanchez", "Romero", "Sosa", "Alvarez", "Torres", "Ruiz", "Suarez",
                    "Acosta", "Castro", "Medina", "Ortiz"
                ],
                Cities:
                [
                    "Buenos Aires", "Cordoba", "Rosario", "Mendoza", "La Plata",
                    "Mar del Plata", "Tucuman", "Salta", "Santa Fe", "Neuquen"
                ]),
            ["Brazil"] = new(
                PriorityOneTeamNames:
                [
                    "Rio Carioca", "Sao Paulo Paulista", "Belo Horizonte Mineiro", "Porto Alegre Sul", "Salvador Bahia",
                    "Recife Coral", "Fortaleza Sol", "Curitiba Pinheiros", "Manaus Amazonia", "Goiania Cerrado",
                    "Santos Praia", "Belem Norte", "Campinas Ponte", "Natal Dunas", "Florianopolis Ilha",
                    "Cuiaba Pantanal", "Vitoria Capixaba", "Maceio Mar", "Joao Pessoa Litoral", "Brasilia Planalto"
                ],
                PriorityTwoTeamNames:
                [
                    "Niteroi Guanabara", "Sao Luis Reggae", "Aracaju Sergipe", "Londrina Cafe", "Joinville Norte",
                    "Ribeirao Preto Interior", "Uberlandia Triangulo", "Pelotas Gaucho", "Juiz de Fora Zona", "Caxias Serra",
                    "Bauru Central", "Macapa Equator", "Boa Vista Branco", "Porto Velho Madeira", "Teresina Piaui",
                    "Petrolina Sao Francisco", "Caruaru Agreste", "Sorocaba Ferro", "Maringa Verde", "Uberaba Minas"
                ],
                FirstNames:
                [
                    "Joao", "Gabriel", "Pedro", "Lucas", "Matheus", "Rafael", "Bruno", "Felipe",
                    "Guilherme", "Thiago", "Andre", "Leonardo", "Caio", "Gustavo", "Daniel", "Henrique",
                    "Victor", "Diego", "Igor", "Vinicius"
                ],
                LastNames:
                [
                    "Silva", "Santos", "Oliveira", "Souza", "Rodrigues", "Ferreira", "Alves", "Pereira",
                    "Lima", "Gomes", "Ribeiro", "Carvalho", "Almeida", "Monteiro", "Melo", "Araujo",
                    "Costa", "Nascimento", "Barbosa", "Rocha"
                ],
                Cities:
                [
                    "Sao Paulo", "Rio de Janeiro", "Belo Horizonte", "Porto Alegre", "Salvador",
                    "Recife", "Fortaleza", "Curitiba", "Manaus", "Brasilia"
                ])
        };

        private sealed record NationGenerationData(
            string[] PriorityOneTeamNames,
            string[] PriorityTwoTeamNames,
            string[] FirstNames,
            string[] LastNames,
            string[] Cities)
        {
            public string[] GetTeamNamesForPriority(int priority) => priority == 2 ? PriorityTwoTeamNames : PriorityOneTeamNames;
        }

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

        public async Task<List<Team>> GenerateTeamsForCompetition(Guid? serverID, Guid? nationID, int numberOfTeams = 20, int priority = 1)
        {
            var teams = new List<Team>();
            var random = new Random();

            var nations = await _context.Nations.ToListAsync();
            var nationsById = nations.ToDictionary(n => n.NationID);
            var targetNation = nations.FirstOrDefault(n => n.NationID == nationID);
            var teamGenerationData = GetGenerationData(targetNation);
            var restNations = nations.Where(n => n.NationID != nationID).ToList();

            // Create a shuffled copy of team names to avoid duplicates
            var availableNames = teamGenerationData.GetTeamNamesForPriority(priority).ToList();
            
            for (int i = 0; i < numberOfTeams && availableNames.Count > 0; i++)
            {
                // Pick a random name from available names
                var randomIndex = random.Next(availableNames.Count);
                var teamName = availableNames[randomIndex];
                
                // Remove the name from available names to prevent duplicates
                availableNames.RemoveAt(randomIndex);

                Stadium stadium = new Stadium
                {
                    StadiumID = Guid.NewGuid(),
                    Name = $"{teamName} Stadium",
                    Capacity = random.Next(20000, 80001), // Random capacity between 20,000 and 80,000
                    Latitude = random.NextDouble() * 180 - 90, // Random latitude between -90 and 90
                    Longitude = random.NextDouble() * 360 - 180, // Random longitude between -180 and 180
                    City = teamGenerationData.Cities[random.Next(teamGenerationData.Cities.Length)]
                };

                _context.Add(stadium);

                Kit kit = new Kit
                {
                    KitID = Guid.NewGuid(),
                    HomeShirtColor = $"#{random.Next(0x1000000):X6}",
                    HomeShortsColor = $"#{random.Next(0x1000000):X6}",
                    AwayShirtColor = $"#{random.Next(0x1000000):X6}",
                    AwayShortsColor = $"#{random.Next(0x1000000):X6}"
                };

                _context.Kits.Add(kit);

                var team = new Team
                {
                    TeamID = Guid.NewGuid(),
                    Name = teamName,
                    Competitions = new List<Competition>(),
                    Contracts = new List<Contract>(),
                    Code = BuildTeamCode(teamName),
                    StadiumID = stadium.StadiumID,
                    KitID = kit.KitID
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
                var teamPlayerStats = new List<PlayerStats>();

                List<byte> teamShirtNumbersAssigned = new List<byte>();
                var requiredPrimaryPositions = BuildRequiredPrimaryPlayerPositions(random);

                // Generate 30 people for each team
                for (int j = 0; j < 30; j++)
                {
                    var personID = Guid.NewGuid();
                    var assignedNationID = nationID;
                    if (restNations.Count > 0 && random.Next(0, 10) >= 7)
                    {
                        assignedNationID = restNations[random.Next(restNations.Count)].NationID;
                    }

                    Nation? assignedNation = null;
                    if (assignedNationID.HasValue)
                    {
                        nationsById.TryGetValue(assignedNationID.Value, out assignedNation);
                    }
                    var personGenerationData = GetGenerationData(assignedNation ?? targetNation);

                    // Create Person
                    var person = new Person
                    {
                        PersonID = personID,
                        Name = personGenerationData.FirstNames[random.Next(personGenerationData.FirstNames.Length)],
                        Surname = personGenerationData.LastNames[random.Next(personGenerationData.LastNames.Length)],
                        DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-random.Next(18, 35)).AddDays(-random.Next(0, 365))),
                        PlaceOfBirth = personGenerationData.Cities[random.Next(personGenerationData.Cities.Length)],
                        NationID = assignedNationID,
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
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        EndDate = new DateOnly(contractEndYear, 6, 30),
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
                    var requiredPrimaryPosition = j < requiredPrimaryPositions.Count ? requiredPrimaryPositions[j] : (PlayerPosition?)null;
                    var trainedPositions = GeneratePlayerTrainedPositions(random, personID, requiredPrimaryPosition);
                    
                    // Generate PlayerTrainedRoles for each PlayerTrainedPosition
                    var trainedRoles = new List<PlayerTrainedRole>();
                    foreach (var trainedPosition in trainedPositions)
                    {
                        var rolesForPosition = GeneratePlayerTrainedRoles(random, personID, trainedPosition.PlayerPosition);
                        trainedRoles.AddRange(rolesForPosition);
                    }

                    byte shirtNumber = (byte)random.Next(1, 100);

                    if(teamShirtNumbersAssigned.Contains(shirtNumber))
                    {
                        // If shirt number is already assigned, find the next available one
                        shirtNumber = 1;
                        while (teamShirtNumbersAssigned.Contains(shirtNumber) && shirtNumber < 99)
                        {
                            shirtNumber++;
                        }
                    }

                    teamShirtNumbersAssigned.Add(shirtNumber);
                    contract.ShirtNumber = shirtNumber;

                    // Add entities to database context
                    _context.People.Add(person);
                    _context.Contracts.Add(contract);
                    _context.PlayerStats.Add(playerStats);
                    _context.PlayerTrainedPositions.AddRange(trainedPositions);
                    _context.PlayerTrainedRoles.AddRange(trainedRoles);

                    // Store player ID for position assignment
                    teamPlayerIDs.Add(personID);
                    teamPlayerStats.Add(playerStats);
                }

                AssignBestTacticSpecialists(primaryTactic, teamPlayerStats);

                teams.Add(team);
            }

            return teams;
        }

        private static void AssignBestTacticSpecialists(Tactic tactic, IReadOnlyCollection<PlayerStats> teamPlayerStats)
        {
            tactic.CaptainID = GetBestCaptainID(teamPlayerStats);
            tactic.PenaltyTakerID = GetBestPenaltyTakerID(teamPlayerStats);

            Guid? bestCornerTakerID = GetBestCornerTakerID(teamPlayerStats);
            tactic.LeftCornerTakerID = bestCornerTakerID;
            tactic.RightCornerTakerID = bestCornerTakerID;
        }

        private static Guid? GetBestCaptainID(IReadOnlyCollection<PlayerStats> teamPlayerStats)
        {
            return GetBestPlayerID(teamPlayerStats, stats => (stats.Decisions + stats.Teamwork) / 2.0);
        }

        private static Guid? GetBestPenaltyTakerID(IReadOnlyCollection<PlayerStats> teamPlayerStats)
        {
            return GetBestPlayerID(teamPlayerStats, stats =>
                (stats.Shooting + stats.Kicking + stats.Decisions + stats.Strength) / 4.0);
        }

        private static Guid? GetBestCornerTakerID(IReadOnlyCollection<PlayerStats> teamPlayerStats)
        {
            return GetBestPlayerID(teamPlayerStats, stats =>
                (stats.Crossing + stats.Kicking + stats.Teamwork + stats.Decisions + stats.Strength) / 5.0);
        }

        private static Guid? GetBestPlayerID(IReadOnlyCollection<PlayerStats> teamPlayerStats, Func<PlayerStats, double> scoreSelector)
        {
            return teamPlayerStats
                .OrderByDescending(scoreSelector)
                .Select(stats => (Guid?)stats.PersonID)
                .FirstOrDefault();
        }

        private static NationGenerationData GetGenerationData(Nation? nation)
        {
            if (nation != null && GenerationDataByNation.TryGetValue(nation.Name, out var nationGenerationData))
            {
                return nationGenerationData;
            }

            if (GenerationDataByNation.Values.FirstOrDefault() is { } fallbackGenerationData)
            {
                return fallbackGenerationData;
            }

            throw new InvalidOperationException("No nation generation data has been configured.");
        }

        private static string BuildTeamCode(string teamName)
        {
            var sanitizedName = new string(teamName
                .Where(char.IsLetterOrDigit)
                .Select(char.ToUpperInvariant)
                .ToArray());

            if (string.IsNullOrWhiteSpace(sanitizedName))
            {
                var fallbackSeed = teamName.Aggregate(17L, (current, character) => (current * 31 + character) % 1000);
                return $"T{fallbackSeed:D3}";
            }

            return sanitizedName.Length >= 4
                ? sanitizedName[..4]
                : sanitizedName.PadRight(4, 'X');
        }

        private static List<PlayerPosition> BuildRequiredPrimaryPlayerPositions(Random random)
        {
            var positions = new List<PlayerPosition>();

            AddRepeatedPositions(positions, PlayerPosition.Goalkeeper, 2);
            AddRepeatedPositions(positions, PlayerPosition.RightBack, 2);
            AddRandomPositions(positions, random, 3, PlayerPosition.LeftCenterBack, PlayerPosition.CentralCenterBack, PlayerPosition.RightCenterBack);
            AddRepeatedPositions(positions, PlayerPosition.LeftBack, 2);
            AddRandomPositions(positions, random, 3, PlayerPosition.LeftCenterMidfielder, PlayerPosition.CentralCenterMidfielder, PlayerPosition.RightCenterMidfielder);
            AddRepeatedPositions(positions, PlayerPosition.LeftMidfielder, 2);
            AddRepeatedPositions(positions, PlayerPosition.RightMidfielder, 2);
            AddRandomPositions(positions, random, 3, PlayerPosition.LeftStriker, PlayerPosition.CentralStriker, PlayerPosition.RightStriker);

            return positions;
        }

        private static void AddRepeatedPositions(List<PlayerPosition> positions, PlayerPosition position, int count)
        {
            for (int i = 0; i < count; i++)
            {
                positions.Add(position);
            }
        }

        private static void AddRandomPositions(List<PlayerPosition> positions, Random random, int count, params PlayerPosition[] candidates)
        {
            for (int i = 0; i < count; i++)
            {
                positions.Add(candidates[random.Next(candidates.Length)]);
            }
        }

        private List<PlayerTrainedPosition> GeneratePlayerTrainedPositions(Random random, Guid personID, PlayerPosition? primaryPosition = null)
        {
            var trainedPositions = new List<PlayerTrainedPosition>();
            
            // Get all valid player positions (exclude None)
            var validPositions = Enum.GetValues(typeof(PlayerPosition))
                .Cast<PlayerPosition>()
                .Where(p => p != PlayerPosition.None)
                .ToList();

            // First trained position (80-100 adaptaption)
            var firstPosition = primaryPosition ?? validPositions[random.Next(validPositions.Count)];
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
            
            var validRoles = GetValidRolesForPosition(position);

            if (!validRoles.Any())
            {
                return trainedRoles;
            }

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

        private static List<PlayerRole> GetValidRolesForPosition(PlayerPosition position)
        {
            return position switch
            {
                PlayerPosition.Goalkeeper => new List<PlayerRole>
                {
                    PlayerRole.Goalkeeper,
                    PlayerRole.SweeperKeeper
                },

                PlayerPosition.RightCenterBack or
                PlayerPosition.CentralCenterBack or
                PlayerPosition.LeftCenterBack => new List<PlayerRole>
                {
                    PlayerRole.CenterBack,
                    PlayerRole.BallPlayingDefender,
                    PlayerRole.NoNonsenseCenterBack,
                    PlayerRole.Libero,
                    PlayerRole.Stopper,
                    PlayerRole.Cover
                },

                PlayerPosition.RightBack or
                PlayerPosition.LeftBack or
                PlayerPosition.RightWingBack or
                PlayerPosition.LeftWingBack => new List<PlayerRole>
                {
                    PlayerRole.FullBack,
                    PlayerRole.WingBack,
                    PlayerRole.CompleteWingBack,
                    PlayerRole.InvertedWingBack,
                    PlayerRole.WideCenterBack
                },

                PlayerPosition.RightDefensiveMidfielder or
                PlayerPosition.CentralDefensiveMidfielder or
                PlayerPosition.LeftDefensiveMidfielder => new List<PlayerRole>
                {
                    PlayerRole.DefensiveMidfielder,
                    PlayerRole.Anchorman,
                    PlayerRole.HalfBack,
                    PlayerRole.DeepLyingPlaymaker,
                    PlayerRole.Regista,
                    PlayerRole.Volante,
                    PlayerRole.SegundoVolante,
                    PlayerRole.BallWinningMidfielder
                },

                PlayerPosition.RightCenterMidfielder or
                PlayerPosition.CentralCenterMidfielder or
                PlayerPosition.LeftCenterMidfielder => new List<PlayerRole>
                {
                    PlayerRole.CentralMidfielder,
                    PlayerRole.BoxToBoxMidfielder,
                    PlayerRole.Mezzala,
                    PlayerRole.Carrilero,
                    PlayerRole.AdvancedPlaymaker,
                    PlayerRole.RoamingPlaymaker
                },

                PlayerPosition.RightMidfielder or
                PlayerPosition.LeftMidfielder or
                PlayerPosition.RightWinger or
                PlayerPosition.LeftWinger => new List<PlayerRole>
                {
                    PlayerRole.WideMidfielder,
                    PlayerRole.WidePlaymaker,
                    PlayerRole.Winger,
                    PlayerRole.InvertedWinger,
                    PlayerRole.InsideForward,
                    PlayerRole.InvertedForward,
                    PlayerRole.Raumdeuter,
                    PlayerRole.WideTargetMan,
                    PlayerRole.DefensiveWinger
                },

                PlayerPosition.RightAttackingMidfielder or
                PlayerPosition.CentralAttackingMidfielder or
                PlayerPosition.LeftAttackingMidfielder => new List<PlayerRole>
                {
                    PlayerRole.AttackingMidfielder,
                    PlayerRole.ShadowStriker,
                    PlayerRole.Enganche,
                    PlayerRole.Trequartista,
                    PlayerRole.SecondStriker,
                    PlayerRole.FalseTen,
                    PlayerRole.CentralWinger
                },

                PlayerPosition.RightStriker or
                PlayerPosition.CentralStriker or
                PlayerPosition.LeftStriker => new List<PlayerRole>
                {
                    PlayerRole.AdvancedForward,
                    PlayerRole.CompleteForward,
                    PlayerRole.Poacher,
                    PlayerRole.TargetMan,
                    PlayerRole.DeepLyingForward,
                    PlayerRole.PressingForward,
                    PlayerRole.DefensiveForward,
                    PlayerRole.FalseNine,
                    PlayerRole.TrequartistaForward
                },

                _ => new List<PlayerRole>()
            };
        }

        public async Task AssignPlayersToGeneratedTeams(IEnumerable<Guid> teamIDs)
        {
            var generatedTeamIDs = teamIDs.Distinct().ToList();
            if (!generatedTeamIDs.Any())
            {
                return;
            }

            var random = new Random();
            var tactics = await _context.Tactics
                .Where(t => generatedTeamIDs.Contains(t.TeamID) && t.isMain)
                .ToListAsync();

            foreach (var tactic in tactics)
            {
                var teamPlayerIDs = await _context.Contracts
                    .Where(c => c.TeamID == tactic.TeamID && c.Role == Role.Player)
                    .Select(c => c.PersonID)
                    .ToListAsync();

                var assignedPlayerIDs = await AssignPlayersToFormation(tactic.TacticID, teamPlayerIDs, tactic.Formation, random);
                AssignSubstitutionsAndReserves(tactic.TacticID, teamPlayerIDs, assignedPlayerIDs);
            }
        }

        private async Task<HashSet<Guid>> AssignPlayersToFormation(Guid tacticID, List<Guid> teamPlayerIDs, Formation? formation, Random random)
        {
            if (formation == Formation.Four_Four_Two)
            {
                return await AssignPlayersToFourFourTwo(tacticID, teamPlayerIDs, random);
            }

            return new HashSet<Guid>();
            // Add more formations here as needed
        }

        private void AssignSubstitutionsAndReserves(Guid tacticID, List<Guid> teamPlayerIDs, HashSet<Guid> assignedPlayerIDs)
        {
            var availablePlayers = teamPlayerIDs.Where(id => !assignedPlayerIDs.Contains(id)).ToList();
            int i = 1;
            foreach (Guid availablePlayerID in availablePlayers)
            {
                if (i <= 9)
                {
                    AddSubstitution(tacticID, availablePlayerID, i);
                }
                else
                {
                    AddReserve(tacticID, availablePlayerID);
                }

                i++;
            }
        }

        private void AddSubstitution(Guid tacticID, Guid personID, int substituteOrder)
        {
            var playerTactic = new PlayerTactic
            {
                PlayerTacticID = Guid.NewGuid(),
                TacticID = tacticID,
                PersonID = personID,
                PlayerPosition = PlayerPosition.None,
                PlayerRole = PlayerRole.None,
                SquadUnit = SquadUnit.Substitute,
                SubstituteOrder = substituteOrder
            };
            _context.PlayerTactics.Add(playerTactic);
        }

        private void AddReserve(Guid tacticID, Guid personID)
        {
            var playerTactic = new PlayerTactic
            {
                PlayerTacticID = Guid.NewGuid(),
                TacticID = tacticID,
                PersonID = personID,
                PlayerPosition = PlayerPosition.None,
                PlayerRole = PlayerRole.None,
                SquadUnit = SquadUnit.Reserve,
                SubstituteOrder = null
            };
            _context.PlayerTactics.Add(playerTactic);
        }

        private async Task<HashSet<Guid>> AssignPlayersToFourFourTwo(Guid tacticID, List<Guid> teamPlayerIDs, Random random)
        {
            var assignedPlayerIDs = new HashSet<Guid>();

            // 1. Assign Goalkeeper
            var goalkeeper = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.Goalkeeper, random);
            CreatePlayerTactic(tacticID, goalkeeper, PlayerPosition.Goalkeeper, PlayerRole.Goalkeeper);
            assignedPlayerIDs.Add(goalkeeper);

            // 2. Assign Defenders (1 Left, 2 Center, 1 Right)
            var leftBack = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftBack, random);
            CreatePlayerTactic(tacticID, leftBack, PlayerPosition.LeftBack, PlayerRole.FullBack);
            assignedPlayerIDs.Add(leftBack);

            var centerBack1 = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightCenterBack, random);
            CreatePlayerTactic(tacticID, centerBack1, PlayerPosition.RightCenterBack, PlayerRole.CenterBack);
            assignedPlayerIDs.Add(centerBack1);

            var centerBack2 = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftCenterBack, random);
            CreatePlayerTactic(tacticID, centerBack2, PlayerPosition.LeftCenterBack, PlayerRole.CenterBack);
            assignedPlayerIDs.Add(centerBack2);

            var rightBack = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightBack, random);
            CreatePlayerTactic(tacticID, rightBack, PlayerPosition.RightBack, PlayerRole.FullBack);
            assignedPlayerIDs.Add(rightBack);

            // 3. Assign Midfielders (1 Left, 2 Center, 1 Right)
            var leftMidfielder = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftMidfielder, random);
            CreatePlayerTactic(tacticID, leftMidfielder, PlayerPosition.LeftMidfielder, PlayerRole.WideMidfielder);
            assignedPlayerIDs.Add(leftMidfielder);

            var centralMidfielder1 = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightCenterMidfielder, random);
            CreatePlayerTactic(tacticID, centralMidfielder1, PlayerPosition.RightCenterMidfielder, PlayerRole.CentralMidfielder);
            assignedPlayerIDs.Add(centralMidfielder1);

            var centralMidfielder2 = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftCenterMidfielder, random);
            CreatePlayerTactic(tacticID, centralMidfielder2, PlayerPosition.LeftCenterMidfielder, PlayerRole.CentralMidfielder);
            assignedPlayerIDs.Add(centralMidfielder2);

            var rightMidfielder = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightMidfielder, random);
            CreatePlayerTactic(tacticID, rightMidfielder, PlayerPosition.RightMidfielder, PlayerRole.WideMidfielder);
            assignedPlayerIDs.Add(rightMidfielder);

            // 4. Assign Forwards (2 Strikers)
            var striker1 = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.RightStriker, random);
            CreatePlayerTactic(tacticID, striker1, PlayerPosition.RightStriker, PlayerRole.AdvancedForward);
            assignedPlayerIDs.Add(striker1);

            var striker2 = await FindBestPlayerForPosition(teamPlayerIDs, assignedPlayerIDs, PlayerPosition.LeftStriker, random);
            CreatePlayerTactic(tacticID, striker2, PlayerPosition.LeftStriker, PlayerRole.AdvancedForward);
            assignedPlayerIDs.Add(striker2);

            return assignedPlayerIDs;
        }

        private async Task<Guid> FindBestPlayerForPosition(List<Guid> teamPlayerIDs, HashSet<Guid> assignedPlayerIDs, PlayerPosition desiredPosition, Random random)
        {
            // Get all unassigned players from the team
            var availablePlayers = teamPlayerIDs.Where(id => !assignedPlayerIDs.Contains(id)).ToList();

            if (!availablePlayers.Any())
            {
                // This shouldn't happen with 30 players and 11 positions, but return random if it does
                return teamPlayerIDs[random.Next(teamPlayerIDs.Count)];
            }

            var playersWithTrainedPositions = await _context.PlayerTrainedPositions
                .Where(ptp => availablePlayers.Contains(ptp.PersonID) && ptp.PlayerPosition == desiredPosition)
                .OrderByDescending(ptp => ptp.PlayerTrainedPositionAdaptation)
                .ToListAsync();


            if (playersWithTrainedPositions.Any())
            {
                return playersWithTrainedPositions.First().PersonID;
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
