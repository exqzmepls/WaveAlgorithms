namespace Core.Services.Echo;

public interface IEchoService
{
    public Task StartWaveAsync();

    public Task OnSignalAsync(Guid waveId, int receivedFromPort);
}