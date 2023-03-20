namespace Core.Clients.Echo;

public interface IEchoClient
{
    public Task SendSignalAsync(Guid waveId, int sender, int receiverPort);
}