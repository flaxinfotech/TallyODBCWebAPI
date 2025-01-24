using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TallyIntegrationAPI.Models;

namespace TallyIntegrationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly TallyConfiguration _config;

        public ConfigurationController(IOptions<TallyConfiguration> config)
        {
            _config = config.Value;
        }

        [HttpGet]
        public IActionResult GetConfiguration()
        {
            return Ok(_config);
        }
    }
}
