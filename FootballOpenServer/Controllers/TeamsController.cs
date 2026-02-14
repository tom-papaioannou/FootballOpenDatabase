using FootballOpenServer.Models.Contracts;
using FootballOpenServer.Models.People;
using FootballOpenServer.Models.Teams;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FootballOpenServer.Controllers
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

        [HttpGet("getCurrentTeam")]
        public async Task<ActionResult<Team>> GetCurrentTeam()
        {
            var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id.ToString() == userID);

            if (user == null)
            {
                return NotFound("User not found");
            }

            Person person = await _context.People.FirstOrDefaultAsync(u => u.AppUser.Id.ToString() == userID);

            if (person == null)
            {
                return NotFound("Manager not found for this user.");
            }

            var contract = await _context.Contracts
                .Include(c => c.Team)
                .FirstOrDefaultAsync(c => c.EndDate > DateTime.Now && c.PersonID == person.PersonID);

            if (contract == null || contract.Team == null)
                return NotFound("Active contract not found.");

            return Ok(contract.Team);
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

        [HttpGet("getTeamSquad/{teamID}")]
        public async Task<IActionResult> GetTeamSquad(Guid teamID)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.TeamID == teamID);
            if (team == null)
            {
                return NotFound();
            }

            var squad = await _context.Players
                .Where(pl => _context.Contracts.Any(c =>
                    c.PersonID == pl.PersonID &&
                    c.TeamID == teamID &&
                    c.EndDate > DateTime.Now &&
                    c.Role == Role.Player))
                .Include(pl => pl.Person)
                .Include(pl => pl.PlayerTrainedPositions)
                .Select(pl => new
                {
                    pl.PlayerID,
                    Person = new
                    {
                        pl.Person!.Name,
                        pl.Person!.Surname,
                        pl.Person!.DateOfBirth,
                    },
                    PlayerTrainedPositions = pl.PlayerTrainedPositions!.Select(ptp => new
                    {
                        ptp.PlayerPosition,
                        ptp.PlayerTrainedPositionAdaptation
                    })
                })
                .ToListAsync();

            return Ok(squad);
        }
    }
}
