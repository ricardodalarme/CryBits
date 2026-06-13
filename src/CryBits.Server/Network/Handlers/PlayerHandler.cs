using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Server.Network.Handlers;

internal sealed class PlayerHandler()
{
    public static PlayerHandler Instance { get; } = new();

    [PacketHandler]
    internal void PlayerMove(EntityId entityId, PlayerMovePacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(
            new MoveIntent(entityId, (Direction)packet.Direction, packet.Movement));
    }

    [PacketHandler]
    internal void PlayerAttack(EntityId entityId, PlayerAttackPacket _)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new AttackIntent(entityId));
    }

    [PacketHandler]
    internal void AddPoint(EntityId entityId, AddPointPacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new AddPointIntent(entityId, packet.Attribute));
    }

    [PacketHandler]
    internal void CollectItem(EntityId entityId, CollectItemPacket _)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new CollectItemIntent(entityId));
    }

    [PacketHandler]
    internal void DropItem(EntityId entityId, DropItemPacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(
            new DropItemIntent(entityId, packet.Slot, packet.Amount));
    }

    [PacketHandler]
    internal void InventoryChange(EntityId entityId, InventoryChangePacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(
            new InventorySwapIntent(entityId, packet.OldSlot, packet.NewSlot));
    }

    [PacketHandler]
    internal void InventoryUse(EntityId entityId, InventoryUsePacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new InventoryUseIntent(entityId, packet.Slot));
    }

    [PacketHandler]
    internal void EquipmentRemove(EntityId entityId, EquipmentRemovePacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new EquipmentRemoveIntent(entityId, packet.Slot));
    }

    [PacketHandler]
    internal void HotbarAdd(EntityId entityId, HotbarAddPacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(
            new HotbarAddIntent(entityId, packet.HotbarSlot, (SlotType)packet.Type, packet.Slot));
    }

    [PacketHandler]
    internal void HotbarChange(EntityId entityId, HotbarChangePacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(
            new HotbarSwapIntent(entityId, packet.OldSlot, packet.NewSlot));
    }

    [PacketHandler]
    internal void HotbarUse(EntityId entityId, HotbarUsePacket packet)
    {
        WorldHost.Current.CurrentTick?.Intents.Enqueue(new HotbarUseIntent(entityId, packet.Slot));
    }
}
