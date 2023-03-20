using Microsoft.Extensions.Configuration;

namespace Core.Configuration;

public class NetConfigurationProvider : INetConfigurationProvider
{
    private readonly IConfiguration _configuration;

    public NetConfigurationProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public int GetEndpoint()
    {
        var endpoint = _configuration.GetValue<int>("Endpoint");
        return endpoint;
    }

    public IReadOnlyCollection<int> GetNeighbours()
    {
        var endpoint = GetEndpoint();
        var netNodes = _configuration.GetRequiredSection("Net").Get<IEnumerable<Node>>()!;

        var node = netNodes.Single(n => n.Endpoint == endpoint);
        return node.Neighbours.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<int> GetNeighbours(int exclude)
    {
        var neighbours = GetNeighbours();
        var result = neighbours.Where(n => n != exclude);
        return result.ToList().AsReadOnly();
    }
}