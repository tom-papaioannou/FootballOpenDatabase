// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using FootballOpenServer.DTO.Teams;
using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Teams;
using FootballOpenServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly FootballDbContext _db;
        private readonly ITeamAccessService _teamAccessService;

        public TeamsController(FootballDbContext db, ITeamAccessService teamAccessService)
        {
            _db = db;
            _teamAccessService = teamAccessService;
        }

        [HttpGet("getCurrentTeam")]
        public async Task<ActionResult<TeamInformationDTO>> GetCurrentTeam()
        {
            var team = await _teamAccessService.GetOwnedTeamAsync(User);

            if (team == null)
            {
                return NotFound();
            }

            return Ok(team);
        }

        [HttpGet("{teamID}")]
        public async Task<ActionResult<Team>> GetTeam(Guid teamID)
        {
            var team = await _db.Teams
                .Include(t => t.Competitions)
                .FirstOrDefaultAsync(t => t.TeamID == teamID);

            if (team == null)
                return NotFound();

            return Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam([FromBody] Team team)
        {
            _db.Teams.Add(team);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTeam), new { teamID = team.TeamID }, team);
        }

        [HttpPut("{teamID}")]
        public async Task<IActionResult> UpdateTeam(Guid teamID, [FromBody] Team updatedTeam)
        {
            if (teamID != updatedTeam.TeamID)
                return BadRequest();

            var team = await _db.Teams.FindAsync(teamID);
            if (team == null)
                return NotFound();

            team.Name = updatedTeam.Name;

            _db.Entry(team).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("getTeamSquad/{teamID}")]
        public async Task<IActionResult> GetTeamSquad(Guid teamID)
        {
            var team = await _db.Teams.FirstOrDefaultAsync(t => t.TeamID == teamID);
            if (team == null)
            {
                return NotFound();
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var squad = await _db.Contracts
                .Where(c => c.TeamID == teamID && (c.EndDate == null || c.EndDate > today) && c.Role == Role.Player)
                .Include(c => c.Person)
                    .ThenInclude(p => p.PlayerTrainedPositions)
                .Include(c => c.Person)
                    .ThenInclude(p => p.PlayerTrainedRoles)
                .Select(c => new
                {
                    c.Person.PersonID,
                    c.Person.Name,
                    c.Person.Surname,
                    c.Person.DateOfBirth,
                    c.Person.NationID,
                    c.ShirtNumber,
                    c.Wage,
                    c.EndDate,
                    PlayerTrainedPositions = c.Person.PlayerTrainedPositions!.Select(ptp => new
                    {
                        ptp.PlayerPosition,
                        ptp.PlayerTrainedPositionAdaptation
                    }),
                    PlayerTrainedRoles = c.Person.PlayerTrainedRoles!.Select(ptr => new
                    {
                        ptr.PlayerPosition,
                        ptr.PlayerRole,
                        ptr.PlayerTrainedRoleAdaptation
                    })
                })
                .ToListAsync();

            return Ok(squad);
        }

        [HttpPut("updatePlayerShirtNumber")]
        public async Task<IActionResult> UpdatePlayerShirtNumber([FromBody] UpdatePlayerShirtNumberDTO model)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var playerContract = await _db.Contracts
                .FirstOrDefaultAsync(c =>
                    c.TeamID == model.TeamID &&
                    c.PersonID == model.PersonID &&
                    (c.EndDate == null || c.EndDate > today) &&
                    c.Role == Role.Player);

            if (playerContract == null)
            {
                return NotFound();
            }

            var previousAssignedContract = await _db.Contracts
                .FirstOrDefaultAsync(c =>
                    c.TeamID == model.TeamID &&
                    c.PersonID != model.PersonID &&
                    c.ShirtNumber == model.ShirtNumber &&
                    (c.EndDate == null || c.EndDate > today) &&
                    c.Role == Role.Player);

            var playerPreviousShirtNumber = playerContract.ShirtNumber;
            playerContract.ShirtNumber = model.ShirtNumber;

            if (previousAssignedContract != null)
            {
                previousAssignedContract.ShirtNumber = playerPreviousShirtNumber;
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("getPlayerDetails/{personID}")]
        public async Task<IActionResult> GetPlayerDetails(Guid personID)
        {
            var player = await _db.People
                .Where(p => p.PersonID == personID)
                .Select(p => new {
                    p.Name,
                    p.Surname,
                    p.DateOfBirth,
                    p.PlaceOfBirth,
                    p.NationID,
                    p.Weight,
                    p.Height,
                    Contracts = p.Contracts
                            .OrderByDescending(c => c.EndDate)
                            .Select(c => new {
                                c.StartDate,
                                c.EndDate,
                                c.Wage,
                                Team = new { c.Team.Name }
                            }),
                    p.PlayerStats,
                    p.PlayerTrainedPositions,
                    p.PlayerTrainedRoles
                })
                .FirstOrDefaultAsync();

            return Ok(player);
        }

        [HttpGet("getCurrentTeamDashboard")]
        public async Task<ActionResult<Team>> GetCurrentTeamDashboard()
        {
            var team = await _teamAccessService.GetOwnedTeamAsync(User);
            if (team == null)
            {
                return NotFound();
            }

            Competition competition = await _db.Competitions
                .Where(c => c.Teams.Any(t => t.TeamID == team.TeamID) && c.CompetitionType == CompetitionType.League)
                .FirstOrDefaultAsync();

            Person[] players = await _db.People
                .Where(p => _db.Contracts.Any(c => c.PersonID == p.PersonID && c.TeamID == team.TeamID && c.EndDate > DateOnly.FromDateTime(DateTime.UtcNow) && c.Role == Role.Player))
                .Include(p => p.PlayerTrainedPositions)
                .Take(10)
                .ToArrayAsync();

            Formation formation = await _db.Tactics
                .Where(t => t.TeamID == team.TeamID && t.isMain)
                .Select(t => t.Formation ?? Formation.None)
                .FirstOrDefaultAsync();

            var dashboardInformation = new
            {
                TeamName = team.Name,
                CompetitionID = competition?.CompetitionID,
                CompetitionName = competition?.CompetitionName ?? "No active league",
                Players = players,
                Formation = formation,
                Kit = team.Kit
            };

            return Ok(dashboardInformation);
        }
    }
}
