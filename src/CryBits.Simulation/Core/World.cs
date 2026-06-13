using CryBits.Simulation.Components;
using CryBits.Simulation.State;
using System;
using System.Collections.Generic;

namespace CryBits.Simulation.Core;

public sealed class World
{
    public Dictionary<Guid, MapState> Maps { get; } = [];
    public long TickCount { get; set; }
    public EntityRegistry Entities { get; } = new();
    public DirtyTracking Dirty { get; } = new();

    public EntityId? FindPlayer(string name)
    {
        foreach (var state in Entities.All)
        {
            if (!state.Has<PlayerTag>()) continue;
            var appearance = state.Get<PlayerAppearance>()!;
            if (appearance.Name.Equals(name))
                return state.Id;
        }
        return null;
    }

    public EntityId? FindPlayer(EntityId id)
    {
        var state = Entities.Get(id);
        if (state != null && state.Has<PlayerTag>()) return id;
        return null;
    }

    public EntityId? FindNpcInstance(EntityId id)
    {
        var state = Entities.Get(id);
        if (state != null && state.Has<NpcTag>()) return id;
        return null;
    }
}
