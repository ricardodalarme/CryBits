using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>
/// Request-driven system that owns equipping and unequipping items for a player.
/// </summary>
internal static class EquipmentSystem
{
    /// <summary>
    /// Equips the item in <paramref name="slot"/>, swapping out any currently worn item of that type.
    /// Removes the item from the inventory slot before placing it in the equipment slot.
    /// </summary>
    public static void Equip(Player player, ItemSlot slot)
    {
        var item = slot.Item;
        InventorySystem.TakeItem(player, slot, 1);

        var currentEquip = player.Equipment[item.EquipType];
        if (currentEquip != null) InventorySystem.GiveItem(player, currentEquip, 1);

        player.Equipment[item.EquipType] = item;
        for (byte i = 0; i < (byte)Attribute.Count; i++) player.Attribute[i] += item.EquipAttribute[i];

        PlayerSender.PlayerInventory(player);
        PlayerSender.PlayerEquipments(player);
        PlayerSender.PlayerHotbar(player);
    }

    /// <summary>
    /// Unequips the item in <paramref name="equipSlot"/>, returning it to the inventory or
    /// dropping it on the ground when the inventory is full.
    /// Items bound on equip cannot be removed.
    /// </summary>
    public static void Unequip(Player player, byte equipSlot)
    {
        if (player.Equipment[equipSlot] == null) return;
        if (player.Equipment[equipSlot].Bind == BindOn.Equip) return;

        if (!InventorySystem.GiveItem(player, player.Equipment[equipSlot], 1))
        {
            if (player.MapInstance.Item.Count == Config.MaxMapItems) return;

            player.MapInstance.Item.Add(new MapItemInstance(player.Equipment[equipSlot], 1, player.X, player.Y));
            MapSender.MapItems(player.MapInstance);
            PlayerSender.PlayerInventory(player);
        }

        for (byte i = 0; i < (byte)Attribute.Count; i++)
            player.Attribute[i] -= player.Equipment[equipSlot].EquipAttribute[i];
        player.Equipment[equipSlot] = null;

        PlayerSender.PlayerEquipments(player);
    }
}
