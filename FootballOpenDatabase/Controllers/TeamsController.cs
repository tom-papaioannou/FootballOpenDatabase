using Microsoft.AspNetCore.Mvc;
using FootballOpenDatabase.Models.Teams;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenDatabase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly FootballDbContext _context;

        public TeamsController(FootballDbContext context)
        {
            _context = context;
        }

        [HttpGet("{teamID}")]
        public async Task<ActionResult<Team>> GetTeam(Guid teamID)
        {
            var team = await _context.Teams
                .Include(t => t.Competitions)
                .FirstOrDefaultAsync(t => t.TeamID == teamID);

            if (team == null)
                return NotFound();

            return Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam([FromBody] Team team)
        {
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTeam), new { teamID = team.TeamID }, team);
        }

        [HttpPut("{teamID}")]
        public async Task<IActionResult> UpdateTeam(Guid teamID, [FromBody] Team updatedTeam)
        {
            if (teamID != updatedTeam.TeamID)
                return BadRequest();

            var team = await _context.Teams.FindAsync(teamID);
            if (team == null)
                return NotFound();

            team.Name = updatedTeam.Name;

            _context.Entry(team).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
