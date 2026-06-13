using CryBits.Definitions;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Network.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;
using LiteNetLib;

namespace CryBits.Server.Network.Senders;

internal sealed class NpcSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static NpcSender Instance { get; } = new(PackageSender.Instance, DefinitionCatalog.Instance);

    public void Npcs(GameSession session)
    {
        packageSender.ToPlayer(session, new NpcsPacket { List = _catalog.Npcs });
    }

    public void MapNpcs(Player player, MapInstance mapInstance)
    {
        var packet = new MapNpcsPacket { Npcs = new PacketsMapNpc[mapInstance.Npc.Length] };
        for (byte i = 0; i < mapInstance.Npc.Length; i++)
        {
            packet.Npcs[i] = new PacketsMapNpc
            {
                InstanceId = mapInstance.Npc[i].Id,
                NpcId = mapInstance.Npc[i].Data.GetId(),
                X = mapInstance.Npc[i].X,
                Y = mapInstance.Npc[i].Y,
                Direction = (byte)mapInstance.Npc[i].Direction,
                Vital = new short[(byte)Vital.Count]
            };
            for (byte n = 0; n < (byte)Vital.Count; n++) packet.Npcs[i].Vital[n] = mapInstance.Npc[i].Vital[n];
        }

        packageSender.ToPlayer(player, packet);
    }

    public void MapNpc(NpcInstance npcInstance)
    {
        var packet = new MapNpcPacket
        {
            InstanceId = npcInstance.Id,
            NpcId = npcInstance.Data.GetId(),
            X = npcInstance.X,
            Y = npcInstance.Y,
            Direction = (byte)npcInstance.Direction,
            Vital = new short[(byte)Vital.Count]
        };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = npcInstance.Vital[n];
        packageSender.ToMap(npcInstance.MapInstance.Id, packet, DeliveryMethod.ReliableUnordered);
    }

    public void MapNpcMovement(NpcInstance npcInstance, byte movement)
    {
        var speed = movement == (byte)Movement.Moving
            ? Globals.RunSpeedPixelsPerSecond
            : Globals.WalkSpeedPixelsPerSecond;

        packageSender.ToMap(npcInstance.MapInstance.Id,
            new MapNpcMovementPacket
            {
                InstanceId = npcInstance.Id,
                X = npcInstance.X,
                Y = npcInstance.Y,
                Direction = (byte)npcInstance.Direction,
                Movement = movement,
                Speed = speed
            }, DeliveryMethod.Sequenced);
    }

    public void MapNpcDirection(NpcInstance npcInstance)
    {
        packageSender.ToMap(npcInstance.MapInstance.Id,
            new MapNpcDirectionPacket { InstanceId = npcInstance.Id, Direction = (byte)npcInstance.Direction },
            DeliveryMethod.Sequenced);
    }

    public void MapNpcVitals(NpcInstance npcInstance)
    {
        var packet = new MapNpcVitalsPacket { InstanceId = npcInstance.Id, Vital = new short[(byte)Vital.Count] };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = npcInstance.Vital[n];
        packageSender.ToMap(npcInstance.MapInstance.Id, packet, DeliveryMethod.ReliableSequenced);
    }

    public void MapNpcDied(NpcInstance npcInstance)
    {
        packageSender.ToMap(npcInstance.MapInstance.Id, new MapNpcDiedPacket { InstanceId = npcInstance.Id },
            DeliveryMethod.ReliableUnordered);
    }
}
