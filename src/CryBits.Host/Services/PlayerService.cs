using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Services;

internal sealed class PlayerService(WorldHost host)
{
    [PacketHandler]
    internal void PlayerMove(EntityId entityId, PlayerMovePacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(
            new MoveIntent(entityId, (Direction)packet.Direction, (Movement)packet.Movement));
    }

    [PacketHandler]
    internal void PlayerAttack(EntityId entityId, PlayerAttackPacket _)
    {
        host.CurrentTick?.Intents.Enqueue(new AttackIntent(entityId, null));
    }

    [PacketHandler]
    internal void AddPoint(EntityId entityId, AddPointPacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(new AddPointIntent(entityId, packet.Attribute));
    }

    [PacketHandler]
    internal void CollectItem(EntityId entityId, CollectItemPacket _)
    {
        host.CurrentTick?.Intents.Enqueue(new CollectItemIntent(entityId));
    }

    [PacketHandler]
    internal void DropItem(EntityId entityId, DropItemPacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(
            new DropItemIntent(entityId, (byte)packet.Slot, packet.Amount));
    }

    [PacketHandler]
    internal void InventoryChange(EntityId entityId, InventoryChangePacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(
            new InventorySwapIntent(entityId, packet.OldSlot, packet.NewSlot));
    }

    [PacketHandler]
    internal void InventoryUse(EntityId entityId, InventoryUsePacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(new InventoryUseIntent(entityId, packet.Slot));
    }

    [PacketHandler]
    internal void EquipmentRemove(EntityId entityId, EquipmentRemovePacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(new EquipmentRemoveIntent(entityId, packet.Slot));
    }

    [PacketHandler]
    internal void HotbarAdd(EntityId entityId, HotbarAddPacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(
            new HotbarAddIntent(entityId, packet.HotbarSlot, (SlotType)packet.Type, packet.Slot));
    }

    [PacketHandler]
    internal void HotbarChange(EntityId entityId, HotbarChangePacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(
            new HotbarSwapIntent(entityId, packet.OldSlot, packet.NewSlot));
    }

    [PacketHandler]
    internal void HotbarUse(EntityId entityId, HotbarUsePacket packet)
    {
        host.CurrentTick?.Intents.Enqueue(new HotbarUseIntent(entityId, packet.Slot));
    }
}
