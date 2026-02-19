using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal static class PlayerSender
{
    public static void Join(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Join);
        data.Put(player.Name);
        Send.ToPlayer(player, data);
    }

    public static void JoinGame(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.JoinGame);
        Send.ToPlayer(player, data);
    }

    public static void JoinMap(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.JoinMap);
        Send.ToPlayer(player, data);
    }

    public static void PlayerLeaveMap(Player player, TempMap map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerLeave);
        data.Put(player.Name);
        Send.ToMapBut(map, player, data);
    }

    public static void PlayerPosition(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerPosition);
        data.Put(player.Name);
        data.Put(player.X);
        data.Put(player.Y);
        data.Put((byte)player.Direction);
        Send.ToMap(player.Map, data);
    }

    public static void PlayerVitals(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerVitals);
        data.Put(player.Name);
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            data.Put(player.Vital[i]);
            data.Put(player.MaxVital(i));
        }

        Send.ToMap(player.Map, data);
    }

    public static void PlayerLeave(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerLeave);
        data.Put(player.Name);
        Send.ToAllBut(player, data);
    }

    public static void PlayerMove(Player player, byte movement)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerMove);
        data.Put(player.Name);
        data.Put(player.X);
        data.Put(player.Y);
        data.Put((byte)player.Direction);
        data.Put(movement);
        Send.ToMapBut(player.Map, player, data);
    }

    public static void PlayerDirection(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerDirection);
        data.Put(player.Name);
        data.Put((byte)player.Direction);
        Send.ToMapBut(player.Map, player, data);
    }

    public static void PlayerExperience(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerExperience);
        data.Put(player.Experience);
        data.Put(player.ExpNeeded);
        data.Put(player.Points);
        Send.ToPlayer(player, data);
    }

    public static void PlayerAttack(Player player, string victim = "", Target victimType = 0)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerAttack);
        data.Put(player.Name);
        data.Put(victim);
        data.Put((byte)victimType);
        Send.ToMap(player.Map, data);
    }

    public static void PlayerEquipments(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerEquipments);
        data.Put(player.Name);
        for (byte i = 0; i < (byte)Equipment.Count; i++) data.PutGuid(player.Equipment[i].GetId());
        Send.ToMap(player.Map, data);
    }

    public static void PlayerInventory(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerInventory);
        for (byte i = 0; i < MaxInventory; i++)
        {
            data.PutGuid(player.Inventory[i].Item.GetId());
            data.Put(player.Inventory[i].Amount);
        }

        Send.ToPlayer(player, data);
    }

    public static void PlayerHotbar(Player player)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.PlayerHotbar);
        for (byte i = 0; i < MaxHotbar; i++)
        {
            data.Put((byte)player.Hotbar[i].Type);
            data.Put(player.Hotbar[i].Slot);
        }

        Send.ToPlayer(player, data);
    }
}