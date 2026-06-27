using CryBits.Host.Core;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using CryBits.Transport;
using CryBits.Transport.Abstractions;
using MemoryPack;

namespace CryBits.Host.Replication;

internal sealed class KeyframeReplicator(
    World world,
    SessionManager sessions,
    KeyframeEncoder encoder,
    EventFanout eventFanout,
    ITransport transport) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        if (world.Dirty == null) return;

        var hasDirty = world.Dirty.All.Count > 0;
        var hasEvents = tick.Events.Events.Count > 0;

        if (!hasDirty && !hasEvents) return;

        if (hasDirty) SendKeyframes();
        if (hasEvents) eventFanout.Fanout(tick, world);
        world.Dirty.Clear();
    }

    private void SendKeyframes()
    {
        var dirtyMaps = new HashSet<Guid>();
        foreach (var (entityId, _) in world.Dirty!.All)
        {
            var pos = world.Get<Position>(entityId);
            if (pos != null) dirtyMaps.Add(pos.MapId);
        }

        foreach (var session in sessions.Where(s => s.IsPlaying && s.Character.HasValue))
        {
            var observerId = session.Character!.Value;
            var pos = world.Get<Position>(observerId);
            if (pos == null) continue;

            if (!dirtyMaps.Contains(pos.MapId)) continue;

            var entities = GetEntitiesOnMap(pos.MapId);
            var packet = encoder.Encode(pos.MapId, entities);
            var bytes = MemoryPackSerializer.Serialize<IServerPacket>(packet);
            transport.Send(session.Id, bytes, DeliveryChannel.ReliableOrdered);
        }
    }

    private List<EntityId> GetEntitiesOnMap(Guid mapId)
    {
        var result = new List<EntityId>();
        foreach (var state in world.All)
        {
            var pos = state.Get<Position>();
            if (pos != null && pos.MapId == mapId)
                result.Add(state.Id);
        }
        return result;
    }
}
