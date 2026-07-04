// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using SoccerOpenServer.DTO.Registration;
using SoccerOpenServer.DTO.Teams;
using SoccerOpenServer.Models.Servers;
using SoccerOpenServer.Models.Teams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SoccerOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServersController : ControllerBase
    {
        private readonly FootballDbContext _db;

        public ServersController(FootballDbContext db)
        {
            _db = db;
        }

        [HttpGet("getUserServer/{userID}")]
        [Authorize]
        public async Task<IActionResult> GetUserServer(Guid userID)
        {
            Guid serverID = await _db.AppUsers
                .Where(u => u.Id == userID)
                .Include(u => u.Person)
                .Select(u => u.Person.ServerID ?? new Guid())
                .FirstOrDefaultAsync();
            return Ok(serverID);
        }

        [HttpGet("getAllServers")]
        [Authorize]
        public async Task<IActionResult> GetAllServers()
        {
            return Ok(await _db.Servers.ToListAsync());
        }

        [HttpGet("joinable")]
        public async Task<ActionResult<IEnumerable<JoinableServerDTO>>> GetJoinableServers()
        {
            try
            {
                var servers = await _db.Competitions
                    .AsNoTracking()
                    .Where(c =>
                        c.Priority == 2 &&
                        c.Teams.Any(t => t.AppUserID == null))
                    .Select(c => new JoinableServerDTO
                    {
                        ServerID = c.ServerID ?? new Guid(),
                        Name = c.Server!.Name
                    })
                    .Distinct()
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                return Ok(servers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving joinable servers.");
            }
        }

        [HttpGet("{serverID}/nations")]
        public async Task<ActionResult<IEnumerable<RegistrationNationDTO>>> GetRegistrationNations(Guid serverID)
        {
            try
            {
                var serverExists = await _db.Servers.AnyAsync(s => s.ServerID == serverID);
                if (!serverExists)
                {
                    return NotFound();
                }

                var nations = await (
                    from competition in _db.Competitions.AsNoTracking()
                    join nation in _db.Nations.AsNoTracking()
                        on competition.NationID equals nation.NationID
                    from team in _db.Teams.AsNoTracking()
                    where competition.ServerID == serverID
                          && competition.Priority == 2
                          && team.AppUserID == null
                          && team.Competitions.Any(teamCompetition =>
                              teamCompetition.CompetitionID == competition.CompetitionID)
                    select new RegistrationNationDTO
                    {
                        NationID = nation.NationID,
                        Name = nation.Name,
                        ISO2 = nation.ISO2,
                        ISO3 = nation.ISO3,
                        FlagUrl = nation.FlagUrl
                    }
                )
                .Distinct()
                .OrderBy(n => n.Name)
                .ToListAsync();

                return Ok(nations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving registration nations.");
            }
        }

        [HttpGet("{serverID}/nations/{nationID}/registration-teams")]
        public async Task<ActionResult<IEnumerable<RegistrationTeamDTO>>> GetRegistrationTeams(Guid serverID, Guid nationID)
        {
            var serverExists = await _db.Servers.AnyAsync(s => s.ServerID == serverID);
            if (!serverExists)
            {
                return NotFound();
            }

            var nationExists = await _db.Nations.AnyAsync(n => n.NationID == nationID);
            if (!nationExists)
            {
                return NotFound();
            }

            var teams = await _db.Competitions
                .AsNoTracking()
                .Where(c =>
                    c.ServerID == serverID &&
                    c.NationID == nationID &&
                    c.Priority == 2)
                .SelectMany(c => c.Teams.Select(t => new RegistrationTeamDTO
                {
                    TeamID = t.TeamID,
                    Name = t.Name,
                    Code = t.Code,

                    CompetitionID = c.CompetitionID,
                    CompetitionName = c.CompetitionName,

                    NationID = c.NationID!.Value,
                    NationName = c.Nation!.Name,

                    IsAvailable = t.AppUserID == null,

                    BadgeUrl = null,

                    Stadium = t.Stadium == null
                        ? null
                        : new StadiumDTO
                        {
                            StadiumID = t.Stadium.StadiumID,
                            Name = t.Stadium.Name,
                            Capacity = t.Stadium.Capacity,
                            City = t.Stadium.City,
                            Latitude = t.Stadium.Latitude,
                            Longitude = t.Stadium.Longitude
                        },

                    Kit = t.Kit
                }))
                .OrderBy(t => t.CompetitionName)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return Ok(teams);
        }

        [HttpPost("createNewServer")]
        [Authorize]
        public async Task<IActionResult> CreateNewServer([FromBody] Server server)
        {
            try
            {
                await _db.Servers.AddAsync(server);
                await _db.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return BadRequest();
            } 
            return Ok(server);
        }

        [HttpGet("getServerInformation/{serverID}")]
        [Authorize]
        public async Task<IActionResult> GetServerInformation(Guid serverID)
        {
            Server server = await _db.Servers
                .AsNoTracking()
                .Include(s => s.Persons)
                .Include(s => s.Competitions).ThenInclude(c => c.Nation).AsSplitQuery()
                .Include(s => s.Competitions).ThenInclude(c => c.Continent).AsSplitQuery()
                .FirstOrDefaultAsync(s => s.ServerID == serverID);

            return Ok(server);
        }
    }
}
