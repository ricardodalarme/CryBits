using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.Network.Senders;
using CryBits.Simulation.Components;
using CryBits.Server.Systems.Combat;
using CryBits.Server.Systems.Inventory;
using CryBits.Server.Systems.Movement;
using CryBits.Server.Systems.Progression;
using CryBits.Server.World;
using System;
using CryBits.Simulation.State;

namespace CryBits.Server.Network.Handlers;

internal sealed class PlayerHandler(
    MovementSystem movementSystem,
    CombatSystem combatSystem,
    LevelingSystem levelingSystem,
    InventorySystem inventorySystem,
    EquipmentSystem equipmentSystem,
    HotbarSystem hotbarSystem,
    PlayerSender playerSender)
{
    public static PlayerHandler Instance { get; } = new(
        MovementSystem.Instance,
        CombatSystem.Instance,
        LevelingSystem.Instance,
        InventorySystem.Instance,
        EquipmentSystem.Instance,
        HotbarSystem.Instance,
        PlayerSender.Instance);

    [PacketHandler]
    internal void PlayerMove(EntityId entityId, PlayerMovePacket packet)
    {
        movementSystem.ChangeDirection(entityId, (Direction)packet.Direction);
        movementSystem.Move(entityId, packet.Movement);
    }

    [PacketHandler]
    internal void PlayerAttack(EntityId entityId, PlayerAttackPacket _)
    {
        combatSystem.Attack(entityId);
    }

    [PacketHandler]
    internal void AddPoint(EntityId entityId, AddPointPacket packet)
    {
        levelingSystem.AddPoint(entityId, packet.Attribute);
    }

    [PacketHandler]
    internal void CollectItem(EntityId entityId, CollectItemPacket _)
    {
        inventorySystem.CollectItem(entityId);
    }

    [PacketHandler]
    internal void DropItem(EntityId entityId, DropItemPacket packet)
    {
        var slot = packet.Slot;
        var amount = packet.Amount;
        if (slot != -1)
        {
            var world = GameWorld.Current;
            var state = world.Entities.Get(entityId)!;
            var inventory = state.Get<InventoryState>()!;
            inventorySystem.DropItem(entityId, inventory.Slots[slot], amount);
        }
    }

    [PacketHandler]
    internal void InventoryChange(EntityId entityId, InventoryChangePacket packet)
    {
        short slotOld = packet.OldSlot, slotNew = packet.NewSlot;

        var world = GameWorld.Current;
        var state = world.Entities.Get(entityId)!;
        var inventory = state.Get<InventoryState>()!;
        var trade = state.Get<TradeState>();

        // Early exits.
        if (inventory.Slots[slotOld].ItemId == Guid.Empty) return;
        if (slotOld == slotNew) return;
        if (trade?.Partner != null) return;

        // Swap inventory slots.
        (inventory.Slots[slotOld], inventory.Slots[slotNew]) = (inventory.Slots[slotNew], inventory.Slots[slotOld]);
        playerSender.PlayerInventory(entityId);
        hotbarSystem.SyncInventorySwap(entityId, slotOld, slotNew);
    }

    [PacketHandler]
    internal void InventoryUse(EntityId entityId, InventoryUsePacket packet)
    {
        var world = GameWorld.Current;
        var state = world.Entities.Get(entityId)!;
        var inventory = state.Get<InventoryState>()!;
        inventorySystem.UseItem(entityId, packet.Slot, inventory.Slots[packet.Slot]);
    }

    [PacketHandler]
    internal void EquipmentRemove(EntityId entityId, EquipmentRemovePacket packet)
    {
        equipmentSystem.Unequip(entityId, packet.Slot);
    }

    [PacketHandler]
    internal void HotbarAdd(EntityId entityId, HotbarAddPacket packet)
    {
        hotbarSystem.Add(entityId, packet.HotbarSlot, (SlotType)packet.Type, packet.Slot);
    }

    [PacketHandler]
    internal void HotbarChange(EntityId entityId, HotbarChangePacket packet)
    {
        hotbarSystem.Change(entityId, packet.OldSlot, packet.NewSlot);
    }

    [PacketHandler]
    internal void HotbarUse(EntityId entityId, HotbarUsePacket packet)
    {
        hotbarSystem.Use(entityId, packet.Slot);
    }
}
