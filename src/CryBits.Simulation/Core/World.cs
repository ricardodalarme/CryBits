using CryBits.Simulation.Components;
using CryBits.Simulation.State;
using System;
using System.Collections.Generic;

namespace CryBits.Simulation.Core;

public sealed class World
{
    public Dictionary<Guid, MapState> Maps { get; } = [];
    public EntityRegistry Entities { get; } = new();
    public DirtyTracking Dirty { get; } = new();
    public Tick? CurrentTick { get; set; }

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

    public EntityId? FindPlayerByValue(Guid id)
    {
        var entityId = new EntityId(id);
        var state = Entities.Get(entityId);
        if (state != null && state.Has<PlayerTag>()) return entityId;
        return null;
    }

    public EntityId? FindNpcInstance(Guid id)
    {
        var entityId = new EntityId(id);
        var state = Entities.Get(entityId);
        if (state != null && state.Has<NpcTag>()) return entityId;
        return null;
    }
}
