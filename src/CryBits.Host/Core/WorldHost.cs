using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using System;
using System.Collections.Generic;

namespace CryBits.Host.Core;

internal sealed class WorldHost
{
    public static WorldHost Current { get; private set; } = null!;

    public World Simulation { get; } = new();

    public Dictionary<Guid, MapState> Maps => Simulation.Maps;
    public EntityRegistry Entities => Simulation.Entities;
    public DirtyTracking Dirty => Simulation.Dirty;
    public Tick? CurrentTick
    {
        get => Simulation.CurrentTick;
        set => Simulation.CurrentTick = value;
    }

    public List<GameSession> Sessions { get; } = [];
    public SessionMap SessionMap { get; } = new();

    public EntityId? FindPlayer(string name) => Simulation.FindPlayer(name);

    public WorldHost() => Current = this;
}
