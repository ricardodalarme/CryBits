using System.Drawing;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Network.Senders;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>
/// Request-driven system that owns all player inventory operations:
/// giving, taking, dropping, using, equipping, unequipping, and collecting ground items.
/// </summary>
internal static class InventorySystem
{
    /// <summary>
    /// Adds <paramref name="amount"/> of <paramref name="item"/> to <paramref name="player"/>'s inventory.
    /// Stacks onto an existing slot when the item is stackable; otherwise fills an empty slot.
    /// Returns false when the inventory is full or the item is null.
    /// </summary>
    public static bool GiveItem(Player player, Item item, short amount)
    {
        if (item == null) return false;

        var slotItem = player.FindInventory(item);
        var slotEmpty = player.FindInventory(null);

        if (slotEmpty == null) return false;
        if (amount == 0) amount = 1;

        if (slotItem != null && item.Stackable)
            slotItem.Amount += amount;
        else
        {
            slotEmpty.Item = item;
            slotEmpty.Amount = item.Stackable ? amount : (byte)1;
        }

        PlayerSender.PlayerInventory(player);
        return true;
    }

    /// <summary>
    /// Removes <paramref name="amount"/> of the item in <paramref name="slot"/> from
    /// <paramref name="player"/>'s inventory. Also clears the matching hotbar entry when
    /// the slot is fully emptied.
    /// </summary>
    public static void TakeItem(Player player, ItemSlot slot, short amount)
    {
        if (slot == null) return;
        if (amount <= 0) amount = 1;

        if (amount == slot.Amount)
        {
            slot.Item = null;
            slot.Amount = 0;

            var hotbarSlot = player.FindHotbar(SlotType.Item, slot);
            if (hotbarSlot != null)
            {
                hotbarSlot.Type = SlotType.None;
                hotbarSlot.Slot = 0;
                PlayerSender.PlayerHotbar(player);
            }
        }
        else
            slot.Amount -= amount;

        PlayerSender.PlayerInventory(player);
    }

    /// <summary>
    /// Drops <paramref name="amount"/> of the item in <paramref name="slot"/> onto the map tile
    /// the player is standing on, then removes it from the inventory.
    /// </summary>
    public static void DropItem(Player player, ItemSlot slot, short amount)
    {
        if (player.Map.Item.Count == MaxMapItems) return;
        if (slot.Item == null) return;
        if (slot.Item.Bind == BindOn.Pickup) return;
        if (player.Trade != null) return;

        if (amount > slot.Amount) amount = slot.Amount;

        player.Map.Item.Add(new TempMapItems(slot.Item, amount, player.X, player.Y));
        MapSender.MapItems(player.Map);
        TakeItem(player, slot, amount);
    }

    /// <summary>
    /// Uses the item in <paramref name="slot"/>. Equipment items are equipped; potions apply
    /// their vital and experience effects. No-ops if the player has an active trade.
    /// </summary>
    public static void UseItem(Player player, ItemSlot slot)
    {
        var item = slot.Item;
        if (item == null) return;
        if (player.Trade != null) return;

        if (player.Level < item.ReqLevel)
        {
            ChatSender.Message(player, "You do not have the level required to use this item.", Color.White);
            return;
        }

        if (item.ReqClass != null && player.Class != item.ReqClass)
        {
            ChatSender.Message(player, "You can not use this item.", Color.White);
            return;
        }

        if (item.Type == ItemType.Equipment)
        {
            EquipmentSystem.Equip(player, slot);
        }
        else if (item.Type == ItemType.Potion)
        {
            var hadEffect = false;
            LevelingSystem.GiveExperience(player, item.PotionExperience);

            for (byte i = 0; i < (byte)Vital.Count; i++)
            {
                if (player.Vital[i] < player.MaxVital(i) && item.PotionVital[i] != 0) hadEffect = true;

                player.Vital[i] += item.PotionVital[i];
                if (player.Vital[i] < 0) player.Vital[i] = 0;
                if (player.Vital[i] > player.MaxVital(i)) player.Vital[i] = player.MaxVital(i);
            }

            // A lethal potion triggers the death flow (will delegate to CombatSystem in the next refactor)
            if (player.Vital[(byte)Vital.Hp] == 0) player.Died();

            if (item.PotionExperience > 0 || hadEffect) TakeItem(player, slot, 1);
        }
    }

    /// <summary>
    /// Picks up the item on the player's current map tile and adds it to the inventory.
    /// Removes the item from the map and notifies all players when successful.
    /// </summary>
    public static void CollectItem(Player player)
    {
        var mapItem = player.Map.HasItem(player.X, player.Y);
        if (mapItem == null) return;

        if (GiveItem(player, mapItem.Item, mapItem.Amount))
        {
            player.Map.Item.Remove(mapItem);
            MapSender.MapItems(player.Map);
        }
    }
}
