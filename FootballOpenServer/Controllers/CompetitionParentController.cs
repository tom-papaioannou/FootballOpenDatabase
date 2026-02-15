// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using Microsoft.AspNetCore.Mvc;
using FootballOpenServer.Models.Competitions;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionParentController : ControllerBase
    {
        private readonly FootballDbContext _context;

        public CompetitionParentController(FootballDbContext context)
        {
            _context = context;
        }

        [HttpGet("getAllCompetitionParents")]
        public async Task<ActionResult<CompetitionParent>> GetAllCompetitionParents()
        {
            var competitionParent = await _context.CompetitionParents.ToListAsync();

            if (competitionParent == null)
                return NotFound();

            return Ok(competitionParent);
        }

        [HttpGet("{competitionParentID}")]
        public async Task<ActionResult<CompetitionParent>> GetCompetitionParent(Guid competitionParentID)
        {
            var competitionParent = await _context.CompetitionParents
                .FirstOrDefaultAsync(cp => cp.CompetitionParentID == competitionParentID);

            if (competitionParent == null)
                return NotFound();

            return Ok(competitionParent);
        }

        [HttpPost]
        public async Task<ActionResult<CompetitionParent>> PostCompetitionParent([FromBody] CreateCompetitionParentRequest request)
        {
            var competitionParent = new CompetitionParent
            {
                CompetitionParentID = Guid.NewGuid(),
                Name = request.Name,
                CompetitionParentType = request.CompetitionParentType,
                NumberOfLeagues = request.NumberOfLeagues,
                NumberOfCups = request.NumberOfCups,
                NumberOfNationalLeagues = request.NumberOfNationalLeagues,
                NumberOfNationalCups = request.NumberOfNationalCups,
                NationalTeamID = request.NationalTeamID
            };

            _context.CompetitionParents.Add(competitionParent);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCompetitionParent), new { competitionParentID = competitionParent.CompetitionParentID }, competitionParent);
        }

        [HttpPut("{competitionParentID}")]
        public async Task<IActionResult> UpdateCompetitionParent(Guid competitionParentID, [FromBody] CompetitionParent updatedCompetitionParent)
        {
            if (competitionParentID != updatedCompetitionParent.CompetitionParentID)
                return BadRequest("The ID in the URL does not match the ID in the request body");

            var competitionParent = await _context.CompetitionParents.FindAsync(competitionParentID);
            if (competitionParent == null)
                return NotFound();

            competitionParent.Name = updatedCompetitionParent.Name;
            competitionParent.CompetitionParentType = updatedCompetitionParent.CompetitionParentType;
            competitionParent.NumberOfLeagues = updatedCompetitionParent.NumberOfLeagues;
            competitionParent.NumberOfCups = updatedCompetitionParent.NumberOfCups;
            competitionParent.NumberOfNationalLeagues = updatedCompetitionParent.NumberOfNationalLeagues;
            competitionParent.NumberOfNationalCups = updatedCompetitionParent.NumberOfNationalCups;
            competitionParent.NationalTeamID = updatedCompetitionParent.NationalTeamID;

            _context.Entry(competitionParent).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{competitionParentID}")]
        public async Task<IActionResult> UpdateCompetitionParent(Guid competitionParentID)
        {
            var competitionParent = await _context.CompetitionParents.FindAsync(competitionParentID);
            if (competitionParent == null)
                return NotFound();

            _context.Entry(competitionParent).State = EntityState.Deleted;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
