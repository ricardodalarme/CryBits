using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal static class NpcSender
{
    public static void Npcs(GameSession session)
    {
        PackageSender.ToPlayer(session, new NpcsPacket { List = Npc.List });
    }

    public static void MapNpcs(Player player, MapInstance mapInstance)
    {
        var packet = new MapNpcsPacket { Npcs = new PacketsMapNpc[mapInstance.Npc.Length] };
        for (byte i = 0; i < mapInstance.Npc.Length; i++)
        {
            packet.Npcs[i] = new PacketsMapNpc
            {
                NpcId = mapInstance.Npc[i].Data.GetId(),
                X = mapInstance.Npc[i].X,
                Y = mapInstance.Npc[i].Y,
                Direction = (byte)mapInstance.Npc[i].Direction,
                Vital = new short[(byte)Vital.Count]
            };
            for (byte n = 0; n < (byte)Vital.Count; n++) packet.Npcs[i].Vital[n] = mapInstance.Npc[i].Vital[n];
        }

        PackageSender.ToPlayer(player, packet);
    }

    public static void MapNpc(NpcInstance npcInstance)
    {
        var packet = new MapNpcPacket
        {
            Index = npcInstance.Index,
            NpcId = npcInstance.Data.GetId(),
            X = npcInstance.X,
            Y = npcInstance.Y,
            Direction = (byte)npcInstance.Direction,
            Vital = new short[(byte)Vital.Count]
        };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = npcInstance.Vital[n];
        PackageSender.ToMap(npcInstance.MapInstance, packet);
    }

    public static void MapNpcMovement(NpcInstance npcInstance, byte movement)
    {
        var speed = movement == (byte)Movement.Moving
            ? Globals.RunSpeedPixelsPerSecond
            : Globals.WalkSpeedPixelsPerSecond;

        PackageSender.ToMap(npcInstance.MapInstance,
            new MapNpcMovementPacket
            {
                Index = npcInstance.Index,
                X = npcInstance.X,
                Y = npcInstance.Y,
                Direction = (byte)npcInstance.Direction,
                Movement = movement,
                Speed = speed
            });
    }

    public static void MapNpcDirection(NpcInstance npcInstance)
    {
        PackageSender.ToMap(npcInstance.MapInstance,
            new MapNpcDirectionPacket { Index = npcInstance.Index, Direction = (byte)npcInstance.Direction });
    }

    public static void MapNpcVitals(NpcInstance npcInstance)
    {
        var packet = new MapNpcVitalsPacket { Index = npcInstance.Index, Vital = new short[(byte)Vital.Count] };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = npcInstance.Vital[n];
        PackageSender.ToMap(npcInstance.MapInstance, packet);
    }

    public static void MapNpcAttack(NpcInstance npcInstance, string victim = "", Target victimType = 0)
    {
        PackageSender.ToMap(npcInstance.MapInstance,
            new MapNpcAttackPacket { Index = npcInstance.Index, Victim = victim, VictimType = (byte)victimType });
    }

    public static void MapNpcDied(NpcInstance npcInstance)
    {
        PackageSender.ToMap(npcInstance.MapInstance, new MapNpcDiedPacket { Index = npcInstance.Index });
    }
}
