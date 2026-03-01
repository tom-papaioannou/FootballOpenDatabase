// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using Microsoft.AspNetCore.Mvc;
using FootballOpenServer.Models.Competitions;
using FootballOpenServer.Models.Teams;
using FootballOpenServer.Services;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionsController : ControllerBase
    {
        private readonly FootballDbContext _context;
        private readonly ITeamGenerationService _teamGenerationService;

        public CompetitionsController(FootballDbContext context, ITeamGenerationService teamGenerationService)
        {
            _context = context;
            _teamGenerationService = teamGenerationService;
        }

        [HttpGet("{competitionID}")]
        public async Task<ActionResult<Competition>> GetCompetition(Guid competitionID)
        {
            var competition = await _context.Competitions
                .Include(c => c.Teams)
                .Include(c => c.Nation)
                .Include(c => c.Continent)
                .FirstOrDefaultAsync(c => c.CompetitionID == competitionID);

            if (competition == null)
                return NotFound();

            return Ok(competition);
        }

        [HttpGet("getAllCompetitions/{competitionParentID}")]
        public async Task<ActionResult<Competition>> GetAllCompetitions(Guid competitionParentID)
        {
            var competition = await _context.Competitions
                .Where(c => c.NationID == competitionParentID || c.ContinentID == competitionParentID)
                .Include(c => c.Teams)
                .Include(c => c.Nation)
                .Include(c => c.Continent)
                .ToListAsync();

            if (competition == null)
                return NotFound();

            return Ok(competition);
        }

        [HttpPost]
        public async Task<ActionResult<Competition>> PostCompetition([FromBody] CreateCompetitionRequest request)
        {
            if(request.NationID != null)
            {
                var nation = await _context.Nations.FindAsync(request.NationID);
                if (nation == null)
                    return BadRequest($"Nation with ID {request.NationID} not found");

                // Generate 20 teams for the new competition
                var generatedTeams = _teamGenerationService.GenerateTeamsForCompetition(20);

                var competition = new Competition
                {
                    CompetitionID = Guid.NewGuid(),
                    CompetitionName = request.CompetitionName,
                    NationID = request.NationID,
                    CompetitionTeamsType = request.CompetitionTeamsType,
                    Priority = request.Priority,
                    CompetitionType = request.CompetitionType,
                    Teams = generatedTeams
                };

                _context.Competitions.Add(competition);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCompetition), new { competitionID = competition.CompetitionID }, competition);
            }
            else if(request.ContinentID != null)
            {
                var continent = await _context.Continents.FindAsync(request.ContinentID);
                if (continent == null)
                    return BadRequest($"Continent with ID {request.ContinentID} not found");

                return BadRequest($"Not Implemented yet.");
            }
            else
            {
                // Competition Parent is the World
                return BadRequest($"Not Implemented yet.");
            }
        }

        [HttpPut("{competitionID}")]
        public async Task<IActionResult> UpdateCompetition(Guid competitionID, [FromBody] Competition updatedCompetition)
        {
            if (competitionID != updatedCompetition.CompetitionID)
                return BadRequest();

            var competition = await _context.Competitions.FindAsync(competitionID);
            if (competition == null)
                return NotFound();

            competition.CompetitionName = updatedCompetition.CompetitionName;
            competition.NationID = updatedCompetition.NationID;
            competition.ContinentID = updatedCompetition.ContinentID;
            competition.CompetitionTeamsType = updatedCompetition.CompetitionTeamsType;
            competition.Priority = updatedCompetition.Priority;
            competition.CompetitionType = updatedCompetition.CompetitionType;

            _context.Entry(competition).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}