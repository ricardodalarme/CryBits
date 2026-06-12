using CryBits.Definitions.Catalog;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Simulation.Core;
using CryBits.Server.Simulation.Events;
using CryBits.Server.Systems.Progression;
using CryBits.Server.World;
using System;
using System.Drawing;
using static CryBits.Globals;

namespace CryBits.Server.Systems.Inventory;

internal sealed class InventorySystem(
    PlayerSender playerSender,
    MapSender mapSender,
    LevelingSystem levelingSystem,
    ChatSender chatSender,
    DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static InventorySystem Instance { get; } = new(
        PlayerSender.Instance,
        MapSender.Instance,
        LevelingSystem.Instance,
        ChatSender.Instance,
        DefinitionCatalog.Instance);

    public bool GiveItem(Player player, Item item, short amount)
    {
        if (item == null) return false;

        var slotItem = player.FindInventory(item.Id);
        var slotEmpty = player.FindInventory(Guid.Empty);

        if (slotEmpty == null) return false;
        if (amount == 0) amount = 1;

        if (slotItem != null && item.Stackable)
            slotItem.Amount += amount;
        else
        {
            slotEmpty.ItemId = item.Id;
            slotEmpty.Amount = item.Stackable ? amount : (byte)1;
        }

        playerSender.PlayerInventory(player);
        return true;
    }

    public void TakeItem(Player player, ItemSlot slot, short amount)
    {
        if (slot == null) return;
        if (amount <= 0) amount = 1;

        if (amount == slot.Amount)
        {
            slot.ItemId = Guid.Empty;
            slot.Amount = 0;

            var hotbarSlot = player.FindHotbar(SlotType.Item, slot);
            if (hotbarSlot != null)
            {
                hotbarSlot.Type = SlotType.None;
                hotbarSlot.Slot = 0;
                playerSender.PlayerHotbar(player);
            }
        }
        else
            slot.Amount -= amount;

        playerSender.PlayerInventory(player);
    }

    public void DropItem(Player player, ItemSlot slot, short amount)
    {
        if (player.MapInstance.Item.Count == Config.MaxMapItems) return;
        if (slot.ItemId == Guid.Empty) return;
        var item = _catalog.Items.Get(slot.ItemId);
        if (item == null || item.Bind == BindOn.Pickup) return;
        if (player.Trade != null) return;

        if (amount > slot.Amount) amount = slot.Amount;

        player.MapInstance.Item.Add(new MapItemInstance(slot.ItemId, amount, player.X, player.Y));
        mapSender.MapItems(player.MapInstance);
        TakeItem(player, slot, amount);
    }

    public void UseItem(Player player, int slotIndex, ItemSlot slot)
    {
        var item = _catalog.Items.Get(slot.ItemId);
        if (item == null) return;
        if (player.Trade != null) return;

        if (player.Level < item.ReqLevel)
        {
            chatSender.Message(player, "You do not have the level required to use this item.", Color.White);
            return;
        }

        if (item.ReqClassId.HasValue && player.Class.Id != item.ReqClassId.Value)
        {
            chatSender.Message(player, "You can not use this item.", Color.White);
            return;
        }

        if (item.Type == ItemType.Equipment)
        {
            GameWorld.Current.CurrentTick?.Events.Emit(new ItemUsedEvent
            {
                Player = player,
                SlotIndex = slotIndex,
                Item = item
            });
        }
        else if (item.Type == ItemType.Potion)
        {
            var hadEffect = false;
            levelingSystem.GiveExperience(player, item.PotionExperience);

            for (byte i = 0; i < (byte)Vital.Count; i++)
            {
                if (player.Vital[i] < player.MaxVital(i) && item.PotionVital[i] != 0) hadEffect = true;

                player.Vital[i] += item.PotionVital[i];
                if (player.Vital[i] < 0) player.Vital[i] = 0;
                if (player.Vital[i] > player.MaxVital(i)) player.Vital[i] = player.MaxVital(i);
            }

            if (player.Vital[(byte)Vital.Hp] == 0)
                GameWorld.Current.CurrentTick?.Events.Emit(new EntityDiedEvent { Entity = player, Source = null });

            if (item.PotionExperience > 0 || hadEffect) TakeItem(player, slot, 1);
        }
    }

    public void CollectItem(Player player)
    {
        var mapItem = player.MapInstance.HasItem(player.X, player.Y);
        if (mapItem == null) return;

        var item = _catalog.Items.Get(mapItem.ItemId);
        if (item == null) return;

        if (GiveItem(player, item, mapItem.Amount))
        {
            player.MapInstance.Item.Remove(mapItem);
            mapSender.MapItems(player.MapInstance);
        }
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case ItemUsedEvent use when use.Item.Type == ItemType.Equipment:
                {
                    var slot = use.Player.Inventory[use.SlotIndex];
                    if (slot.ItemId == Guid.Empty || slot.ItemId != use.Item.Id) continue;
                    TakeItem(use.Player, slot, 1);
                    break;
                }
                case ItemEquippedEvent equip when equip.OldItem != null:
                {
                    if (!GiveItem(equip.Player, equip.OldItem, 1))
                    {
                        if (equip.Player.MapInstance.Item.Count == Config.MaxMapItems) continue;
                        equip.Player.MapInstance.Item.Add(new MapItemInstance(equip.OldItem.Id, 1,
                            equip.Player.X, equip.Player.Y));
                        mapSender.MapItems(equip.Player.MapInstance);
                        playerSender.PlayerInventory(equip.Player);
                    }
                    break;
                }
            }
        }
    }
}
