using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using Attribute = CryBits.Definitions.Characters.Attribute;
using CryBits.Server.Simulation.Core;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Server.World;
using System;
using System.Linq;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;

namespace CryBits.Server.Systems.Inventory;

internal sealed class EquipmentSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static EquipmentSystem Instance { get; } = new(DefinitionCatalog.Instance);

    public void Equip(EntityId entityId, Item item)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
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

        GameWorld.Current.CurrentTick?.Events.Emit(new ItemEquippedEvent
        {
            PlayerId = entityId.Value,
            EquipSlot = item.EquipType,
            ItemId = item.Id,
            OldItemId = oldItem?.Id
        });

        GameWorld.Current.Dirty.Mark<EquipmentState>(entityId);
        GameWorld.Current.Dirty.Mark<StatBlock>(entityId);
    }

    public void Unequip(EntityId entityId, byte equipSlot)
    {
        var e = GameWorld.Current.Entities.Get(entityId)!;
        var equip = e.Get<EquipmentState>()!;
        var stats = e.Get<StatBlock>()!;

        var oldItemId = equip.Slots[equipSlot];
        if (oldItemId == Guid.Empty) return;
        var oldItem = _catalog.Items.Get(oldItemId);
        if (oldItem?.Bind == BindOn.Equip) return;

        for (byte i = 0; i < (byte)Attribute.Count; i++)
            stats.Attribute[i] -= oldItem.EquipAttribute[i];
        equip.Slots[equipSlot] = Guid.Empty;

        GameWorld.Current.CurrentTick?.Events.Emit(new ItemEquippedEvent
        {
            PlayerId = entityId.Value,
            EquipSlot = equipSlot,
            ItemId = null,
            OldItemId = oldItem.Id
        });

        GameWorld.Current.Dirty.Mark<EquipmentState>(entityId);
        GameWorld.Current.Dirty.Mark<StatBlock>(entityId);
    }

    public void Execute(GameWorld world, Tick tick)
    {
        foreach (var ev in tick.Events.Events.ToArray())
        {
            if (ev is not ItemUsedEvent use) continue;
            var playerId = world.FindPlayerByValue(use.PlayerId);
            if (playerId == null) continue;
            var item = _catalog.Items.Get(use.ItemId);
            if (item == null || item.Type != ItemType.Equipment) continue;
            Equip(playerId.Value, item);
        }
    }
}
