using CryBits.Definitions;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using CryBits.Transport;

namespace CryBits.Host.Network.Senders;

internal sealed class NpcSender(PackageSender packageSender, EntityRegistry entities)
{
    public void MapNpcs(EntityId entityId, MapState mapState)
    {
        var packet = new MapNpcsPacket { Npcs = new PacketsMapNpc[mapState.NpcIds.Count] };
        for (byte i = 0; i < mapState.NpcIds.Count; i++)
        {
            var npcId = mapState.NpcIds[i];
            var npcState = entities.Get(npcId)!.Get<NpcState>()!;
            var pos = entities.Get(npcId)!.Get<Position>()!;
            var vitals = entities.Get(npcId)!.Get<Vitals>()!;
            packet.Npcs[i] = new PacketsMapNpc
            {
                InstanceId = npcId.Value,
                NpcId = npcState.NpcDefId,
                X = pos.X,
                Y = pos.Y,
                Direction = (byte)pos.Direction,
                Vital = new short[(byte)Vital.Count]
            };
            for (byte n = 0; n < (byte)Vital.Count; n++) packet.Npcs[i].Vital[n] = n == 0 ? vitals.Hp : vitals.Mp;
        }

        packageSender.ToPlayer(entityId, packet);
    }

    public void MapNpc(EntityId entityId)
    {
        var entity = entities.Get(entityId)!;
        var npcState = entity.Get<NpcState>()!;
        var pos = entity.Get<Position>()!;
        var vitals = entity.Get<Vitals>()!;
        var packet = new MapNpcPacket
        {
            InstanceId = entityId.Value,
            NpcId = npcState.NpcDefId,
            X = pos.X,
            Y = pos.Y,
            Direction = (byte)pos.Direction,
            Vital = new short[(byte)Vital.Count]
        };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = n == 0 ? vitals.Hp : vitals.Mp;
        packageSender.ToMap(pos.MapId, packet, DeliveryChannel.ReliableUnordered);
    }

    public void MapNpcMovement(EntityId entityId, byte movement)
    {
        var entity = entities.Get(entityId)!;
        var pos = entity.Get<Position>()!;
        var speed = movement == (byte)Movement.Moving
            ? Globals.RunSpeedPixelsPerSecond
            : Globals.WalkSpeedPixelsPerSecond;

        packageSender.ToMap(pos.MapId,
            new MapNpcMovementPacket
            {
                InstanceId = entityId.Value,
                X = pos.X,
                Y = pos.Y,
                Direction = (byte)pos.Direction,
                Movement = movement,
                Speed = speed
            }, DeliveryChannel.Sequenced);
    }

    public void MapNpcDirection(EntityId entityId)
    {
        var pos = entities.Get(entityId)!.Get<Position>()!;
        packageSender.ToMap(pos.MapId,
            new MapNpcDirectionPacket { InstanceId = entityId.Value, Direction = (byte)pos.Direction },
            DeliveryChannel.Sequenced);
    }

    public void MapNpcVitals(EntityId entityId)
    {
        var entity = entities.Get(entityId)!;
        var vitals = entity.Get<Vitals>()!;
        var pos = entity.Get<Position>()!;
        var packet = new MapNpcVitalsPacket { InstanceId = entityId.Value, Vital = new short[(byte)Vital.Count] };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = n == 0 ? vitals.Hp : vitals.Mp;
        packageSender.ToMap(pos.MapId, packet, DeliveryChannel.ReliableSequenced);
    }

    public void MapNpcDied(Guid mapId, EntityId entityId)
    {
        packageSender.ToMap(mapId, new MapNpcDiedPacket { InstanceId = entityId.Value },
            DeliveryChannel.ReliableUnordered);
    }
}
