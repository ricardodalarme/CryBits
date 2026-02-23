using System.Drawing;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>
/// Request-driven system that owns all player inventory operations.
/// </summary>
internal static class InventorySystem
{
    /// <summary>Adds <paramref name="amount"/> of <paramref name="item"/> to the player's inventory.</summary>
    public static bool GiveItem(Player player, Item? item, short amount)
    {
        if (item == null) return false;

        var inv       = player.Get<InventoryComponent>();
        var slotItem  = player.FindInventory(item);
        var slotEmpty = player.FindInventory(null);

        if (slotEmpty == null) return false;
        if (amount == 0) amount = 1;

        if (slotItem != null && item.Stackable)
            slotItem.Amount += amount;
        else
        {
            slotEmpty.Item   = item;
            slotEmpty.Amount = item.Stackable ? amount : (byte)1;
        }

        PlayerSender.PlayerInventory(player);
        return true;
    }

    /// <summary>Removes <paramref name="amount"/> of the item in <paramref name="slot"/> from the inventory.</summary>
    public static void TakeItem(Player player, ItemSlot? slot, short amount)
    {
        if (slot == null) return;
        if (amount <= 0) amount = 1;

        var hotbarSlot = player.FindHotbar(SlotType.Item, slot);

        if (amount == slot.Amount)
        {
            slot.Item   = null;
            slot.Amount = 0;

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

    /// <summary>Drops <paramref name="amount"/> of the item in <paramref name="slot"/> onto the map.</summary>
    public static void DropItem(Player player, ItemSlot? slot, short amount)
    {
        if (slot == null || slot.Item == null) return;
        if (slot.Item.Bind == BindOn.Pickup) return;
        if (player.Get<TradeComponent>().PartnerId != null) return;

        var pos = player.Get<PositionComponent>();
        if (ServerContext.Instance.MapItemCount(pos.MapId) >= Config.MaxMapItems) return;

        if (amount > slot.Amount) amount = slot.Amount;

        var itemEntity = ServerContext.Instance.World.Create();
        ServerContext.Instance.World.Add(itemEntity, new MapItemComponent
        {
            Item   = slot.Item,
            Amount = amount,
            X      = pos.X,
            Y      = pos.Y,
            MapId  = pos.MapId
        });
        MapSender.MapItems(player.MapInstance);
        TakeItem(player, slot, amount);
    }

    /// <summary>Uses the item in <paramref name="slot"/>.</summary>
    public static void UseItem(Player player, ItemSlot? slot)
    {
        if (slot == null) return;
        var item = slot.Item;
        if (item == null) return;
        if (player.Get<TradeComponent>().PartnerId != null) return;

        var pd = player.Get<PlayerDataComponent>();

        if (pd.Level < item.ReqLevel)
        {
            ChatSender.Message(player, "You do not have the level required to use this item.", Color.White);
            return;
        }

        if (item.ReqClass != null && pd.Class != item.ReqClass)
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

            var vitals = player.Get<VitalsComponent>();
            for (byte i = 0; i < (byte)Vital.Count; i++)
            {
                if (vitals.Values[i] < player.MaxVital(i) && item.PotionVital[i] != 0) hadEffect = true;

                vitals.Values[i] += item.PotionVital[i];
                if (vitals.Values[i] < 0) vitals.Values[i] = 0;
                if (vitals.Values[i] > player.MaxVital(i)) vitals.Values[i] = player.MaxVital(i);
            }

            if (vitals.Values[(byte)Vital.Hp] == 0) CombatSystem.Died(player);

            if (item.PotionExperience > 0 || hadEffect) TakeItem(player, slot, 1);
        }
    }

    /// <summary>Picks up the item on the player's current map tile.</summary>
    public static void CollectItem(Player player)
    {
        var pos      = player.Get<PositionComponent>();
        var itemId   = player.MapInstance.HasItem(pos.X, pos.Y);
        if (itemId < 0) return;

        var world    = ServerContext.Instance.World;
        var mapItem  = world.Get<MapItemComponent>(itemId);

        if (GiveItem(player, mapItem.Item, mapItem.Amount))
        {
            world.Destroy(itemId);
            MapSender.MapItems(player.MapInstance);
        }
    }
}
