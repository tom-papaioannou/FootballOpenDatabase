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

        [HttpGet("getAllServers")]
        public async Task<IActionResult> GetAllServers()
        {
            return Ok(await _db.Servers.ToListAsync());
        }
    }
}
