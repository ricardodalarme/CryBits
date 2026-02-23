using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Enums;
using CryBits.Packets.Client;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.Network.Senders;

internal static class PlayerSender
{
    /// <summary>Notify the server that the local player changed facing direction.</summary>
    public static void PlayerDirection(Direction direction) =>
        Send.Packet(new PlayerDirectionPacket { Direction = (byte)direction });

    /// <summary>Notify the server that the local player started moving to a new tile.</summary>
    public static void PlayerMove(byte x, byte y, Movement movement) =>
        Send.Packet(new PlayerMovePacket { X = x, Y = y, Movement = (byte)movement });

    public static void PlayerAttack() => Send.Packet(new PlayerAttackPacket());

    public static void AddPoint(Attribute attribute) =>
        Send.Packet(new AddPointPacket { Attribute = (byte)attribute });

    public static void CollectItem() => Send.Packet(new CollectItemPacket());

    public static void DropItem(short slot, short amount) =>
        Send.Packet(new DropItemPacket { Slot = slot, Amount = amount });

    public static void InventoryChange(short old, short @new)
    {
        Send.Packet(new InventoryChangePacket { OldSlot = old, NewSlot = @new });
        Panels.Drop.Visible = false;
    }

    public static void InventoryUse(byte slot)
    {
        Send.Packet(new InventoryUsePacket { Slot = slot });
        Panels.Drop.Visible = false;
    }

    public static void EquipmentRemove(byte slot) =>
        Send.Packet(new EquipmentRemovePacket { Slot = slot });

    public static void HotbarAdd(short hotbarSlot, byte type, short slot) =>
        Send.Packet(new HotbarAddPacket { HotbarSlot = hotbarSlot, Type = type, Slot = slot });

    public static void HotbarChange(short old, short @new) =>
        Send.Packet(new HotbarChangePacket { OldSlot = old, NewSlot = @new });

    public static void HotbarUse(byte slot)
    {
        if (TextBox.Focused == null)
        {
            Send.Packet(new HotbarUsePacket { Slot = slot });
            Panels.Drop.Visible = false;
        }
    }
}
