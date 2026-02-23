using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal static class PlayerSender
{
    public static void Join(Player player)
    {
        Send.ToPlayer(player, new JoinPacket { Name = player.Get<PlayerDataComponent>().Name });
    }

    public static void JoinGame(Player player)
    {
        Send.ToPlayer(player, new JoinGamePacket());
    }

    public static void JoinMap(Player player)
    {
        Send.ToPlayer(player, new JoinMapPacket());
    }

    public static void PlayerLeaveMap(Player player, MapInstance mapInstance)
    {
        Send.ToMapBut(mapInstance, player, new PlayerLeavePacket { Name = player.Get<PlayerDataComponent>().Name });
    }

    public static void PlayerPosition(Player player)
    {
        var pd  = player.Get<PlayerDataComponent>();
        var pos = player.Get<PositionComponent>();
        var dir = player.Get<DirectionComponent>();
        Send.ToMap(player.MapInstance,
            new PlayerPositionPacket { Name = pd.Name, X = pos.X, Y = pos.Y, Direction = (byte)dir.Value });
    }

    public static void PlayerVitals(Player player)
    {
        var pd     = player.Get<PlayerDataComponent>();
        var vitals = player.Get<VitalsComponent>();
        var packet = new PlayerVitalsPacket
        { Name = pd.Name, Vital = new short[(byte)Vital.Count], MaxVital = new short[(byte)Vital.Count] };
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            packet.Vital[i]    = vitals.Values[i];
            packet.MaxVital[i] = player.MaxVital(i);
        }

        Send.ToMap(player.MapInstance, packet);
    }

    public static void PlayerLeave(Player player)
    {
        Send.ToAllBut(player, new PlayerLeavePacket { Name = player.Get<PlayerDataComponent>().Name });
    }

    public static void PlayerMove(Player player, byte movement)
    {
        var pd  = player.Get<PlayerDataComponent>();
        var pos = player.Get<PositionComponent>();
        var dir = player.Get<DirectionComponent>();
        Send.ToMapBut(player.MapInstance, player,
            new PlayerMovePacket
            {
                Name      = pd.Name,
                X         = pos.X,
                Y         = pos.Y,
                Direction = (byte)dir.Value,
                Movement  = movement
            });
    }

    public static void PlayerDirection(Player player)
    {
        var pd  = player.Get<PlayerDataComponent>();
        var dir = player.Get<DirectionComponent>();
        Send.ToMapBut(player.MapInstance, player,
            new PlayerDirectionPacket { Name = pd.Name, Direction = (byte)dir.Value });
    }

    public static void PlayerExperience(Player player)
    {
        var pd = player.Get<PlayerDataComponent>();
        Send.ToPlayer(player,
            new PlayerExperiencePacket
            { Experience = pd.Experience, ExpNeeded = player.ExpNeeded, Points = pd.Points });
    }

    public static void PlayerAttack(Player player, string victim = "", Target victimType = 0)
    {
        Send.ToMap(player.MapInstance,
            new PlayerAttackPacket { Name = player.Get<PlayerDataComponent>().Name, Victim = victim, VictimType = (byte)victimType });
    }

    public static void PlayerEquipments(Player player)
    {
        var pd    = player.Get<PlayerDataComponent>();
        var equip = player.Get<EquipmentComponent>();
        var packet = new PlayerEquipmentsPacket
        { Name = pd.Name, Equipments = new System.Guid[(byte)Equipment.Count] };
        for (byte i = 0; i < (byte)Equipment.Count; i++) packet.Equipments[i] = equip.Slots[i].GetId();
        Send.ToMap(player.MapInstance, packet);
    }

    public static void PlayerInventory(Player player)
    {
        var inv    = player.Get<InventoryComponent>();
        var packet = new PlayerInventoryPacket
        { ItemIds = new System.Guid[MaxInventory], Amounts = new short[MaxInventory] };
        for (byte i = 0; i < MaxInventory; i++)
        {
            packet.ItemIds[i] = inv.Slots[i].Item.GetId();
            packet.Amounts[i] = inv.Slots[i].Amount;
        }

        Send.ToPlayer(player, packet);
    }

    public static void PlayerHotbar(Player player)
    {
        var hotbar = player.Get<HotbarComponent>();
        var packet = new PlayerHotbarPacket { Types = new byte[MaxHotbar], Slots = new byte[MaxHotbar] };
        for (byte i = 0; i < MaxHotbar; i++)
        {
            packet.Types[i] = (byte)hotbar.Slots[i].Type;
            packet.Slots[i] = (byte)hotbar.Slots[i].Slot;
        }

        Send.ToPlayer(player, packet);
    }
}
