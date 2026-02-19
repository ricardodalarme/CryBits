using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class NpcSender
{
    public static void Npcs(Account account)
    {
        var data = new NetDataWriter();
        data.Put((byte)ServerPacket.Npcs);
        data.WriteObject(Npc.List);
        Send.ToPlayer(account, data);
    }

    public static void MapNpcs(Player player, TempMap map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcs);
        data.Put((short)map.Npc.Length);
        for (byte i = 0; i < map.Npc.Length; i++)
        {
            data.PutGuid(map.Npc[i].Data.GetId());
            data.Put(map.Npc[i].X);
            data.Put(map.Npc[i].Y);
            data.Put((byte)map.Npc[i].Direction);
            for (byte n = 0; n < (byte)Vital.Count; n++) data.Put(map.Npc[i].Vital[n]);
        }

        Send.ToPlayer(player, data);
    }

    public static void MapNpc(TempNpc npc)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpc);
        data.Put(npc.Index);
        data.PutGuid(npc.Data.GetId());
        data.Put(npc.X);
        data.Put(npc.Y);
        data.Put((byte)npc.Direction);
        for (byte n = 0; n < (byte)Vital.Count; n++) data.Put(npc.Vital[n]);
        Send.ToMap(npc.Map, data);
    }

    public static void MapNpcMovement(TempNpc npc, byte movement)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcMovement);
        data.Put(npc.Index);
        data.Put(npc.X);
        data.Put(npc.Y);
        data.Put((byte)npc.Direction);
        data.Put(movement);
        Send.ToMap(npc.Map, data);
    }

    public static void MapNpcDirection(TempNpc npc)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcDirection);
        data.Put(npc.Index);
        data.Put((byte)npc.Direction);
        Send.ToMap(npc.Map, data);
    }

    public static void MapNpcVitals(TempNpc npc)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcVitals);
        data.Put(npc.Index);
        for (byte n = 0; n < (byte)Vital.Count; n++) data.Put(npc.Vital[n]);
        Send.ToMap(npc.Map, data);
    }

    public static void MapNpcAttack(TempNpc npc, string victim = "", Target victimType = 0)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcAttack);
        data.Put(npc.Index);
        data.Put(victim);
        data.Put((byte)victimType);
        Send.ToMap(npc.Map, data);
    }

    public static void MapNpcDied(TempNpc npc)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.MapNpcDied);
        data.Put(npc.Index);
        Send.ToMap(npc.Map, data);
    }
}
