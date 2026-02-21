using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal static class PlayerSender
{
    public static void Join(Player player)
    {
        Send.ToPlayer(player, ServerPacket.Join, new JoinPacket { Name = player.Name });
    }

    public static void JoinGame(Player player)
    {
        Send.ToPlayer(player, ServerPacket.JoinGame, new JoinGamePacket());
    }

    public static void JoinMap(Player player)
    {
        Send.ToPlayer(player, ServerPacket.JoinMap, new JoinMapPacket());
    }

    public static void PlayerLeaveMap(Player player, TempMap map)
    {
        Send.ToMapBut(map, player, ServerPacket.PlayerLeave, new PlayerLeavePacket { Name = player.Name });
    }

    public static void PlayerPosition(Player player)
    {
        Send.ToMap(player.Map, ServerPacket.PlayerPosition,
            new PlayerPositionPacket
            { Name = player.Name, X = player.X, Y = player.Y, Direction = (byte)player.Direction });
    }

    public static void PlayerVitals(Player player)
    {
        var packet = new PlayerVitalsPacket
        { Name = player.Name, Vital = new short[(byte)Vital.Count], MaxVital = new short[(byte)Vital.Count] };
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            packet.Vital[i] = player.Vital[i];
            packet.MaxVital[i] = player.MaxVital(i);
        }

        Send.ToMap(player.Map, ServerPacket.PlayerVitals, packet);
    }

    public static void PlayerLeave(Player player)
    {
        Send.ToAllBut(player, ServerPacket.PlayerLeave, new PlayerLeavePacket { Name = player.Name });
    }

    public static void PlayerMove(Player player, byte movement)
    {
        Send.ToMapBut(player.Map, player, ServerPacket.PlayerMove,
            new PlayerMovePacket
            {
                Name = player.Name,
                X = player.X,
                Y = player.Y,
                Direction = (byte)player.Direction,
                Movement = movement
            });
    }

    public static void PlayerDirection(Player player)
    {
        Send.ToMapBut(player.Map, player, ServerPacket.PlayerDirection,
            new PlayerDirectionPacket { Name = player.Name, Direction = (byte)player.Direction });
    }

    public static void PlayerExperience(Player player)
    {
        Send.ToPlayer(player, ServerPacket.PlayerExperience,
            new PlayerExperiencePacket
            { Experience = player.Experience, ExpNeeded = player.ExpNeeded, Points = player.Points });
    }

    public static void PlayerAttack(Player player, string victim = "", Target victimType = 0)
    {
        Send.ToMap(player.Map, ServerPacket.PlayerAttack,
            new PlayerAttackPacket { Name = player.Name, Victim = victim, VictimType = (byte)victimType });
    }

    public static void PlayerEquipments(Player player)
    {
        var packet = new PlayerEquipmentsPacket
        { Name = player.Name, Equipments = new System.Guid[(byte)Equipment.Count] };
        for (byte i = 0; i < (byte)Equipment.Count; i++) packet.Equipments[i] = player.Equipment[i].GetId();
        Send.ToMap(player.Map, ServerPacket.PlayerEquipments, packet);
    }

    public static void PlayerInventory(Player player)
    {
        var packet = new PlayerInventoryPacket
        { ItemIds = new System.Guid[MaxInventory], Amounts = new short[MaxInventory] };
        for (byte i = 0; i < MaxInventory; i++)
        {
            packet.ItemIds[i] = player.Inventory[i].Item.GetId();
            packet.Amounts[i] = player.Inventory[i].Amount;
        }

        Send.ToPlayer(player, ServerPacket.PlayerInventory, packet);
    }

    public static void PlayerHotbar(Player player)
    {
        var packet = new PlayerHotbarPacket { Types = new byte[MaxHotbar], Slots = new byte[MaxHotbar] };
        for (byte i = 0; i < MaxHotbar; i++)
        {
            packet.Types[i] = (byte)player.Hotbar[i].Type;
            packet.Slots[i] = (byte)player.Hotbar[i].Slot;
        }

        Send.ToPlayer(player, ServerPacket.PlayerHotbar, packet);
    }
}
