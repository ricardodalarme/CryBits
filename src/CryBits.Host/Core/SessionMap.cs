using CryBits.Simulation.State;
using System.Collections.Generic;

namespace CryBits.Host.Core;

internal sealed class SessionMap
{
    private readonly Dictionary<EntityId, GameSession> _map = [];

    public void Register(EntityId id, GameSession s) => _map[id] = s;
    public void Unregister(EntityId id) => _map.Remove(id);
    public GameSession? Get(EntityId id) => _map.GetValueOrDefault(id);
}
