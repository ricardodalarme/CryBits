namespace CryBits.Simulation.Events;

public abstract record SimEvent
{
    public long TickNumber { get; set; }
}
