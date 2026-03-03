using FootballOpenServer.Models.Servers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballOpenServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServersController : ControllerBase
    {
        private readonly FootballDbContext _db;

        public ServersController(FootballDbContext db)
        {
            _db = db;
        }

        [HttpGet("getUserServer/{userID}")]
        public async Task<IActionResult> GetUserServer(Guid userID)
        {
            Guid serverID = await _db.AppUsers
                .Where(u => u.Id == userID)
                .Include(u => u.Person)
                .Select(u => u.Person.ServerID ?? new Guid())
                .FirstOrDefaultAsync();
            return Ok(serverID);
        }

        [HttpGet("getAllServers")]
        public async Task<IActionResult> GetAllServers()
        {
            return Ok(await _db.Servers.ToListAsync());
        }

        [HttpPost("createNewServer")]
        public async Task<IActionResult> CreateNewServer([FromBody] Server server)
        {
            try
            {
                await _db.Servers.AddAsync(server);
                await _db.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return BadRequest();
            } 
            return Ok(server);
        }

        [HttpGet("getServerInformation/{serverID}")]
        public async Task<IActionResult> GetServerInformation(Guid serverID)
        {
            Server server = await _db.Servers
                .Include(s => s.Persons)
                .Include(s => s.Competitions)
                .FirstOrDefaultAsync(s => s.ServerID == serverID);

            return Ok(server);
        }
    }
}
