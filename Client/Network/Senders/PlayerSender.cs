using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Enums;
using CryBits.Packets.Client;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.Network.Senders;

internal static class PlayerSender
{
    public static void PlayerDirection() => Send.Packet(ClientPacket.PlayerDirection, new PlayerDirectionPacket { Direction = (byte)Player.Me.Direction });

    public static void PlayerMove() => Send.Packet(ClientPacket.PlayerMove, new PlayerMovePacket { X = Player.Me.X, Y = Player.Me.Y, Movement = (byte)Player.Me.Movement });

    public static void PlayerAttack() => Send.Packet(ClientPacket.PlayerAttack, new PlayerAttackPacket());

    public static void AddPoint(Attribute attribute) => Send.Packet(ClientPacket.AddPoint, new AddPointPacket { Attribute = (byte)attribute });

    public static void CollectItem() => Send.Packet(ClientPacket.CollectItem, new CollectItemPacket());

    public static void DropItem(short slot, short amount) => Send.Packet(ClientPacket.DropItem, new DropItemPacket { Slot = slot, Amount = amount });

    public static void InventoryChange(short old, short @new)
    {
        Send.Packet(ClientPacket.InventoryChange, new InventoryChangePacket { OldSlot = old, NewSlot = @new });

        // Close drop panel
        Panels.Drop.Visible = false;
    }

    public static void InventoryUse(byte slot)
    {
        Send.Packet(ClientPacket.InventoryUse, new InventoryUsePacket { Slot = slot });

        // Close drop panel
        Panels.Drop.Visible = false;
    }

    public static void EquipmentRemove(byte slot) => Send.Packet(ClientPacket.EquipmentRemove, new EquipmentRemovePacket { Slot = slot });

    public static void HotbarAdd(short hotbarSlot, byte type, short slot) => Send.Packet(ClientPacket.HotbarAdd, new HotbarAddPacket { HotbarSlot = hotbarSlot, Type = type, Slot = slot });

    public static void HotbarChange(short old, short @new) => Send.Packet(ClientPacket.HotbarChange, new HotbarChangePacket { OldSlot = old, NewSlot = @new });

    public static void HotbarUse(byte slot)
    {
        if (TextBox.Focused == null)
        {
            Send.Packet(ClientPacket.HotbarUse, new HotbarUsePacket { Slot = slot });

            // Close drop panel
            Panels.Drop.Visible = false;
        }
    }
}
