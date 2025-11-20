using Microsoft.AspNetCore.Mvc;
using FootballOpenDatabase.Models.Competitions;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenDatabase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitionsController : ControllerBase
    {
        private readonly FootballDbContext _context;

        public CompetitionsController(FootballDbContext context)
        {
            _context = context;
        }

        [HttpGet("{competitionID}")]
        public async Task<ActionResult<Competition>> GetCompetition(Guid competitionID)
        {
            var competition = await _context.Competitions
                .Include(c => c.Teams)
                .Include(c => c.CompetitionParent)
                .FirstOrDefaultAsync(c => c.CompetitionID == competitionID);

            if (competition == null)
                return NotFound();

            return Ok(competition);
        }

        [HttpPost]
        public async Task<ActionResult<Competition>> PostCompetition([FromBody] Competition competition)
        {
            _context.Competitions.Add(competition);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCompetition), new { competitionID = competition.CompetitionID }, competition);
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
            competition.ParentID = updatedCompetition.ParentID;
            competition.CompetitionTeamsType = updatedCompetition.CompetitionTeamsType;
            competition.Priority = updatedCompetition.Priority;
            competition.CompetitionType = updatedCompetition.CompetitionType;

            _context.Entry(competition).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}