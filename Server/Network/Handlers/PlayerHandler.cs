using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;

namespace CryBits.Server.Network.Handlers;

internal static class PlayerHandler
{
    [PacketHandler]
    internal static void PlayerDirection(Player player, PlayerDirectionPacket packet)
    {
        MovementSystem.ChangeDirection(player, (Direction)packet.Direction);
    }

    [PacketHandler]
    internal static void PlayerMove(Player player, PlayerMovePacket packet)
    {
        var pos = ServerContext.Instance.World.Get<PositionComponent>(player.EntityId);
        if (pos.X != packet.X || pos.Y != packet.Y)
            PlayerSender.PlayerPosition(player);
        else
            MovementSystem.Move(player, packet.Movement);
    }

    [PacketHandler]
    internal static void PlayerAttack(Player player, PlayerAttackPacket _)
    {
        CombatSystem.Attack(player);
    }

    [PacketHandler]
    internal static void AddPoint(Player player, AddPointPacket packet)
    {
        LevelingSystem.AddPoint(player, packet.Attribute);
    }

    [PacketHandler]
    internal static void CollectItem(Player player, CollectItemPacket _)
    {
        InventorySystem.CollectItem(player);
    }

    [PacketHandler]
    internal static void DropItem(Player player, DropItemPacket packet)
    {
        var slot = packet.Slot;
        var amount = packet.Amount;
        if (slot < 0) return;

        var inv = ServerContext.Instance.World.Get<InventoryComponent>(player.EntityId);
        InventorySystem.DropItem(player, inv.Slots[slot], amount);
    }

    [PacketHandler]
    internal static void InventoryChange(Player player, InventoryChangePacket packet)
    {
        short slotOld = packet.OldSlot, slotNew = packet.NewSlot;

        var world = ServerContext.Instance.World;
        var inv = world.Get<InventoryComponent>(player.EntityId);
        var trade = world.Get<TradeComponent>(player.EntityId);

        if (inv.Slots[slotOld].Item == null) return;
        if (slotOld == slotNew) return;
        if (trade.PartnerId != null) return;

        (inv.Slots[slotOld], inv.Slots[slotNew]) = (inv.Slots[slotNew], inv.Slots[slotOld]);
        PlayerSender.PlayerInventory(player);
        HotbarSystem.SyncInventorySwap(player, slotOld, slotNew);
    }

    [PacketHandler]
    internal static void InventoryUse(Player player, InventoryUsePacket packet)
    {
        var inv = ServerContext.Instance.World.Get<InventoryComponent>(player.EntityId);
        InventorySystem.UseItem(player, inv.Slots[packet.Slot]);
    }

    [PacketHandler]
    internal static void EquipmentRemove(Player player, EquipmentRemovePacket packet)
    {
        EquipmentSystem.Unequip(player, packet.Slot);
    }

    [PacketHandler]
    internal static void HotbarAdd(Player player, HotbarAddPacket packet)
    {
        HotbarSystem.Add(player, packet.HotbarSlot, (SlotType)packet.Type, packet.Slot);
    }

    [PacketHandler]
    internal static void HotbarChange(Player player, HotbarChangePacket packet)
    {
        HotbarSystem.Change(player, packet.OldSlot, packet.NewSlot);
    }

    [PacketHandler]
    internal static void HotbarUse(Player player, HotbarUsePacket packet)
    {
        HotbarSystem.Use(player, packet.Slot);
    }
}
