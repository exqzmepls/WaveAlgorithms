namespace Core.Configuration;

public interface INetConfigurationProvider
{
    public int GetEndpoint();

    public IReadOnlyCollection<int> GetNeighbours();

    public IReadOnlyCollection<int> GetNeighbours(int exclude);
}