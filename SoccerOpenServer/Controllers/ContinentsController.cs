// Copyright (c) 2026 Tom Papaioannou. All rights reserved.
// Licensed under the MIT License

using SoccerOpenServer.Models.Competitions;
using SoccerOpenServer.Models.World;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace SoccerOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContinentsController : ControllerBase
    {
        private readonly SoccerDbContext _context;

        public ContinentsController(SoccerDbContext context)
        {
            _context = context;
        }

        [HttpGet("getAllContinents")]
        public async Task<IActionResult> GetAllContinents()
        {
            var continents = await _context.Continents
                .Include(c => c.Nations)
                .ToListAsync();

            if (continents == null)
                return NotFound();

            return Ok(continents);
        }

        [HttpGet("getAllCompetitionParents")]
        public async Task<ActionResult<Nation>> GetAllCompetitionParents()
        {
            var nations = await _context.Nations.ToListAsync();

            if (nations == null)
                return NotFound();

            return Ok(nations);
        }

        [HttpGet("{competitionParentID}")]
        public async Task<ActionResult<Nation>> GetCompetitionParent(Guid nationID)
        {
            var nation = await _context.Nations
                .FirstOrDefaultAsync(cp => cp.NationID == nationID);

            if (nation == null)
                return NotFound();

            return Ok(nation);
        }

        //[HttpPost]
        //public async Task<ActionResult<Nation>> PostCompetitionParent([FromBody] CreateCompetitionParentRequest request)
        //{
        //    var nation = new Nation
        //    {
        //        NationID = Guid.NewGuid(),
        //        Name = request.Name
        //    };

        //    _context.Nations.Add(nation);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction(nameof(GetCompetitionParent), new { nationID = nation.NationID }, nation);
        //}

        //[HttpPut("{competitionParentID}")]
        //public async Task<IActionResult> UpdateCompetitionParent(Guid competitionParentID, [FromBody] CompetitionParent updatedCompetitionParent)
        //{
        //    if (competitionParentID != updatedCompetitionParent.CompetitionParentID)
        //        return BadRequest("The ID in the URL does not match the ID in the request body");

        //    var competitionParent = await _context.CompetitionParents.FindAsync(competitionParentID);
        //    if (competitionParent == null)
        //        return NotFound();

        //    competitionParent.Name = updatedCompetitionParent.Name;
        //    competitionParent.CompetitionParentType = updatedCompetitionParent.CompetitionParentType;
        //    competitionParent.NumberOfLeagues = updatedCompetitionParent.NumberOfLeagues;
        //    competitionParent.NumberOfCups = updatedCompetitionParent.NumberOfCups;
        //    competitionParent.NumberOfNationalLeagues = updatedCompetitionParent.NumberOfNationalLeagues;
        //    competitionParent.NumberOfNationalCups = updatedCompetitionParent.NumberOfNationalCups;
        //    competitionParent.NationalTeamID = updatedCompetitionParent.NationalTeamID;

        //    _context.Entry(competitionParent).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //[HttpDelete("{competitionParentID}")]
        //public async Task<IActionResult> UpdateCompetitionParent(Guid competitionParentID)
        //{
        //    var competitionParent = await _context.CompetitionParents.FindAsync(competitionParentID);
        //    if (competitionParent == null)
        //        return NotFound();

        //    _context.Entry(competitionParent).State = EntityState.Deleted;
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}
