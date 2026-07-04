// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

﻿using SoccerOpenServer.DTO.Competitions;
using Microsoft.AspNetCore.Mvc;
using SoccerOpenServer.Models.Competitions;
using SoccerOpenServer.Models.Teams;
using SoccerOpenServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SoccerOpenServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionsController : ControllerBase
    {
        private readonly SoccerDbContext _context;
        private readonly ITeamGenerationService _teamGenerationService;
        private readonly ITeamAccessService _teamAccessService;

        public CompetitionsController(
            SoccerDbContext context,
            ITeamGenerationService teamGenerationService,
            ITeamAccessService teamAccessService)
        {
            _context = context;
            _teamGenerationService = teamGenerationService;
            _teamAccessService = teamAccessService;
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

        [HttpGet("{competitionID}/table")]
        public async Task<ActionResult<IEnumerable<CompetitionTableRowDTO>>> GetCompetitionTable(Guid competitionID)
        {
            var competitionExists = await _context.Competitions
                .AnyAsync(c => c.CompetitionID == competitionID);

            if (!competitionExists)
                return NotFound();

            var tableRows = await _context.CompetitionTables
                .Where(ct => ct.CompetitionID == competitionID)
                .Join(
                    _context.Teams,
                    ct => ct.TeamID,
                    team => team.TeamID,
                    (ct, team) => new CompetitionTableRowDTO
                    {
                        TeamID = team.TeamID,
                        TeamName = team.Name,
                        Points = ct.Points,
                        Wins = ct.Wins,
                        Draws = ct.Draws,
                        Losses = ct.Losses,
                        YellowCards = ct.YellowCards,
                        RedCards = ct.RedCards,
                        MatchesPlayed = ct.MatchesPlayed
                    })
                .OrderByDescending(row => row.Points)
                .ThenByDescending(row => row.Wins)
                .ThenByDescending(row => row.Draws)
                .ThenBy(row => row.Losses)
                .ThenBy(row => row.YellowCards)
                .ThenBy(row => row.RedCards)
                .ThenBy(row => row.TeamName)
                .ToListAsync();

            for (int i = 0; i < tableRows.Count; i++)
            {
                tableRows[i].Position = i + 1;
            }

            return Ok(tableRows);
        }

        [HttpGet("{competitionID}/cup-bracket")]
        public async Task<ActionResult<CupBracketDTO>> GetCupBracket(Guid competitionID)
        {
            var competition = await _context.Competitions
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CompetitionID == competitionID);

            if (competition == null)
                return NotFound();

            if (competition.CompetitionType != CompetitionType.Knockout)
                return BadRequest("Cup bracket data is only available for knockout competitions.");

            var rounds = await _context.CupRounds
                .AsNoTracking()
                .Where(r => r.CompetitionID == competitionID)
                .OrderBy(r => r.RoundNumber)
                .ToListAsync();

            var roundIDs = rounds.Select(r => r.CupRoundID).ToList();
            var ties = await _context.CupTies
                .AsNoTracking()
                .Where(t => roundIDs.Contains(t.CupRoundID))
                .OrderBy(t => t.TieNumber)
                .ToListAsync();

            var teamIDs = ties
                .SelectMany(t => new[] { t.HomeTeamID, t.AwayTeamID, t.WinnerTeamID })
                .Where(teamID => teamID.HasValue)
                .Select(teamID => teamID!.Value)
                .Distinct()
                .ToList();

            var teamsByID = await _context.Teams
                .AsNoTracking()
                .Include(t => t.Kit)
                .Where(t => teamIDs.Contains(t.TeamID))
                .ToDictionaryAsync(
                    t => t.TeamID,
                    t => new CupBracketTeamDTO
                    {
                        TeamID = t.TeamID,
                        Name = t.Name,
                        BadgeColor = t.Kit.HomeShirtColor
                    });

            var tiesByRoundID = ties
                .GroupBy(t => t.CupRoundID)
                .ToDictionary(g => g.Key, g => g.OrderBy(t => t.TieNumber).ToList());

            var bracket = new CupBracketDTO
            {
                CompetitionID = competitionID,
                Rounds = rounds
                    .Select(round => new CupBracketRoundDTO
                    {
                        CupRoundID = round.CupRoundID,
                        RoundNumber = round.RoundNumber,
                        TeamCount = round.TeamCount,
                        RoundType = round.RoundType,
                        Ties = tiesByRoundID.GetValueOrDefault(round.CupRoundID, new List<CupTie>())
                            .Select(tie => new CupBracketTieDTO
                            {
                                CupTieID = tie.CupTieID,
                                CupRoundID = tie.CupRoundID,
                                TieNumber = tie.TieNumber,
                                HomeTeamID = tie.HomeTeamID,
                                AwayTeamID = tie.AwayTeamID,
                                WinnerTeamID = tie.WinnerTeamID,
                                NextCupTieID = tie.NextCupTieID,
                                AdvancesAsHomeTeam = tie.AdvancesAsHomeTeam,
                                IsCompleted = tie.IsCompleted,
                                HomeTeam = GetCupBracketTeam(tie.HomeTeamID, teamsByID),
                                AwayTeam = GetCupBracketTeam(tie.AwayTeamID, teamsByID),
                                WinnerTeam = GetCupBracketTeam(tie.WinnerTeamID, teamsByID)
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return Ok(bracket);
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

        [HttpGet("world")]
        public async Task<ActionResult<IEnumerable<Competition>>> GetWorldCompetitions()
        {
            var competitions = await _context.Competitions
                .Where(c => c.NationID == null && c.ContinentID == null)
                .Include(c => c.Teams)
                .Include(c => c.Nation)
                .Include(c => c.Continent)
                .ToListAsync();

            return Ok(competitions);
        }

        [HttpGet("continent/{continentID}")]
        public async Task<ActionResult<IEnumerable<Competition>>> GetContinentCompetitions(Guid continentID)
        {
            var competitions = await _context.Competitions
                .Where(c => c.NationID == null && c.ContinentID == continentID)
                .Include(c => c.Teams)
                .Include(c => c.Nation)
                .Include(c => c.Continent)
                .ToListAsync();

            return Ok(competitions);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<Competition>>> GetMyCompetitions()
        {
            var team = await _teamAccessService.GetOwnedTeamCompetitionsAsync(User);

            if (team == null)
            {
                return Ok(Array.Empty<Competition>());
            }

            return Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult<Competition>> PostCompetition([FromBody] CreateCompetitionRequest request)
        {
            if(request.NationID != null)
            {
                var nation = await _context.Nations.FindAsync(request.NationID);
                if (nation == null)
                    return BadRequest($"Nation with ID {request.NationID} not found");

                // Generate 16 teams for the new competition
                var generatedTeams = await _teamGenerationService.GenerateTeamsForCompetition(request.ServerID, request.NationID, 16, request.Priority);

                var competition = new Competition
                {
                    CompetitionID = Guid.NewGuid(),
                    CompetitionName = request.CompetitionName,
                    NationID = request.NationID,
                    CompetitionTeamsType = request.CompetitionTeamsType,
                    Priority = request.Priority,
                    CompetitionType = request.CompetitionType,
                    Teams = generatedTeams,
                    ServerID = request.ServerID
                };

                _context.Competitions.Add(competition);
                foreach (var team in generatedTeams)
                {
                    _context.CompetitionTables.Add(new CompetitionTable
                    {
                        CompetitionTableID = Guid.NewGuid(),
                        CompetitionID = competition.CompetitionID,
                        TeamID = team.TeamID,
                        MatchesPlayed = 0,
                        Wins = 0,
                        Draws = 0,
                        Losses = 0,
                        GoalsFor = 0,
                        GoalsAgainst = 0,
                        YellowCards = 0,
                        RedCards = 0,
                        Points = 0
                    });
                }

                await _context.SaveChangesAsync();
                await _teamGenerationService.AssignPlayersToGeneratedTeams(generatedTeams.Select(t => t.TeamID));
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

        private static CupBracketTeamDTO? GetCupBracketTeam(Guid? teamID, IReadOnlyDictionary<Guid, CupBracketTeamDTO> teamsByID)
        {
            return teamID.HasValue && teamsByID.TryGetValue(teamID.Value, out var team)
                ? team
                : null;
        }
    }
}
