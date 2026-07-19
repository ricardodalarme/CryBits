using CryBits.Simulation.Core;

namespace CryBits.Client.Replication;

internal sealed class ReplicationState(long localPlayerId)
{
    public long LocalPlayerId { get; } = localPlayerId;

    public EntityId? LocalPlayerEntity => GetNetworkEntity(LocalPlayerId);

    private readonly Dictionary<long, EntityId> _entityById = [];

    public void RegisterNetworkEntity(long id, EntityId entity) => _entityById[id] = entity;

    public void UnregisterNetworkEntity(long id) => _entityById.Remove(id);

    public EntityId? GetNetworkEntity(long id) => _entityById.TryGetValue(id, out var e) ? e : null;

    public long LastAppliedServerTick { get; set; }

    public void RequestKeyframe()
    {
        LastAppliedServerTick = 0;
    }
}
