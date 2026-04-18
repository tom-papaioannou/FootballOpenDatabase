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

            var squad = await _db.People
                .Where(p => _db.Contracts.Any(c => c.PersonID == p.PersonID && c.TeamID == teamID && c.EndDate > DateTime.Now && c.Role == Role.Player))
                .Include(p => p.PlayerTrainedPositions)
                .Select(p => new
                {
                    p.PersonID,
                    p!.Name,
                    p!.Surname,
                    p!.DateOfBirth,
                    PlayerTrainedPositions = p.PlayerTrainedPositions!.Select(ptp => new
                    {
                        ptp.PlayerPosition,
                        ptp.PlayerTrainedPositionAdaptation
                    })
                })
                .ToListAsync();

            return Ok(squad);
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
                    Contracts = p.Contracts
                            .OrderByDescending(c => c.EndDate)
                            .Select(c => new {
                                c.StartDate,
                                c.EndDate,
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
                .Where(p => _db.Contracts.Any(c => c.PersonID == p.PersonID && c.TeamID == team.TeamID && c.EndDate > DateTime.Now && c.Role == Role.Player))
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
                CompetitionName = competition?.CompetitionName ?? "No active league",
                Players = players,
                Formation = formation
            };

            return Ok(dashboardInformation);
        }
    }
}
