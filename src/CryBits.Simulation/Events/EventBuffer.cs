namespace CryBits.Simulation.Events;

public sealed class EventBuffer
{
    private readonly List<SimEvent> _events = [];
    private long _tickNumber;

    public long TickNumber
    {
        set => _tickNumber = value;
    }

    public IReadOnlyList<SimEvent> Events => _events;

    public void Emit(SimEvent ev)
    {
        ev.TickNumber = _tickNumber;
        _events.Add(ev);
    }

    public void Clear() => _events.Clear();
}
