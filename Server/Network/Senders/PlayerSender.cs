using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using static CryBits.Globals;

namespace CryBits.Server.Network.Senders;

internal sealed class PlayerSender(PackageSender packageSender)
{
    public static PlayerSender Instance { get; } = new(PackageSender.Instance);

    public void Join(Player player)
    {
        packageSender.ToPlayer(player, new JoinPacket { PlayerId = player.Id });
    }

    public void JoinGame(Player player)
    {
        packageSender.ToPlayer(player, new JoinGamePacket());
    }

    public void JoinMap(Player player)
    {
        packageSender.ToPlayer(player, new JoinMapPacket());
    }

    public void PlayerLeaveMap(Player player, MapInstance mapInstance)
    {
        packageSender.ToMapBut(mapInstance, player, new PlayerLeavePacket { NetworkId = player.Id });
    }

    public void PlayerPosition(Player player)
    {
        packageSender.ToMap(player.MapInstance,
            new PlayerPositionPacket
            { NetworkId = player.Id, X = player.X, Y = player.Y, Direction = (byte)player.Direction });
    }

    public void PlayerVitals(Player player)
    {
        var packet = new PlayerVitalsPacket
        { NetworkId = player.Id, Vital = new short[(byte)Vital.Count], MaxVital = new short[(byte)Vital.Count] };
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            packet.Vital[i] = player.Vital[i];
            packet.MaxVital[i] = player.MaxVital(i);
        }

        packageSender.ToMap(player.MapInstance, packet);
    }

    public void PlayerLeave(Player player)
    {
        packageSender.ToAllBut(player, new PlayerLeavePacket { NetworkId = player.Id });
    }

    public void PlayerMove(Player player, byte movement)
    {
        var speed = movement == (byte)Movement.Moving
            ? RunSpeedPixelsPerSecond
            : WalkSpeedPixelsPerSecond;

        packageSender.ToMapBut(player.MapInstance, player,
            new PlayerMovePacket
            {
                NetworkId = player.Id,
                X = player.X,
                Y = player.Y,
                Direction = (byte)player.Direction,
                Movement = movement,
                Speed = speed
            });
    }

    public void PlayerDirection(Player player)
    {
        packageSender.ToMapBut(player.MapInstance, player,
            new PlayerDirectionPacket { NetworkId = player.Id, Direction = (byte)player.Direction });
    }

    public void PlayerExperience(Player player)
    {
        packageSender.ToPlayer(player,
            new PlayerExperiencePacket
            { Experience = player.Experience, ExpNeeded = player.ExpNeeded, Points = player.Points });
    }

    public void PlayerEquipments(Player player)
    {
        var packet = new PlayerEquipmentsPacket
        { NetworkId = player.Id, Equipments = new System.Guid[(byte)Equipment.Count] };
        for (byte i = 0; i < (byte)Equipment.Count; i++) packet.Equipments[i] = player.Equipment[i].GetId();
        packageSender.ToMap(player.MapInstance, packet);
    }

    public void PlayerInventory(Player player)
    {
        var packet = new PlayerInventoryPacket
        { ItemIds = new System.Guid[MaxInventory], Amounts = new short[MaxInventory] };
        for (byte i = 0; i < MaxInventory; i++)
        {
            packet.ItemIds[i] = player.Inventory[i].Item.GetId();
            packet.Amounts[i] = player.Inventory[i].Amount;
        }

        packageSender.ToPlayer(player, packet);
    }

    public void PlayerHotbar(Player player)
    {
        var packet = new PlayerHotbarPacket { Types = new byte[MaxHotbar], Slots = new byte[MaxHotbar] };
        for (byte i = 0; i < MaxHotbar; i++)
        {
            packet.Types[i] = (byte)player.Hotbar[i].Type;
            packet.Slots[i] = (byte)player.Hotbar[i].Slot;
        }

        packageSender.ToPlayer(player, packet);
    }
}
