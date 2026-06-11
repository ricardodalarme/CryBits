using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Systems;

internal sealed class HotbarSystem(PlayerSender playerSender, InventorySystem inventorySystem)
{
    public static HotbarSystem Instance { get; } = new(PlayerSender.Instance, InventorySystem.Instance);

    /// <summary>Assigns an inventory or skill slot to a hotbar position.</summary>
    internal void Add(Player player, short hotbarSlot, SlotType type, short slot)
    {
        if (slot != 0 && player.FindHotbar(type, slot) != null) return;

        player.Hotbar[hotbarSlot].Slot = slot;
        player.Hotbar[hotbarSlot].Type = type;
        playerSender.PlayerHotbar(player);
    }

    /// <summary>Swaps two hotbar positions.</summary>
    internal void Change(Player player, short slotOld, short slotNew)
    {
        if (slotOld < 0 || slotNew < 0) return;
        if (slotOld == slotNew) return;
        if (player.Hotbar[slotOld].Slot == 0) return;

        (player.Hotbar[slotOld], player.Hotbar[slotNew]) = (player.Hotbar[slotNew], player.Hotbar[slotOld]);
        playerSender.PlayerHotbar(player);
    }

    /// <summary>Activates the item or skill bound to a hotbar slot.</summary>
    internal void Use(Player player, short hotbarSlot)
    {
        switch (player.Hotbar[hotbarSlot].Type)
        {
            case SlotType.Item:
                inventorySystem.UseItem(player, player.Inventory[player.Hotbar[hotbarSlot].Slot]);
                break;
        }
    }

    /// <summary>
    /// Keeps hotbar indices in sync after two inventory slots are swapped.
    /// Must be called after the inventory swap has already been applied.
    /// </summary>
    internal void SyncInventorySwap(Player player, short slotOld, short slotNew)
    {
        var hotbarSlot = player.FindHotbar(SlotType.Item, player.Inventory[slotOld]);
        if (hotbarSlot == null) return;

        hotbarSlot.Slot = slotNew;
        playerSender.PlayerHotbar(player);
    }
}
