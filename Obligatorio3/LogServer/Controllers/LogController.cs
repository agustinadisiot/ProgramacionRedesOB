using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using LogServer;
using Microsoft.AspNetCore.Mvc;

namespace AdminServer.Controllers
{
    [Route("logs")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly DataAccess da;
        public LogController()
        {
            da = DataAccess.GetInstance();
        }

        [HttpGet]
        public async Task<ActionResult<List<LogRecord>>> GetLogs(
            [FromQuery] int? gameId,
            [FromQuery] string gameName,
            [FromQuery] int? userId,
            [FromQuery] string username,
            [FromQuery] DateTime? minDateTime,
            [FromQuery] DateTime? maxDateTime,
            [FromQuery] string severity
            )
        {

            Filter filter = new Filter
            {
                GameId = gameId,
                GameName = gameName,
                UserId = userId,
                Username = username,
                MinDateTime = minDateTime,
                MaxDateTime = maxDateTime,
                Severity = severity

            };
            return Ok(da.GetLogs(filter));
        }

    }
}
