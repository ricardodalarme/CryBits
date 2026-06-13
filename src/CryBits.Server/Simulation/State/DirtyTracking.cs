using System;
using System.Collections.Generic;

namespace CryBits.Server.Simulation.State;

internal sealed class DirtyTracking
{
    private readonly HashSet<(EntityId, Type)> _dirty = [];

    public void Mark<T>(EntityId id) where T : class
    {
        _dirty.Add((id, typeof(T)));
    }

    public IReadOnlyCollection<(EntityId EntityId, Type ComponentType)> All => _dirty;

    public void Clear() => _dirty.Clear();
}
