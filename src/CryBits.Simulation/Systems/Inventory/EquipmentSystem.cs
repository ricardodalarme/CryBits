using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using Attribute = CryBits.Definitions.Characters.Attribute;
using CryBits.Simulation.Components;
using CryBits.Simulation.Events;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class EquipmentSystem(DefinitionCatalog catalog) : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
        {
            if (intent is EquipmentRemoveIntent remove)
                Unequip(world, tick, remove.SourceEntityId, remove.Slot);
        }

        var events = tick.Events.Events;
        for (var i = 0; i < events.Count; i++)
        {
            if (events[i] is not ItemUsedEvent use) continue;
            var playerId = world.FindPlayer(use.PlayerId);
            if (playerId == null) continue;
            var item = catalog.Items.Get(use.ItemId);
            if (item == null || item.Type != ItemType.Equipment) continue;
            Equip(world, tick, playerId.Value, item);
            tick.Events.Emit(new ItemTakenEvent
            {
                EntityId = use.PlayerId,
                SlotIndex = (byte)use.SlotIndex,
                Amount = 1
            });
        }
    }

    private void Equip(World world, Tick tick, EntityId entityId, Item item)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var equip = e.Get<EquipmentState>()!;
        var attrs = e.Get<AttributesComponent>()!;

        var oldItemId = equip.Slots[item.EquipType];
        var oldItem = oldItemId != Guid.Empty ? catalog.Items.Get(oldItemId) : null;

        equip.Slots[item.EquipType] = item.Id;
        for (byte i = 0; i < (byte)Attribute.Count; i++)
            attrs.Values[i] += item.EquipAttribute[i];
        if (oldItem != null)
            for (byte i = 0; i < (byte)Attribute.Count; i++)
                attrs.Values[i] -= oldItem.EquipAttribute[i];

        tick.Events.Emit(new ItemEquippedEvent
        {
            PlayerId = entityId,
            EquipSlot = item.EquipType,
            ItemId = item.Id,
            OldItemId = oldItem?.Id
        });

        world.MarkDirty<EquipmentState>(entityId);
        world.MarkDirty<AttributesComponent>(entityId);
    }

    private void Unequip(World world, Tick tick, EntityId entityId, byte equipSlot)
    {
        var e = world.Entities.Get(entityId);
        if (e == null) return;
        var equip = e.Get<EquipmentState>()!;
        var attrs = e.Get<AttributesComponent>()!;

        var oldItemId = equip.Slots[equipSlot];
        if (oldItemId == Guid.Empty) return;
        var oldItem = catalog.Items.Get(oldItemId);
        if (oldItem is null || oldItem.Bind == BindOn.Equip) return;

        for (byte i = 0; i < (byte)Attribute.Count; i++)
            attrs.Values[i] -= oldItem.EquipAttribute[i];
        equip.Slots[equipSlot] = Guid.Empty;

        tick.Events.Emit(new ItemEquippedEvent
        {
            PlayerId = entityId,
            EquipSlot = equipSlot,
            ItemId = null,
            OldItemId = oldItem.Id
        });

        world.MarkDirty<EquipmentState>(entityId);
        world.MarkDirty<AttributesComponent>(entityId);
    }
}
