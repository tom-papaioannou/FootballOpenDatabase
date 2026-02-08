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

        public TacticsController(FootballDbContext context)
        {
            _context = context;
        }

        [HttpGet("getTeamTactic/{tacticID}")]
        public async Task<IActionResult> GetTeamTactic(Guid tacticID)
        {
            Tactic? teamTactic = await _context.Tactics.FirstOrDefaultAsync(tactic => tactic.TacticID == tacticID);

            if(teamTactic == null)
            {
                return NotFound();
            }

            return Ok(teamTactic);
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

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get all existing tactics for this team
                var existingTactics = await _context.Tactics
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

                _context.Tactics.Add(newTactic);
                await _context.SaveChangesAsync();
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
            Tactic? tactic = await _context.Tactics.FindAsync(tacticID);

            if (tactic == null)
            {
                return NotFound("Tactic not found.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if there are other tactics for this team
                var otherTacticsCount = await _context.Tactics
                    .CountAsync(t => t.TeamID == tactic.TeamID && t.TacticID != tacticID);

                if (otherTacticsCount == 0)
                {
                    await transaction.RollbackAsync();
                    return BadRequest("Cannot delete the last tactic of a team. A team must have at least one tactic.");
                }

                // If the deleted tactic is main, promote another tactic to main
                if (tactic.isMain)
                {
                    var newMainTactic = await _context.Tactics
                        .Where(t => t.TeamID == tactic.TeamID && t.TacticID != tacticID)
                        .OrderBy(t => t.TacticID)
                        .FirstOrDefaultAsync();

                    if (newMainTactic != null)
                    {
                        newMainTactic.isMain = true;
                    }
                }

                _context.Tactics.Remove(tactic);
                await _context.SaveChangesAsync();
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
            List<PlayerTactic> playerTactics = await _context.PlayerTactics
                .Where(pt => pt.TacticID == tacticID)
                .Include(pt => pt.Player).ThenInclude(p => p.Person)
                .ToListAsync();

            return Ok(playerTactics);
        }

        [HttpGet("getPlayerTacticsByTeamID/{teamID}")]
        public async Task<IActionResult> GetPlayerTacticsByTeamID(Guid teamID)
        {
            // Use a join to get all player tactics for the team's tactics in a single query
            List<PlayerTactic> playerTactics = await _context.PlayerTactics
                .Where(pt => _context.Tactics
                    .Where(t => t.TeamID == teamID)
                    .Select(t => t.TacticID)
                    .Contains(pt.TacticID))
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
