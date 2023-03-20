namespace Core.Services.Echo;

public class Wave
{
    private readonly IDictionary<int, bool> _echo = new Dictionary<int, bool>();

    private Wave(IEnumerable<int> neighboursPorts, int? echoPort = default)
    {
        EchoPort = echoPort;
        Id = Guid.NewGuid();
        foreach (var neighbourPort in neighboursPorts)
        {
            _echo.Add(neighbourPort, false);
        }
    }

    public Guid Id { get; }

    public int? EchoPort { get; }

    public static Wave CreateInitial(IEnumerable<int> neighboursPorts)
    {
        return new Wave(neighboursPorts);
    }

    public static Wave Create(int echoPort, IEnumerable<int> neighboursPorts)
    {
        return new Wave(neighboursPorts, echoPort);
    }

    public void OnEchoReceived(int neighbourPort)
    {
        _echo[neighbourPort] = true;
    }

    public bool IsAllEchoReceived()
    {
        var result = _echo.All(e => e.Value);
        return result;
    }
}