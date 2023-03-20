using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Services.Echo;

public class EchoService : IEchoService
{
    private static readonly IList<Wave> Waves = new List<Wave>();

    private static readonly IList<Signal> ReceivedSignals = new List<Signal>();

    private readonly IConfiguration _configuration;
    private readonly ILogger<EchoService> _logger;

    public EchoService(
        IConfiguration configuration,
        ILogger<EchoService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task StartWaveAsync()
    {
        var netSection = _configuration.GetRequiredSection("Net");
        var configuration = _configuration.
        throw new NotImplementedException();
    }

    public Task OnSignalAsync(Guid waveId, int receivedFromPort)
    {
        var wave = Waves.SingleOrDefault(w => w.Id == waveId);
        if (wave != default)
        {
            OnWaveEchoReceived(wave, receivedFromPort);
        }

        var existingSignal = ReceivedSignals.SingleOrDefault(s => s.WaveId == waveId);
        if (existingSignal != default)
        {
            existingSignal.NeighboursPorts[receivedFromPort] = true;
            var isAllReceived = existingSignal.NeighboursPorts.All(s => s.Value);
            if (isAllReceived)
            {
                SendSignal(waveId, existingSignal.ReceivedFromPort);
            }
        }

        var neighboursPorts = GetNeighboursPorts(receivedFromPort).ToList();
        ReceivedSignals.Add(new Signal(waveId, receivedFromPort, neighboursPorts));
        foreach (var neighbourPort in neighboursPorts)
        {
            SendSignal(waveId, neighbourPort);
        }

        return;
    }

    private Task OnWaveEchoReceived(Wave wave, int receivedFromPort)
    {
        wave.OnEchoReceived(receivedFromPort);
        var isCompleted = wave.IsAllEchoReceived();
        if (isCompleted)
        {
            _logger.LogInformation("Completed");
        }
    }
    
    

    private void SendSignal(Guid waveId, int receiverPort)
    {
    }

    private IEnumerable<int> GetNeighboursPorts(int except)
    {
    }
}

public class Signal
{
    public Signal(Guid waveId, int receivedFromPort, IReadOnlyCollection<int> neighboursPorts)
    {
        WaveId = waveId;
        ReceivedFromPort = receivedFromPort;
        NeighboursPorts = neighboursPorts;
    }

    public Guid WaveId { get; }
    public int ReceivedFromPort { get; }
    public IDictionary<int, bool> NeighboursPorts { get; }
}