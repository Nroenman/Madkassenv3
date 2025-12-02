using Microsoft.AspNetCore.Mvc;

namespace MadkassenRestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("OK");
    }
}