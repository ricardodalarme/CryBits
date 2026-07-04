using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Client.Worlds;

internal sealed class GameContext
{
    public static GameContext Instance { get; } = new();

    public World World { get; } = new(enableDirtyTracking: false);

    public Map? CurrentMap { get; set; }

    public LocalPlayer LocalPlayer { get; set; }

    private readonly Dictionary<long, EntityId> _entityById = [];

    internal GameContext()
    {
        LocalPlayer = new LocalPlayer(World, null);
    }

    public void RegisterNetworkEntity(long id, EntityId entity) => _entityById[id] = entity;

    public void UnregisterNetworkEntity(long id) => _entityById.Remove(id);

    public EntityId? GetNetworkEntity(long id) => _entityById.TryGetValue(id, out var e) ? e : null;

    public void Reset()
    {
        World.Clear();
        _entityById.Clear();
        CurrentMap = null;
        LocalPlayer = new LocalPlayer(World, null);
    }
}
