using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Packets.Client;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.Network.Senders;

internal static class PlayerSender
{
    public static void PlayerDirection() => PacketSender.Packet(new PlayerDirectionPacket { Direction = (byte)Player.Me.Direction });

    public static void PlayerMove() => PacketSender.Packet(new PlayerMovePacket { X = Player.Me.X, Y = Player.Me.Y, Movement = (byte)Player.Me.Movement });

    public static void PlayerAttack() => PacketSender.Packet(new PlayerAttackPacket());

    public static void AddPoint(Attribute attribute) => PacketSender.Packet(new AddPointPacket { Attribute = (byte)attribute });

    public static void CollectItem() => PacketSender.Packet(new CollectItemPacket());

    public static void DropItem(short slot, short amount) => PacketSender.Packet(new DropItemPacket { Slot = slot, Amount = amount });

    public static void InventoryChange(short old, short @new)
    {
        PacketSender.Packet(new InventoryChangePacket { OldSlot = old, NewSlot = @new });

        // Close drop panel
        Panels.Drop.Visible = false;
    }

    public static void InventoryUse(byte slot)
    {
        PacketSender.Packet(new InventoryUsePacket { Slot = slot });

        // Close drop panel
        Panels.Drop.Visible = false;
    }

    public static void EquipmentRemove(byte slot) => PacketSender.Packet(new EquipmentRemovePacket { Slot = slot });

    public static void HotbarAdd(short hotbarSlot, byte type, short slot) => PacketSender.Packet(new HotbarAddPacket { HotbarSlot = hotbarSlot, Type = type, Slot = slot });

    public static void HotbarChange(short old, short @new) => PacketSender.Packet(new HotbarChangePacket { OldSlot = old, NewSlot = @new });

    public static void HotbarUse(byte slot)
    {
        if (TextBox.Focused == null)
        {
            PacketSender.Packet(new HotbarUsePacket { Slot = slot });

            // Close drop panel
            Panels.Drop.Visible = false;
        }
    }
}
