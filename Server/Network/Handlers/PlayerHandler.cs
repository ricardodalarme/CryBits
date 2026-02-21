using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class PlayerHandler
{
    internal static void PlayerDirection(Player player, PlayerDirectionPacket packet)
    {
        MovementSystem.ChangeDirection(player, (Direction)packet.Direction);
    }

    internal static void PlayerMove(Player player, PlayerMovePacket packet)
    {
        if (player.X != packet.X || player.Y != packet.Y)
            PlayerSender.PlayerPosition(player);
        else
            MovementSystem.Move(player, packet.Movement);
    }

    internal static void PlayerAttack(Player player)
    {
        CombatSystem.Attack(player);
    }

    internal static void AddPoint(Player player, AddPointPacket packet)
    {
        LevelingSystem.AddPoint(player, packet.Attribute);
    }

    internal static void CollectItem(Player player)
    {
        InventorySystem.CollectItem(player);
    }

    internal static void DropItem(Player player, DropItemPacket packet)
    {
        var slot = packet.Slot;
        var amount = packet.Amount;
        if (slot != -1) InventorySystem.DropItem(player, player.Inventory[slot], amount);
    }

    internal static void InventoryChange(Player player, InventoryChangePacket packet)
    {
        short slotOld = packet.OldSlot, slotNew = packet.NewSlot;

        // Early exits.
        if (player.Inventory[slotOld].Item == null) return;
        if (slotOld == slotNew) return;
        if (player.Trade != null) return;

        // Swap inventory slots.
        (player.Inventory[slotOld], player.Inventory[slotNew]) = (player.Inventory[slotNew], player.Inventory[slotOld]);
        PlayerSender.PlayerInventory(player);
        HotbarSystem.SyncInventorySwap(player, slotOld, slotNew);
    }

    internal static void InventoryUse(Player player, InventoryUsePacket packet)
    {
        InventorySystem.UseItem(player, player.Inventory[packet.Slot]);
    }

    internal static void EquipmentRemove(Player player, EquipmentRemovePacket packet)
    {
        EquipmentSystem.Unequip(player, packet.Slot);
    }

    internal static void HotbarAdd(Player player, HotbarAddPacket packet)
    {
        HotbarSystem.Add(player, packet.HotbarSlot, (SlotType)packet.Type, packet.Slot);
    }

    internal static void HotbarChange(Player player, HotbarChangePacket packet)
    {
        HotbarSystem.Change(player, packet.OldSlot, packet.NewSlot);
    }

    internal static void HotbarUse(Player player, HotbarUsePacket packet)
    {
        HotbarSystem.Use(player, packet.Slot);
    }
}
