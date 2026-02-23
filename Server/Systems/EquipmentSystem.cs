using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
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
    /// </summary>
    public static void Equip(Player player, ItemSlot slot)
    {
        var item  = slot.Item!;
        var world = ServerContext.Instance.World;
        var equip = player.Get<EquipmentComponent>();
        var attr  = player.Get<AttributeComponent>();

        InventorySystem.TakeItem(player, slot, 1);

        var currentEquip = equip.Slots[item.EquipType];
        if (currentEquip != null) InventorySystem.GiveItem(player, currentEquip, 1);

        equip.Slots[item.EquipType] = item;
        for (byte i = 0; i < (byte)Attribute.Count; i++) attr.Values[i] += item.EquipAttribute[i];

        PlayerSender.PlayerInventory(player);
        PlayerSender.PlayerEquipments(player);
        PlayerSender.PlayerHotbar(player);
    }

    /// <summary>
    /// Unequips the item in <paramref name="equipSlot"/>, returning it to inventory or dropping it.
    /// Items bound on equip cannot be removed.
    /// </summary>
    public static void Unequip(Player player, byte equipSlot)
    {
        var equip = player.Get<EquipmentComponent>();
        var item  = equip.Slots[equipSlot];
        if (item == null) return;
        if (item.Bind == BindOn.Equip) return;

        if (!InventorySystem.GiveItem(player, item, 1))
        {
            var pos = player.Get<PositionComponent>();
            if (ServerContext.Instance.MapItemCount(pos.MapId) >= Config.MaxMapItems) return;

            var itemEntity = ServerContext.Instance.World.Create();
            ServerContext.Instance.World.Add(itemEntity, new MapItemComponent
            {
                Item   = item,
                Amount = 1,
                X      = pos.X,
                Y      = pos.Y,
                MapId  = pos.MapId
            });
            MapSender.MapItems(player.MapInstance);
            PlayerSender.PlayerInventory(player);
        }

        var attr = player.Get<AttributeComponent>();
        for (byte i = 0; i < (byte)Attribute.Count; i++)
            attr.Values[i] -= item.EquipAttribute[i];
        equip.Slots[equipSlot] = null;

        PlayerSender.PlayerEquipments(player);
    }
}
