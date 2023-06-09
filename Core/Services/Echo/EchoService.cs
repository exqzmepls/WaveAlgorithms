using Core.Clients.Echo;
using Core.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Services.Echo;

public class EchoService : IEchoService
{
    private static readonly IList<Wave> Waves = new List<Wave>();

    private readonly INetConfigurationProvider _netConfigurationProvider;
    private readonly IEchoClient _echoClient;
    private readonly ILogger<EchoService> _logger;

    public EchoService(
        INetConfigurationProvider netConfigurationProvider,
        IEchoClient echoClient,
        ILogger<EchoService> logger)
    {
        _netConfigurationProvider = netConfigurationProvider;
        _echoClient = echoClient;
        _logger = logger;
    }

    public async Task StartWaveAsync()
    {
        var neighbours = _netConfigurationProvider.GetNeighbours();
        if (neighbours.Count == 0)
        {
            _logger.LogInformation("No neighbours to send signal");
            return;
        }

        var wave = Wave.CreateInitial(neighbours);

        Waves.Add(wave);

        _logger.LogInformation("Sending wave {WaveId} initiation signals..", wave.Id);
        foreach (var neighbour in neighbours)
        {
            _logger.LogInformation("Sending signal to {Neighbour}...", neighbour);
            await SendSignalAsync(wave.Id, neighbour);
        }
    }

    public Task OnSignalAsync(Guid waveId, int receivedFromPort)
    {
        var wave = Waves.SingleOrDefault(w => w.Id == waveId);
        if (wave == default)
        {
            _logger.LogInformation("New signal received from {Port} (wave = {WaveId})", receivedFromPort, waveId);
            return OnNewSignalReceivedAsync(waveId, receivedFromPort);
        }

        if (wave.EchoPort != default)
            return OnSignalEchoReceivedAsync(wave, receivedFromPort);

        OnWaveEchoReceived(wave, receivedFromPort);
        return Task.CompletedTask;
    }

    private void OnWaveEchoReceived(Wave wave, int receivedFromPort)
    {
        wave.OnEchoReceived(receivedFromPort);
        var isCompleted = wave.IsAllEchoReceived();
        if (isCompleted)
        {
            _logger.LogInformation("Wave {WaveId} completed", wave.Id);
            return;
        }

        _logger.LogInformation("Waiting for others echo...");
    }

    private async Task OnNewSignalReceivedAsync(Guid waveId, int receivedFromPort)
    {
        var neighbours = _netConfigurationProvider.GetNeighbours(receivedFromPort);
        var wave = Wave.Create(waveId, receivedFromPort, neighbours);

        Waves.Add(wave);

        if (neighbours.Count == 0)
        {
            _logger.LogInformation("No neighbours to continue. Sending echo to {Port}...", receivedFromPort);
            await SendSignalAsync(wave.Id, receivedFromPort);
            return;
        }

        _logger.LogInformation("Sending wave {WaveId} signals..", wave.Id);
        foreach (var neighbour in neighbours)
        {
            _logger.LogInformation("Sending signal to {Neighbour}...", neighbour);
            await SendSignalAsync(wave.Id, neighbour);
        }
    }

    private async Task OnSignalEchoReceivedAsync(Wave wave, int receivedFromPort)
    {
        _logger.LogInformation("Echo received from {Port} (wave = {WaveId})", receivedFromPort, wave.Id);

        wave.OnEchoReceived(receivedFromPort);
        var isCompleted = wave.IsAllEchoReceived();
        if (isCompleted)
        {
            var echoPort = wave.EchoPort!.Value;
            _logger.LogInformation("All echo received (wave {WaveId}). Sending echo to {EchoPort}...", wave.Id,
                echoPort);
            await SendSignalAsync(wave.Id, echoPort);
            return;
        }

        _logger.LogInformation("Waiting for others echo...");
    }

    private Task SendSignalAsync(Guid waveId, int receiverPort)
    {
        var endpoint = _netConfigurationProvider.GetEndpoint();
        return _echoClient.SendSignalAsync(waveId, endpoint, receiverPort);
    }
}