using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using Attribute = CryBits.Definitions.Characters.Attribute;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using System;
using System.Linq;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Systems.Inventory;

internal sealed class EquipmentSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static EquipmentSystem Instance { get; } = new(DefinitionCatalog.Instance);

    public void Equip(World world, EntityId entityId, Item item)
    {
        var e = world.Entities.Get(entityId)!;
        var equip = e.Get<EquipmentState>()!;
        var stats = e.Get<StatBlock>()!;

        var oldItemId = equip.Slots[item.EquipType];
        var oldItem = oldItemId != Guid.Empty ? _catalog.Items.Get(oldItemId) : null;

        equip.Slots[item.EquipType] = item.Id;
        for (byte i = 0; i < (byte)Attribute.Count; i++)
            stats.Attribute[i] += item.EquipAttribute[i];
        if (oldItem != null)
            for (byte i = 0; i < (byte)Attribute.Count; i++)
                stats.Attribute[i] -= oldItem.EquipAttribute[i];

        world.CurrentTick?.Events.Emit(new ItemEquippedEvent
        {
            PlayerId = entityId.Value,
            EquipSlot = item.EquipType,
            ItemId = item.Id,
            OldItemId = oldItem?.Id
        });

        world.Dirty.Mark<EquipmentState>(entityId);
        world.Dirty.Mark<StatBlock>(entityId);
    }

    public void Unequip(World world, EntityId entityId, byte equipSlot)
    {
        var e = world.Entities.Get(entityId)!;
        var equip = e.Get<EquipmentState>()!;
        var stats = e.Get<StatBlock>()!;

        var oldItemId = equip.Slots[equipSlot];
        if (oldItemId == Guid.Empty) return;
        var oldItem = _catalog.Items.Get(oldItemId);
        if (oldItem?.Bind == BindOn.Equip) return;

        for (byte i = 0; i < (byte)Attribute.Count; i++)
            stats.Attribute[i] -= oldItem.EquipAttribute[i];
        equip.Slots[equipSlot] = Guid.Empty;

        world.CurrentTick?.Events.Emit(new ItemEquippedEvent
        {
            PlayerId = entityId.Value,
            EquipSlot = equipSlot,
            ItemId = null,
            OldItemId = oldItem.Id
        });

        world.Dirty.Mark<EquipmentState>(entityId);
        world.Dirty.Mark<StatBlock>(entityId);
    }

    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            if (intent is EquipmentRemoveIntent remove)
                Unequip(world, remove.SourceEntityId, remove.Slot);
        }

        foreach (var ev in tick.Events.Events.ToArray())
        {
            if (ev is not ItemUsedEvent use) continue;
            var playerId = world.FindPlayerByValue(use.PlayerId);
            if (playerId == null) continue;
            var item = _catalog.Items.Get(use.ItemId);
            if (item == null || item.Type != ItemType.Equipment) continue;
            Equip(world, playerId.Value, item);
        }
    }
}
