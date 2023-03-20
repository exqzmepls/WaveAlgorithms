namespace Core.Services.Echo;

public class Wave
{
    private readonly IDictionary<int, bool> _echo = new Dictionary<int, bool>();

    private Wave(IEnumerable<int> neighboursPorts) : this(Guid.NewGuid(), default, neighboursPorts)
    {
    }

    private Wave(Guid id, int? echoPort, IEnumerable<int> neighboursPorts)
    {
        EchoPort = echoPort;
        Id = id;
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

    public static Wave Create(Guid id, int echoPort, IEnumerable<int> neighboursPorts)
    {
        return new Wave(id, echoPort, neighboursPorts);
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