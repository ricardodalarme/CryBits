namespace CryBits.Simulation.Core;

public sealed class DirtyTracking
{
    private readonly HashSet<EntityId> _dirty = [];

    public void Mark(EntityId id) => _dirty.Add(id);

    public IReadOnlySet<EntityId> All => _dirty;

    public void Clear() => _dirty.Clear();
}
