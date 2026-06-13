using CryBits.Definitions;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Network.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Simulation.Components;
using CryBits.Server.World;
using LiteNetLib;
using CryBits.Simulation.State;

namespace CryBits.Server.Network.Senders;

internal sealed class NpcSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static NpcSender Instance { get; } = new(PackageSender.Instance, DefinitionCatalog.Instance);

    public void Npcs(GameSession session)
    {
        packageSender.ToPlayer(session, new NpcsPacket { List = _catalog.Npcs });
    }

    public void MapNpcs(EntityId entityId, MapInstance mapInstance)
    {
        var entities = GameWorld.Current.Entities;
        var packet = new MapNpcsPacket { Npcs = new PacketsMapNpc[mapInstance.NpcIds.Count] };
        for (byte i = 0; i < mapInstance.NpcIds.Count; i++)
        {
            var npcId = mapInstance.NpcIds[i];
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
        var entity = GameWorld.Current.Entities.Get(entityId)!;
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
        packageSender.ToMap(pos.MapId, packet, DeliveryMethod.ReliableUnordered);
    }

    public void MapNpcMovement(EntityId entityId, byte movement)
    {
        var entity = GameWorld.Current.Entities.Get(entityId)!;
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
            }, DeliveryMethod.Sequenced);
    }

    public void MapNpcDirection(EntityId entityId)
    {
        var pos = GameWorld.Current.Entities.Get(entityId)!.Get<Position>()!;
        packageSender.ToMap(pos.MapId,
            new MapNpcDirectionPacket { InstanceId = entityId.Value, Direction = (byte)pos.Direction },
            DeliveryMethod.Sequenced);
    }

    public void MapNpcVitals(EntityId entityId)
    {
        var entity = GameWorld.Current.Entities.Get(entityId)!;
        var vitals = entity.Get<Vitals>()!;
        var pos = entity.Get<Position>()!;
        var packet = new MapNpcVitalsPacket { InstanceId = entityId.Value, Vital = new short[(byte)Vital.Count] };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = n == 0 ? vitals.Hp : vitals.Mp;
        packageSender.ToMap(pos.MapId, packet, DeliveryMethod.ReliableSequenced);
    }

    public void MapNpcDied(EntityId entityId)
    {
        var pos = GameWorld.Current.Entities.Get(entityId)!.Get<Position>()!;
        packageSender.ToMap(pos.MapId, new MapNpcDiedPacket { InstanceId = entityId.Value },
            DeliveryMethod.ReliableUnordered);
    }
}
