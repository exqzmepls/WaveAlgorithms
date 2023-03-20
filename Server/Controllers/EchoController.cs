using Core.Services.Echo;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class EchoController : ControllerBase
{
    private static readonly object Locker = new();

    private readonly IEchoService _echoService;
    private readonly ILogger<EchoController> _logger;

    public EchoController(
        IEchoService echoService,
        ILogger<EchoController> logger)
    {
        _echoService = echoService;
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Wave()
    {
        lock (Locker)
        {
            _logger.LogInformation("Starting new wave...");
            _echoService.StartWaveAsync();
            return Ok();
        }
    }

    [HttpGet]
    public IActionResult Wave([FromQuery] Guid id, [FromQuery] int sender)
    {
        lock (Locker)
        {
            _logger.LogInformation("Received signal from {Port} (wave = {WaveId})", sender, id);
            _echoService.OnSignalAsync(id, sender);
            return Ok();
        }
    }
}