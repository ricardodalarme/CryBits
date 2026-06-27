namespace CryBits.Simulation.Events;

public sealed class EventBuffer
{
    private readonly List<SimEvent> _events = [];

    public IReadOnlyList<SimEvent> Events => _events;

    public void Emit(SimEvent ev)
    {
        _events.Add(ev);
    }

    public void Clear() => _events.Clear();
}
