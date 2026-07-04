using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Client.Core;

internal sealed class GameContext
{
    public World World { get; }

    public Map? CurrentMap { get; set; }

    public long LocalPlayerId { get; set; }

    public EntityId? LocalPlayerEntity => GetNetworkEntity(LocalPlayerId);

    private readonly Dictionary<long, EntityId> _entityById = [];

    internal GameContext(DefinitionCatalog catalog)
    {
        World = new World(catalog, enableDirtyTracking: false);
    }

    public void RegisterNetworkEntity(long id, EntityId entity) => _entityById[id] = entity;

    public void UnregisterNetworkEntity(long id) => _entityById.Remove(id);

    public EntityId? GetNetworkEntity(long id) => _entityById.TryGetValue(id, out var e) ? e : null;

    public void Reset()
    {
        World.Clear();
        _entityById.Clear();
        CurrentMap = null;
        LocalPlayerId = 0;
    }
}
