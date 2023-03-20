using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class EchoController : ControllerBase
{
    private readonly ILogger<EchoController> _logger;

    public EchoController(ILogger<EchoController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Wave()
    {
    }

    [HttpGet]
    public IActionResult Wave([FromQuery] Guid id, [FromQuery] int port)
    {
        _logger.LogInformation("Received signal from {Port} (wave = {WaveId})", port, id);
        
    }
}