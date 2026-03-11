// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Teams;
using FootballOpenServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        public async Task<ActionResult<Team>> GetCurrentTeam()
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

            var squad = await _db.Players
                .Where(pl => _db.Contracts.Any(c =>
                    c.PersonID == pl.PersonID &&
                    c.TeamID == teamID &&
                    c.EndDate > DateTime.Now &&
                    c.Role == Role.Player))
                .Include(pl => pl.Person)
                .Include(pl => pl.PlayerTrainedPositions)
                .Select(pl => new
                {
                    pl.PlayerID,
                    Person = new
                    {
                        pl.Person!.Name,
                        pl.Person!.Surname,
                        pl.Person!.DateOfBirth,
                    },
                    PlayerTrainedPositions = pl.PlayerTrainedPositions!.Select(ptp => new
                    {
                        ptp.PlayerPosition,
                        ptp.PlayerTrainedPositionAdaptation
                    })
                })
                .ToListAsync();

            return Ok(squad);
        }

        [HttpGet("getPlayerDetails/{playerID}")]
        public async Task<IActionResult> GetPlayerDetails(Guid playerID)
        {
            var player = await _db.Players
                .Where(pl => pl.PlayerID == playerID)
                .Select(pl => new {
                    Person = new
                    {
                        pl.Person!.Name,
                        pl.Person!.Surname,
                        pl.Person!.DateOfBirth,
                        pl.Person!.PlaceOfBirth,
                        Contracts = pl.Person.Contracts
                            .OrderByDescending(c => c.EndDate)
                            .Select(c => new {
                                c.StartDate,
                                c.EndDate,
                                Team = new { c.Team.Name }
                            })
                    },
                    pl.PlayerStats,
                    pl.PlayerTrainedPositions,
                    pl.PlayerTrainedRoles
                })
                .FirstOrDefaultAsync();


            return Ok(player);
        }
    }
}
