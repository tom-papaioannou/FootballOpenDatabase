using Microsoft.AspNetCore.Mvc;
using FootballOpenDatabase.Models.Tournaments;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenDatabase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        private readonly FootballDbContext _context;

        public TournamentsController(FootballDbContext context)
        {
            _context = context;
        }

        [HttpGet("{tournamentID}")]
        public async Task<ActionResult<Tournament>> GetTournament(Guid tournamentID)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Teams)
                .Include(t => t.TournamentParent)
                .FirstOrDefaultAsync(t => t.TournamentID == tournamentID);

            if (tournament == null)
                return NotFound();

            return Ok(tournament);
        }

        [HttpPost]
        public async Task<ActionResult<Tournament>> PostTournament([FromBody] Tournament tournament)
        {
            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTournament), new { tournamentID = tournament.TournamentID }, tournament);
        }

        [HttpPut("{tournamentID}")]
        public async Task<IActionResult> UpdateTournament(Guid tournamentID, [FromBody] Tournament updatedTournament)
        {
            if (tournamentID != updatedTournament.TournamentID)
                return BadRequest();

            var tournament = await _context.Tournaments.FindAsync(tournamentID);
            if (tournament == null)
                return NotFound();

            tournament.TournamentName = updatedTournament.TournamentName;
            tournament.ParentID = updatedTournament.ParentID;
            tournament.TournamentTeamsType = updatedTournament.TournamentTeamsType;
            tournament.Priority = updatedTournament.Priority;
            tournament.TournamentType = updatedTournament.TournamentType;

            _context.Entry(tournament).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}