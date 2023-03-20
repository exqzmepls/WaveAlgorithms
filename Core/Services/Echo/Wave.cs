namespace Core.Services.Echo;

public class Wave
{
    private readonly IDictionary<int, bool> _echos = new Dictionary<int, bool>();

    public Wave(IEnumerable<int> neighboursPorts)
    {
        Id = Guid.NewGuid();
        foreach (var neighbourPort in neighboursPorts)
        {
            _echos.Add(neighbourPort, false);
        }
    }

    public Guid Id { get; }

    public void OnEchoReceived(int neighbourPort)
    {
        _echos[neighbourPort] = true;
    }

    public bool IsAllEchoReceived()
    {
        var result = _echos.All(e => e.Value);
        return result;
    }
}