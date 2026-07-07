namespace CryBits.Host.Replication;

internal sealed class ObserverState
{
    public long LastAckedTick { get; set; }

    public HashSet<long> KnownEntities { get; } = [];
}
