using ClientSdk;

namespace Core.Clients.Echo;

public class HttpEchoClient : IEchoClient
{
    private readonly HttpClient _httpClient;

    public HttpEchoClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task SendSignalAsync(Guid waveId, int sender, int receiverPort)
    {
        var client = new Client(default, _httpClient)
        {
            BaseUrl = $"http://localhost:{receiverPort}"
        };
        return client.EchoGETAsync(waveId, sender);
    }
}