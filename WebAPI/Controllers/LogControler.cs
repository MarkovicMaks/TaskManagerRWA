using Microsoft.AspNetCore.Mvc;
using TM.BL.Services;

namespace WebAPI.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly LoggingService _loggingService;

        public LogsController(LoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        [HttpGet("get/{N?}")]
        public IActionResult GetLogs(int N = 10)
        {
            var latestLogs = _loggingService.GetLogs(N);
            return Ok(latestLogs);
        }

        [HttpGet("count")]
        public IActionResult GetLogCount()
        {
            return Ok(new { Count = _loggingService.GetLogCount() });
        }
    }
}
