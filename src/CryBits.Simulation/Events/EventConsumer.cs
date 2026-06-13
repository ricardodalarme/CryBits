namespace CryBits.Simulation.Events;

/// <summary>
/// Dispatches buffered simulation events to network senders.
/// </summary>
internal sealed class EventConsumer
{
    public void Flush(EventBuffer buffer)
    {
        buffer.Clear();
    }
}
