using Microsoft.AspNetCore.Mvc;

namespace FootballOpenDatabase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("connection")]
        public Task<ActionResult<string>> TestConnection()
        {
            return Task.FromResult<ActionResult<string>>(Ok("API is working!"));
        }
    }
}
