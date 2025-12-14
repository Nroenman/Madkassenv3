using MadkassenRestAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MadkassenRestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentWeather([FromQuery] double lat, [FromQuery] double lon)
        {
            var result = await _weatherService.GetCurrentWeatherAsync(lat, lon);

            if (result == null)
            {
                return StatusCode(502, "Could not fetch weather from external API");
            }

            return Ok(result);
        }
    }
}
