using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Server.Systems.Progression;
using CryBits.Server.World;
using System;
using System.Drawing;
using static CryBits.Definitions.Globals;
using CryBits.Simulation.Core;
using CryBits.Simulation.Entities;
using CryBits.Simulation.State;

namespace CryBits.Server.Systems.Inventory;

internal sealed class InventorySystem(
    LevelingSystem levelingSystem,
    DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static InventorySystem Instance { get; } = new(
        LevelingSystem.Instance,
        DefinitionCatalog.Instance);

    public bool GiveItem(EntityId entityId, Item item, short amount)
    {
        if (item == null) return false;

        var e = GameWorld.Current.Entities.Get(entityId)!;
        var inv = e.Get<InventoryState>()!;

        var slotItem = inv.Find(item.Id);
        var slotEmpty = Array.Find(inv.Slots, x => x.ItemId == Guid.Empty);
        var slotEmptyIndex = slotEmpty != null ? Array.IndexOf(inv.Slots, slotEmpty) : -1;

        if (slotEmptyIndex == -1) return false;
        if (amount == 0) amount = 1;

        if (slotItem != null && item.Stackable)
            slotItem.Amount += amount;
        else
        {
            var emptySlot = inv.Slots[slotEmptyIndex];
            emptySlot.ItemId = item.Id;
            emptySlot.Amount = item.Stackable ? amount : (byte)1;
        }

        GameWorld.Current.Dirty.Mark<InventoryState>(entityId);
        return true;
    }

    public void TakeItem(EntityId entityId, ItemSlot slot, short amount)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var hotbar = e.Get<HotbarState>();
        var inv = e.Get<InventoryState>()!;

        if (slot == null) return;
        if (amount <= 0) amount = 1;

        if (amount == slot.Amount)
        {
            var slotIndex = Array.IndexOf(inv.Slots, slot);
            slot.ItemId = Guid.Empty;
            slot.Amount = 0;

            if (hotbar != null)
            {
                var hotbarSlot = hotbar.Find(SlotType.Item, (short)slotIndex);
                if (hotbarSlot != null)
                {
                    hotbarSlot.Type = SlotType.None;
                    hotbarSlot.Slot = 0;
                    GameWorld.Current.Dirty.Mark<HotbarState>(entityId);
                }
            }
        }
        else
            slot.Amount -= amount;

        GameWorld.Current.Dirty.Mark<InventoryState>(entityId);
    }

    public void DropItem(EntityId entityId, ItemSlot slot, short amount)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var inv = e.Get<InventoryState>()!;
        var pos = e.Get<Position>()!;
        var trade = e.Get<TradeState>();
        var map = GameWorld.Current.Maps.Get(pos.MapId)!;

        if (map.Item.Count == Config.MaxMapItems) return;
        if (slot.ItemId == Guid.Empty) return;
        var item = _catalog.Items.Get(slot.ItemId);
        if (item == null || item.Bind == BindOn.Pickup) return;
        if (trade?.Partner != null) return;

        if (amount > slot.Amount) amount = slot.Amount;

        map.Item.Add(new GroundItem(slot.ItemId, amount, pos.X, pos.Y));
        TakeItem(entityId, slot, amount);
    }

    public void UseItem(EntityId entityId, int slotIndex, ItemSlot slot)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var stats = e.Get<StatBlock>()!;
        var vitals = e.Get<Vitals>()!;
        var appearance = e.Get<PlayerAppearance>()!;
        var trade = e.Get<TradeState>();
        var catalog = DefinitionCatalog.Instance;

        var item = _catalog.Items.Get(slot.ItemId);
        if (item == null) return;
        if (trade?.Partner != null) return;

        if (stats.Level < item.ReqLevel)
        {
            GameWorld.Current.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You do not have the level required to use this item.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (item.ReqClassId.HasValue && appearance.ClassId != item.ReqClassId.Value)
        {
            GameWorld.Current.CurrentTick?.Events.Emit(new ChatMessageEvent { RecipientId = entityId.Value, Text = "You can not use this item.", ColorArgb = Color.White.ToArgb() });
            return;
        }

        if (item.Type == ItemType.Equipment)
        {
            GameWorld.Current.CurrentTick?.Events.Emit(new ItemUsedEvent
            {
                PlayerId = entityId.Value,
                SlotIndex = slotIndex,
                ItemId = item.Id
            });
        }
        else if (item.Type == ItemType.Potion)
        {
            var hadEffect = false;
            levelingSystem.GiveExperience(entityId, item.PotionExperience);

            for (byte i = 0; i < (byte)Vital.Count; i++)
            {
                var current = i == 0 ? vitals.Hp : vitals.Mp;
                var max = i == 0 ? vitals.MaxHp : vitals.MaxMp;

                if (current < max && item.PotionVital[i] != 0) hadEffect = true;

                current += item.PotionVital[i];
                if (current < 0) current = 0;
                if (current > max) current = max;
                if (i == 0) vitals.Hp = current; else vitals.Mp = current;
            }

            GameWorld.Current.Dirty.Mark<Vitals>(entityId);

            if (vitals.Hp == 0)
                GameWorld.Current.CurrentTick?.Events.Emit(new EntityDiedEvent { EntityId = entityId.Value, EntityIsPlayer = true, SourceId = null, SourceIsPlayer = null });

            if (item.PotionExperience > 0 || hadEffect) TakeItem(entityId, slot, 1);
        }
    }

    public void CollectItem(EntityId entityId)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var pos = e.Get<Position>()!;
        var map = GameWorld.Current.Maps.Get(pos.MapId)!;

        var mapItem = map.HasItem(pos.X, pos.Y);
        if (mapItem == null) return;

        var item = _catalog.Items.Get(mapItem.ItemId);
        if (item == null) return;

        if (GiveItem(entityId, item, mapItem.Amount))
        {
            map.Item.Remove(mapItem);
        }
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case ItemUsedEvent use:
                    {
                        var playerId = world.FindPlayerByValue(use.PlayerId);
                        if (playerId == null) continue;
                        var e = world.Entities.Get(playerId.Value)!;
                        var inv = e.Get<InventoryState>()!;
                        var item = _catalog.Items.Get(use.ItemId);
                        if (item == null || item.Type != ItemType.Equipment) continue;
                        var slot = inv.Slots[use.SlotIndex];
                        if (slot.ItemId == Guid.Empty || slot.ItemId != use.ItemId) continue;
                        TakeItem(playerId.Value, slot, 1);
                        break;
                    }
                case ItemEquippedEvent equip when equip.OldItemId.HasValue:
                    {
                        var playerId = world.FindPlayerByValue(equip.PlayerId);
                        if (playerId == null) continue;
                        var e = world.Entities.Get(playerId.Value)!;
                        var inv = e.Get<InventoryState>()!;
                        var pos = e.Get<Position>()!;
                        var map = world.Maps.Get(pos.MapId)!;

                        var oldItem = _catalog.Items.Get(equip.OldItemId.Value);
                        if (oldItem == null) continue;
                        if (!GiveItem(playerId.Value, oldItem, 1))
                        {
                            if (map.Item.Count == Config.MaxMapItems) continue;
                            map.Item.Add(new GroundItem(equip.OldItemId.Value, 1,
                                pos.X, pos.Y));
                        }
                        break;
                    }
            }
        }
    }
}
