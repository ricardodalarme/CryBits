using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class PlayerHandler
{
    [PacketHandler(ClientPacket.PlayerDirection)]
    internal static void PlayerDirection(Player player, PlayerDirectionPacket packet)
    {
        MovementSystem.ChangeDirection(player, (Direction)packet.Direction);
    }

    [PacketHandler(ClientPacket.PlayerMove)]
    internal static void PlayerMove(Player player, PlayerMovePacket packet)
    {
        if (player.X != packet.X || player.Y != packet.Y)
            PlayerSender.PlayerPosition(player);
        else
            MovementSystem.Move(player, packet.Movement);
    }

    [PacketHandler(ClientPacket.PlayerAttack)]
    internal static void PlayerAttack(Player player)
    {
        CombatSystem.Attack(player);
    }

    [PacketHandler(ClientPacket.AddPoint)]
    internal static void AddPoint(Player player, AddPointPacket packet)
    {
        LevelingSystem.AddPoint(player, packet.Attribute);
    }

    [PacketHandler(ClientPacket.CollectItem)]
    internal static void CollectItem(Player player)
    {
        InventorySystem.CollectItem(player);
    }

    [PacketHandler(ClientPacket.DropItem)]
    internal static void DropItem(Player player, DropItemPacket packet)
    {
        var slot = packet.Slot;
        var amount = packet.Amount;
        if (slot != -1) InventorySystem.DropItem(player, player.Inventory[slot], amount);
    }

    [PacketHandler(ClientPacket.InventoryChange)]
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

    [PacketHandler(ClientPacket.InventoryUse)]
    internal static void InventoryUse(Player player, InventoryUsePacket packet)
    {
        InventorySystem.UseItem(player, player.Inventory[packet.Slot]);
    }

    [PacketHandler(ClientPacket.EquipmentRemove)]
    internal static void EquipmentRemove(Player player, EquipmentRemovePacket packet)
    {
        EquipmentSystem.Unequip(player, packet.Slot);
    }

    [PacketHandler(ClientPacket.HotbarAdd)]
    internal static void HotbarAdd(Player player, HotbarAddPacket packet)
    {
        HotbarSystem.Add(player, packet.HotbarSlot, (SlotType)packet.Type, packet.Slot);
    }

    [PacketHandler(ClientPacket.HotbarChange)]
    internal static void HotbarChange(Player player, HotbarChangePacket packet)
    {
        HotbarSystem.Change(player, packet.OldSlot, packet.NewSlot);
    }

    [PacketHandler(ClientPacket.HotbarUse)]
    internal static void HotbarUse(Player player, HotbarUsePacket packet)
    {
        HotbarSystem.Use(player, packet.Slot);
    }
}
