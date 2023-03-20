using Core.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Services.Echo;

public class EchoService : IEchoService
{
    private static readonly IList<Wave> Waves = new List<Wave>();

    private readonly INetConfigurationProvider _netConfigurationProvider;
    private readonly ILogger<EchoService> _logger;

    public EchoService(
        INetConfigurationProvider netConfigurationProvider,
        ILogger<EchoService> logger)
    {
        _netConfigurationProvider = netConfigurationProvider;
        _logger = logger;
    }

    public Task StartWaveAsync()
    {
        var neighbours = _netConfigurationProvider.GetNeighbours().ToArray();
        var wave = Wave.CreateInitial(neighbours);

        Waves.Add(wave);

        _logger.LogInformation("Sending wave {WaveId} initiation signals..", wave.Id);
        foreach (var neighbour in neighbours)
        {
            _logger.LogInformation("Sending signal to {Neighbour}...", neighbour);
            SendSignalAsync(wave.Id, neighbour);
        }

        return Task.CompletedTask;
    }

    public Task OnSignalAsync(Guid waveId, int receivedFromPort)
    {
        var wave = Waves.SingleOrDefault(w => w.Id == waveId);
        if (wave == default)
        {
            _logger.LogInformation("New signal received from {Port} (wave = {WaveId})", receivedFromPort, waveId);
            return OnNewSignalReceivedAsync(receivedFromPort);
        }

        _logger.LogInformation("Echo received from {Port} (wave = {WaveId})", receivedFromPort, waveId);

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

    private Task OnNewSignalReceivedAsync(int receivedFromPort)
    {
        var neighbours = _netConfigurationProvider.GetNeighbours(receivedFromPort).ToArray();
        var wave = Wave.Create(receivedFromPort, neighbours);

        Waves.Add(wave);

        _logger.LogInformation("Sending wave {WaveId} signals..", wave.Id);
        foreach (var neighbour in neighbours)
        {
            _logger.LogInformation("Sending signal to {Neighbour}...", neighbour);
            SendSignalAsync(wave.Id, neighbour);
        }

        return Task.CompletedTask;
    }

    private Task OnSignalEchoReceivedAsync(Wave wave, int receivedFromPort)
    {
        wave.OnEchoReceived(receivedFromPort);
        var isCompleted = wave.IsAllEchoReceived();
        if (isCompleted)
        {
            _logger.LogInformation("All echo for signal received (wave {WaveId}) completed", wave.Id);
            return SendSignalAsync(wave.Id, wave.EchoPort!.Value);
        }

        _logger.LogInformation("Waiting for others echo...");
        return Task.CompletedTask;
    }


    private Task SendSignalAsync(Guid waveId, int receiverPort)
    {
        _logger.LogInformation("Sending signal to {Port}", receiverPort);
        return Task.CompletedTask;
    }
}