using Microsoft.AspNetCore.Mvc;
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
                .Include(c => c.CompetitionParent)
                .FirstOrDefaultAsync(c => c.CompetitionID == competitionID);

            if (competition == null)
                return NotFound();

            return Ok(competition);
        }

        [HttpGet("getAllCompetitions/{competitionParentID}")]
        public async Task<ActionResult<Competition>> GetAllCompetitions(Guid competitionParentID)
        {
            var competition = await _context.Competitions
                .Where(c => c.ParentID == competitionParentID)
                .Include(c => c.Teams)
                .Include(c => c.CompetitionParent)
                .ToListAsync();

            if (competition == null)
                return NotFound();

            return Ok(competition);
        }

        [HttpPost]
        public async Task<ActionResult<Competition>> PostCompetition([FromBody] CreateCompetitionRequest request)
        {
            // Validate that the CompetitionParent exists
            var competitionParent = await _context.CompetitionParents.FindAsync(request.ParentID);
            if (competitionParent == null)
                return BadRequest($"CompetitionParent with ID {request.ParentID} not found");

            // Generate 20 teams for the new competition
            var generatedTeams = _teamGenerationService.GenerateTeamsForCompetition(20);

            var competition = new Competition
            {
                CompetitionID = Guid.NewGuid(),
                CompetitionName = request.CompetitionName,
                ParentID = request.ParentID,
                CompetitionTeamsType = request.CompetitionTeamsType,
                Priority = request.Priority,
                CompetitionType = request.CompetitionType,
                Teams = generatedTeams
            };

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