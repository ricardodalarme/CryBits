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
        host.IntentFunnel.Submit(
            new MoveIntent(entityId, (Direction)packet.Direction, (Movement)packet.Movement));
    }

    [PacketHandler]
    internal void PlayerAttack(EntityId entityId, PlayerAttackPacket _)
    {
        host.IntentFunnel.Submit(new AttackIntent(entityId, null));
    }

    [PacketHandler]
    internal void AddPoint(EntityId entityId, AddPointPacket packet)
    {
        host.IntentFunnel.Submit(new AddPointIntent(entityId, packet.Attribute));
    }

    [PacketHandler]
    internal void CollectItem(EntityId entityId, CollectItemPacket _)
    {
        host.IntentFunnel.Submit(new CollectItemIntent(entityId));
    }

    [PacketHandler]
    internal void DropItem(EntityId entityId, DropItemPacket packet)
    {
        host.IntentFunnel.Submit(
            new DropItemIntent(entityId, (byte)packet.Slot, packet.Amount));
    }

    [PacketHandler]
    internal void InventoryChange(EntityId entityId, InventoryChangePacket packet)
    {
        host.IntentFunnel.Submit(
            new InventorySwapIntent(entityId, packet.OldSlot, packet.NewSlot));
    }

    [PacketHandler]
    internal void InventoryUse(EntityId entityId, InventoryUsePacket packet)
    {
        host.IntentFunnel.Submit(new InventoryUseIntent(entityId, packet.Slot));
    }

    [PacketHandler]
    internal void EquipmentRemove(EntityId entityId, EquipmentRemovePacket packet)
    {
        host.IntentFunnel.Submit(new EquipmentRemoveIntent(entityId, packet.Slot));
    }

    [PacketHandler]
    internal void HotbarAdd(EntityId entityId, HotbarAddPacket packet)
    {
        host.IntentFunnel.Submit(
            new HotbarAddIntent(entityId, packet.HotbarSlot, (SlotType)packet.Type, packet.Slot));
    }

    [PacketHandler]
    internal void HotbarChange(EntityId entityId, HotbarChangePacket packet)
    {
        host.IntentFunnel.Submit(
            new HotbarSwapIntent(entityId, packet.OldSlot, packet.NewSlot));
    }

    [PacketHandler]
    internal void HotbarUse(EntityId entityId, HotbarUsePacket packet)
    {
        host.IntentFunnel.Submit(new HotbarUseIntent(entityId, packet.Slot));
    }
}
