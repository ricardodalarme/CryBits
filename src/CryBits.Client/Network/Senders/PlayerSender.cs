using CryBits.Client.Framework.Network;
using CryBits.Definitions.Common;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Client.Network.Senders;

internal class PlayerSender(PacketSender packetSender)
{
    public static PlayerSender Instance { get; } = new(PacketSender.Instance);

    public void PlayerMove(Direction direction, Movement movement) =>
        packetSender.Packet(new PlayerMovePacket { Direction = (byte)direction, Movement = (byte)movement },
            DeliveryChannel.Sequenced);

    public void PlayerAttack() =>
        packetSender.Packet(new PlayerAttackPacket(), DeliveryChannel.ReliableUnordered);

    public void AddPoint(Attribute attribute) =>
        packetSender.Packet(new AddPointPacket { Attribute = (byte)attribute });

    public void CollectItem() =>
        packetSender.Packet(new CollectItemPacket(), DeliveryChannel.ReliableUnordered);

    public void DropItem(short slot, short amount) =>
        packetSender.Packet(new DropItemPacket { Slot = slot, Amount = amount }, DeliveryChannel.ReliableUnordered);

    public void InventoryChange(short old, short @new) =>
        packetSender.Packet(new InventoryChangePacket { OldSlot = old, NewSlot = @new });

    public void InventoryUse(byte slot) =>
        packetSender.Packet(new InventoryUsePacket { Slot = slot }, DeliveryChannel.ReliableUnordered);

    public void EquipmentRemove(byte slot) =>
        packetSender.Packet(new EquipmentRemovePacket { Slot = slot }, DeliveryChannel.ReliableUnordered);

    public void HotbarAdd(short hotbarSlot, byte type, short slot) =>
        packetSender.Packet(new HotbarAddPacket { HotbarSlot = hotbarSlot, Type = type, Slot = slot });

    public void HotbarChange(short old, short @new) =>
        packetSender.Packet(new HotbarChangePacket { OldSlot = old, NewSlot = @new });

    public void HotbarUse(byte slot) =>
        packetSender.Packet(new HotbarUsePacket { Slot = slot }, DeliveryChannel.ReliableUnordered);
}
