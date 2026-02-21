using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;

namespace CryBits.Server.Network.Senders;

internal static class NpcSender
{
    public static void Npcs(Account account)
    {
        Send.ToPlayer(account, new NpcsPacket { List = Npc.List });
    }

    public static void MapNpcs(Player player, TempMap map)
    {
        var packet = new MapNpcsPacket { Npcs = new PacketsMapNpc[map.Npc.Length] };
        for (byte i = 0; i < map.Npc.Length; i++)
        {
            packet.Npcs[i] = new PacketsMapNpc
            {
                NpcId = map.Npc[i].Data.GetId(),
                X = map.Npc[i].X,
                Y = map.Npc[i].Y,
                Direction = (byte)map.Npc[i].Direction,
                Vital = new short[(byte)Vital.Count]
            };
            for (byte n = 0; n < (byte)Vital.Count; n++) packet.Npcs[i].Vital[n] = map.Npc[i].Vital[n];
        }

        Send.ToPlayer(player, packet);
    }

    public static void MapNpc(TempNpc npc)
    {
        var packet = new MapNpcPacket
        {
            Index = npc.Index,
            NpcId = npc.Data.GetId(),
            X = npc.X,
            Y = npc.Y,
            Direction = (byte)npc.Direction,
            Vital = new short[(byte)Vital.Count]
        };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = npc.Vital[n];
        Send.ToMap(npc.Map, packet);
    }

    public static void MapNpcMovement(TempNpc npc, byte movement)
    {
        Send.ToMap(npc.Map,
            new MapNpcMovementPacket
            { Index = npc.Index, X = npc.X, Y = npc.Y, Direction = (byte)npc.Direction, Movement = movement });
    }

    public static void MapNpcDirection(TempNpc npc)
    {
        Send.ToMap(npc.Map,
            new MapNpcDirectionPacket { Index = npc.Index, Direction = (byte)npc.Direction });
    }

    public static void MapNpcVitals(TempNpc npc)
    {
        var packet = new MapNpcVitalsPacket { Index = npc.Index, Vital = new short[(byte)Vital.Count] };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = npc.Vital[n];
        Send.ToMap(npc.Map, packet);
    }

    public static void MapNpcAttack(TempNpc npc, string victim = "", Target victimType = 0)
    {
        Send.ToMap(npc.Map,
            new MapNpcAttackPacket { Index = npc.Index, Victim = victim, VictimType = (byte)victimType });
    }

    public static void MapNpcDied(TempNpc npc)
    {
        Send.ToMap(npc.Map, new MapNpcDiedPacket { Index = npc.Index });
    }
}
