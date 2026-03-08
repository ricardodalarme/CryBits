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

    private readonly MovementSystem _movementSystem = movementSystem;
    private readonly CombatSystem _combatSystem = combatSystem;
    private readonly LevelingSystem _levelingSystem = levelingSystem;
    private readonly InventorySystem _inventorySystem = inventorySystem;
    private readonly EquipmentSystem _equipmentSystem = equipmentSystem;
    private readonly HotbarSystem _hotbarSystem = hotbarSystem;
    private readonly PlayerSender _playerSender = playerSender;

    [PacketHandler]
    internal void PlayerMove(Player player, PlayerMovePacket packet)
    {
        _movementSystem.ChangeDirection(player, (Direction)packet.Direction);
        _movementSystem.Move(player, packet.Movement);
    }

    [PacketHandler]
    internal void PlayerAttack(Player player, PlayerAttackPacket _)
    {
        _combatSystem.Attack(player);
    }

    [PacketHandler]
    internal void AddPoint(Player player, AddPointPacket packet)
    {
        _levelingSystem.AddPoint(player, packet.Attribute);
    }

    [PacketHandler]
    internal void CollectItem(Player player, CollectItemPacket _)
    {
        _inventorySystem.CollectItem(player);
    }

    [PacketHandler]
    internal void DropItem(Player player, DropItemPacket packet)
    {
        var slot = packet.Slot;
        var amount = packet.Amount;
        if (slot != -1) _inventorySystem.DropItem(player, player.Inventory[slot], amount);
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
        _playerSender.PlayerInventory(player);
        _hotbarSystem.SyncInventorySwap(player, slotOld, slotNew);
    }

    [PacketHandler]
    internal void InventoryUse(Player player, InventoryUsePacket packet)
    {
        _inventorySystem.UseItem(player, player.Inventory[packet.Slot]);
    }

    [PacketHandler]
    internal void EquipmentRemove(Player player, EquipmentRemovePacket packet)
    {
        _equipmentSystem.Unequip(player, packet.Slot);
    }

    [PacketHandler]
    internal void HotbarAdd(Player player, HotbarAddPacket packet)
    {
        _hotbarSystem.Add(player, packet.HotbarSlot, (SlotType)packet.Type, packet.Slot);
    }

    [PacketHandler]
    internal void HotbarChange(Player player, HotbarChangePacket packet)
    {
        _hotbarSystem.Change(player, packet.OldSlot, packet.NewSlot);
    }

    [PacketHandler]
    internal void HotbarUse(Player player, HotbarUsePacket packet)
    {
        _hotbarSystem.Use(player, packet.Slot);
    }
}
