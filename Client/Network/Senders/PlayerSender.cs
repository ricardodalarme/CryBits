using CryBits.Client.Entities;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.UI.Game.Views;
using CryBits.Packets.Client;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.Network.Senders;

internal class PlayerSender(PacketSender packetSender)
{
    public static PlayerSender Instance { get; } = new(PacketSender.Instance);

    public void PlayerDirection() => packetSender.Packet(new PlayerDirectionPacket { Direction = (byte)Player.Me.Direction });

    public void PlayerMove() => packetSender.Packet(new PlayerMovePacket { X = Player.Me.X, Y = Player.Me.Y, Movement = (byte)Player.Me.Movement });

    public void PlayerAttack() => packetSender.Packet(new PlayerAttackPacket());

    public void AddPoint(Attribute attribute) => packetSender.Packet(new AddPointPacket { Attribute = (byte)attribute });

    public void CollectItem() => packetSender.Packet(new CollectItemPacket());

    public void DropItem(short slot, short amount) => packetSender.Packet(new DropItemPacket { Slot = slot, Amount = amount });

    public void InventoryChange(short old, short @new)
    {
        packetSender.Packet(new InventoryChangePacket { OldSlot = old, NewSlot = @new });

        // Close drop panel
        DropItemView.Panel.Visible = false;
    }

    public void InventoryUse(byte slot)
    {
        packetSender.Packet(new InventoryUsePacket { Slot = slot });

        // Close drop panel
        DropItemView.Panel.Visible = false;
    }

    public void EquipmentRemove(byte slot) => packetSender.Packet(new EquipmentRemovePacket { Slot = slot });

    public void HotbarAdd(short hotbarSlot, byte type, short slot) => packetSender.Packet(new HotbarAddPacket { HotbarSlot = hotbarSlot, Type = type, Slot = slot });

    public void HotbarChange(short old, short @new) => packetSender.Packet(new HotbarChangePacket { OldSlot = old, NewSlot = @new });

    public void HotbarUse(byte slot)
    {
        if (TextBox.Focused == null)
        {
            packetSender.Packet(new HotbarUsePacket { Slot = slot });

            // Close drop panel
            DropItemView.Panel.Visible = false;
        }
    }
}
