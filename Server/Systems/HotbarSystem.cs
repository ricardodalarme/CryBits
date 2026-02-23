using CryBits.Enums;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Systems;

internal static class HotbarSystem
{
    /// <summary>Assigns an inventory or skill slot to a hotbar position.</summary>
    internal static void Add(Player player, short hotbarSlot, SlotType type, short slot)
    {
        var hotbar = player.Get<HotbarComponent>();
        if (slot != 0 && player.FindHotbar(type, slot) != null) return;

        hotbar.Slots[hotbarSlot].Slot = slot;
        hotbar.Slots[hotbarSlot].Type = type;
        PlayerSender.PlayerHotbar(player);
    }

    /// <summary>Swaps two hotbar positions.</summary>
    internal static void Change(Player player, short slotOld, short slotNew)
    {
        if (slotOld < 0 || slotNew < 0) return;
        if (slotOld == slotNew) return;

        var hotbar = player.Get<HotbarComponent>();
        if (hotbar.Slots[slotOld].Slot == 0) return;

        (hotbar.Slots[slotOld], hotbar.Slots[slotNew]) = (hotbar.Slots[slotNew], hotbar.Slots[slotOld]);
        PlayerSender.PlayerHotbar(player);
    }

    /// <summary>Activates the item or skill bound to a hotbar slot.</summary>
    internal static void Use(Player player, short hotbarSlot)
    {
        var hotbar = player.Get<HotbarComponent>();
        switch (hotbar.Slots[hotbarSlot].Type)
        {
            case SlotType.Item:
                var inv = player.Get<InventoryComponent>();
                InventorySystem.UseItem(player, inv.Slots[hotbar.Slots[hotbarSlot].Slot]);
                break;
        }
    }

    /// <summary>
    /// Keeps hotbar indices in sync after two inventory slots are swapped.
    /// Must be called after the inventory swap has already been applied.
    /// </summary>
    internal static void SyncInventorySwap(Player player, short slotOld, short slotNew)
    {
        var inv        = player.Get<InventoryComponent>();
        var hotbarSlot = player.FindHotbar(SlotType.Item, inv.Slots[slotOld]);
        if (hotbarSlot == null) return;

        hotbarSlot.Slot = slotNew;
        PlayerSender.PlayerHotbar(player);
    }
}
