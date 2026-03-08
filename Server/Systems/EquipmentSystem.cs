using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>
/// Request-driven system that owns equipping and unequipping items for a player.
/// InventorySystem is accessed via its Instance to avoid a circular constructor dependency.
/// </summary>
internal sealed class EquipmentSystem(PlayerSender playerSender, MapSender mapSender)
{
    public static EquipmentSystem Instance { get; } = new(PlayerSender.Instance, MapSender.Instance);

    private readonly PlayerSender _playerSender = playerSender;
    private readonly MapSender _mapSender = mapSender;

    /// <summary>
    /// Equips the item in <paramref name="slot"/>, swapping out any currently worn item of that type.
    /// Removes the item from the inventory slot before placing it in the equipment slot.
    /// </summary>
    public void Equip(Player player, ItemSlot slot)
    {
        var item = slot.Item;
        InventorySystem.Instance.TakeItem(player, slot, 1);

        var currentEquip = player.Equipment[item.EquipType];
        if (currentEquip != null) InventorySystem.Instance.GiveItem(player, currentEquip, 1);

        player.Equipment[item.EquipType] = item;
        for (byte i = 0; i < (byte)Attribute.Count; i++) player.Attribute[i] += item.EquipAttribute[i];

        _playerSender.PlayerInventory(player);
        _playerSender.PlayerEquipments(player);
        _playerSender.PlayerHotbar(player);
    }

    /// <summary>
    /// Unequips the item in <paramref name="equipSlot"/>, returning it to the inventory or
    /// dropping it on the ground when the inventory is full.
    /// Items bound on equip cannot be removed.
    /// </summary>
    public void Unequip(Player player, byte equipSlot)
    {
        if (player.Equipment[equipSlot] == null) return;
        if (player.Equipment[equipSlot].Bind == BindOn.Equip) return;

        if (!InventorySystem.Instance.GiveItem(player, player.Equipment[equipSlot], 1))
        {
            if (player.MapInstance.Item.Count == Config.MaxMapItems) return;

            player.MapInstance.Item.Add(new MapItemInstance(player.Equipment[equipSlot], 1, player.X, player.Y));
            _mapSender.MapItems(player.MapInstance);
            _playerSender.PlayerInventory(player);
        }

        for (byte i = 0; i < (byte)Attribute.Count; i++)
            player.Attribute[i] -= player.Equipment[equipSlot].EquipAttribute[i];
        player.Equipment[equipSlot] = null;

        _playerSender.PlayerEquipments(player);
    }
}
