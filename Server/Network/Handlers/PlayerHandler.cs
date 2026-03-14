using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;

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
    internal void PlayerMove(Player player, PlayerMovePacket packet)
    {
        movementSystem.ChangeDirection(player, (Direction)packet.Direction);
        movementSystem.Move(player, packet.Movement);
    }

    [PacketHandler]
    internal void PlayerAttack(Player player, PlayerAttackPacket _)
    {
        combatSystem.Attack(player);
    }

    [PacketHandler]
    internal void AddPoint(Player player, AddPointPacket packet)
    {
        levelingSystem.AddPoint(player, packet.Attribute);
    }

    [PacketHandler]
    internal void CollectItem(Player player, CollectItemPacket _)
    {
        inventorySystem.CollectItem(player);
    }

    [PacketHandler]
    internal void DropItem(Player player, DropItemPacket packet)
    {
        var slot = packet.Slot;
        var amount = packet.Amount;
        if (slot != -1) inventorySystem.DropItem(player, player.Inventory[slot], amount);
    }

    [PacketHandler]
    internal void InventoryChange(Player player, InventoryChangePacket packet)
    {
        short slotOld = packet.OldSlot, slotNew = packet.NewSlot;

        // Early exits.
        if (player.Inventory[slotOld].Item == null) return;
        if (slotOld == slotNew) return;
        if (player.Trade != null) return;

        // Swap inventory slots.
        (player.Inventory[slotOld], player.Inventory[slotNew]) = (player.Inventory[slotNew], player.Inventory[slotOld]);
        playerSender.PlayerInventory(player);
        hotbarSystem.SyncInventorySwap(player, slotOld, slotNew);
    }

    [PacketHandler]
    internal void InventoryUse(Player player, InventoryUsePacket packet)
    {
        inventorySystem.UseItem(player, player.Inventory[packet.Slot]);
    }

    [PacketHandler]
    internal void EquipmentRemove(Player player, EquipmentRemovePacket packet)
    {
        equipmentSystem.Unequip(player, packet.Slot);
    }

    [PacketHandler]
    internal void HotbarAdd(Player player, HotbarAddPacket packet)
    {
        hotbarSystem.Add(player, packet.HotbarSlot, (SlotType)packet.Type, packet.Slot);
    }

    [PacketHandler]
    internal void HotbarChange(Player player, HotbarChangePacket packet)
    {
        hotbarSystem.Change(player, packet.OldSlot, packet.NewSlot);
    }

    [PacketHandler]
    internal void HotbarUse(Player player, HotbarUsePacket packet)
    {
        hotbarSystem.Use(player, packet.Slot);
    }
}
