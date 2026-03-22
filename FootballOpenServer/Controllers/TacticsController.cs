// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using FootballOpenServer.Models.Teams;
using FootballOpenServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TacticsController : ControllerBase
    {
        private FootballDbContext _db;
        private readonly ITeamAccessService _teamAccessService;

        public TacticsController(FootballDbContext db, ITeamAccessService teamAccessService)
        {
            _db = db;
            _teamAccessService = teamAccessService;
        }

        [HttpGet("getTeamTactic/{tacticID}")]
        public async Task<IActionResult> GetTeamTactic(Guid tacticID)
        {
            Tactic? teamTactic = await _db.Tactics.FirstOrDefaultAsync(tactic => tactic.TacticID == tacticID);

            if(teamTactic == null)
            {
                return NotFound("Tactic not found.");
            }

            var team = await _teamAccessService.GetOwnedTeamAsync(User);

            if (team == null || team.TeamID != teamTactic.TeamID)
            {
                return NotFound("Team not found or user does not have access to this tactic.");
            }

            return Ok(teamTactic);
        }

        [HttpGet("getTeamTactics")]
        public async Task<IActionResult> GetTeamTactics()
        {
            var team = await _teamAccessService.GetOwnedTeamAsync(User);

            if (team == null)
            {
                return NotFound("Team not found.");
            }

            List<Tactic> teamTactics = await _db.Tactics
                .Where(tactic => tactic.TeamID == team.TeamID)
                .ToListAsync();

            return Ok(teamTactics);
        }

        [HttpPost("createTeamTactic")]
        public async Task<IActionResult> CreateTeamTactic([FromBody] Tactic newTactic)
        {
            var teamExists = await _db.Teams.AnyAsync(t => t.TeamID == newTactic.TeamID);

            if (!teamExists)
            {
                return NotFound("Team not found.");
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Get all existing tactics for this team
                var existingTactics = await _db.Tactics
                    .Where(t => t.TeamID == newTactic.TeamID)
                    .ToListAsync();

                // If this is the first tactic for the team, always set it as main
                if (!existingTactics.Any())
                {
                    newTactic.isMain = true;
                }
                // If the new tactic is marked as main, set all other tactics to not main
                else if (newTactic.isMain)
                {
                    foreach (var tactic in existingTactics)
                    {
                        tactic.isMain = false;
                    }
                }

                _db.Tactics.Add(newTactic);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(newTactic);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpDelete("deleteTeamTactic/{tacticID}")]
        public async Task<IActionResult> DeleteTeamTactic(Guid tacticID)
        {
            Tactic? tactic = await _db.Tactics.FindAsync(tacticID);

            if (tactic == null)
            {
                return NotFound("Tactic not found.");
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Check if there are other tactics for this team
                var otherTacticsCount = await _db.Tactics
                    .CountAsync(t => t.TeamID == tactic.TeamID && t.TacticID != tacticID);

                if (otherTacticsCount == 0)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Cannot delete the last tactic of a team. A team must have at least one tactic.");
                }

                // If the deleted tactic is main, promote another tactic to main
                if (tactic.isMain)
                {
                    var newMainTactic = await _db.Tactics
                        .Where(t => t.TeamID == tactic.TeamID && t.TacticID != tacticID)
                        .OrderBy(t => t.TacticID)
                        .FirstOrDefaultAsync();

                    if (newMainTactic != null)
                    {
                        newMainTactic.isMain = true;
                    }
                }

                _db.Tactics.Remove(tactic);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpGet("getPlayerTactics/{tacticID}")]
        public async Task<IActionResult> GetPlayerTactics(Guid tacticID)
        {
            List<PlayerTactic> playerTactics = await _db.PlayerTactics
                .Where(pt => pt.TacticID == tacticID)
                .Include(p => p.Person)
                .ToListAsync();

            return Ok(playerTactics);
        }

        [HttpGet("getPlayerTacticsByTeamID/{teamID}")]
        public async Task<IActionResult> GetPlayerTacticsByTeamID(Guid teamID)
        {
            // Use a join to get all player tactics for the team's tactics in a single query
            List<PlayerTactic> playerTactics = await _db.PlayerTactics
                .Where(pt => _db.Tactics
                    .Where(t => t.TeamID == teamID)
                    .Select(t => t.TacticID)
                    .Contains(pt.TacticID))
                .ToListAsync();

            return Ok(playerTactics);
        }

        [HttpPost("addPlayerTactic")]
        public async Task<IActionResult> AddPlayerTactic([FromBody] PlayerTactic newPlayerTactic)
        {
            PlayerTactic? alreadySamePlayerTactic = await _db.PlayerTactics
                .Where(pt => pt.TacticID == newPlayerTactic.TacticID && pt.PlayerPosition == newPlayerTactic.PlayerPosition)
                .FirstOrDefaultAsync();

            if (alreadySamePlayerTactic != null)
            {
                _db.PlayerTactics.Remove(alreadySamePlayerTactic);
            }

            _db.PlayerTactics.Add(newPlayerTactic);
            await _db.SaveChangesAsync();

            return Ok(newPlayerTactic);
        }
    }
}
