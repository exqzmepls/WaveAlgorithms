namespace Core.Configuration;

public class Node
{
    public int Endpoint { get; init; }

    public IEnumerable<int> Neighbours { get; init; } = null!;
}