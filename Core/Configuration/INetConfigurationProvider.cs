namespace Core.Configuration;

public interface INetConfigurationProvider
{
    public int GetEndpoint();

    public IEnumerable<int> GetNeighbours();

    public IEnumerable<int> GetNeighbours(int exclude);
}