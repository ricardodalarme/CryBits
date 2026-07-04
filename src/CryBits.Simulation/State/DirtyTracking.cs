namespace CryBits.Simulation.State;

public sealed class DirtyTracking
{
    private readonly HashSet<(EntityId, Type)> _dirty = [];

    public void Mark<T>(EntityId id) where T : class
    {
        Mark(id, typeof(T));
    }

    public void Mark(EntityId id, Type type)
    {
        _dirty.Add((id, type));
    }

    public IReadOnlyCollection<(EntityId EntityId, Type ComponentType)> All => _dirty;

    public void Clear() => _dirty.Clear();
}
