using FootballOpenServer.Models.Teams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TacticsController : ControllerBase
    {
        private FootballDbContext _context;

        TacticsController(FootballDbContext context)
        {
            _context = context;
        }

        [HttpGet("getTeamTactics/{teamID}")]
        public async Task<IActionResult> GetTeamTactics(Guid teamID)
        {
            List<Tactic> teamTactics = await _context.Tactics
                .Where(tactic => tactic.TeamID == teamID)
                .ToListAsync();

            return Ok(teamTactics);
        }

        [HttpPost("createTeamTactic")]
        public async Task<IActionResult> CreateTeamTactic([FromBody] Tactic newTactic)
        {
            var teamExists = await _context.Teams.AnyAsync(t => t.TeamID == newTactic.TeamID);

            if (!teamExists)
            {
                return NotFound("Team not found.");
            }

            _context.Tactics.Add(newTactic);

            try
            { 
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }

            return Ok(newTactic);
        }

        [HttpGet("getPlayerTactics/{tacticID}")]
        public async Task<IActionResult> GetPlayerTactics(Guid tacticID)
        {
            List<PlayerTactic> playerTactics = await _context.PlayerTactics
                .Where(pt => pt.TacticID == tacticID)
                .ToListAsync();

            return Ok(playerTactics);
        }

        [HttpPost("addPlayerTactic")]
        public async Task<IActionResult> AddPlayerTactic([FromBody] PlayerTactic newPlayerTactic)
        {
            PlayerTactic? alreadySamePlayerTactic = await _context.PlayerTactics
                .Where(pt => pt.TacticID == newPlayerTactic.TacticID && pt.PlayerPosition == newPlayerTactic.PlayerPosition)
                .FirstOrDefaultAsync();

            if (alreadySamePlayerTactic != null)
            {
                _context.PlayerTactics.Remove(alreadySamePlayerTactic);
            }

            _context.PlayerTactics.Add(newPlayerTactic);
            await _context.SaveChangesAsync();

            return Ok(newPlayerTactic);
        }
    }
}
