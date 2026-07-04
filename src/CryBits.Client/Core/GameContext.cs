using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Client.Core;

internal sealed class GameContext(DefinitionCatalog catalog, long localPlayerId)
{
    public World World { get; } = new(catalog, enableDirtyTracking: false);

    public Map? CurrentMap { get; set; }

    public long LocalPlayerId { get; } = localPlayerId;

    public EntityId? LocalPlayerEntity => GetNetworkEntity(LocalPlayerId);

    private readonly Dictionary<long, EntityId> _entityById = [];

    public void RegisterNetworkEntity(long id, EntityId entity) => _entityById[id] = entity;

    public void UnregisterNetworkEntity(long id) => _entityById.Remove(id);

    public EntityId? GetNetworkEntity(long id) => _entityById.TryGetValue(id, out var e) ? e : null;
}
