using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using Attribute = CryBits.Definitions.Characters.Attribute;

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
            tick.Events.Emit(new ItemTakenEvent(tick.TickNumber, use.PlayerId, (byte)use.SlotIndex, (short)1));
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

        var newSlots = (Guid[])equip.Slots.Clone();
        newSlots[item.EquipType] = item.Id;

        var newValues = (short[])attrs.Values.Clone();
        for (byte i = 0; i < (byte)Attribute.Count; i++)
            newValues[i] += item.EquipAttribute[i];
        if (oldItem != null)
            for (byte i = 0; i < (byte)Attribute.Count; i++)
                newValues[i] -= oldItem.EquipAttribute[i];

        world.Set(entityId, new EquipmentState(newSlots));
        world.Set(entityId, new AttributesComponent(newValues));

        tick.Events.Emit(new ItemEquippedEvent(tick.TickNumber, entityId, item.EquipType, item.Id, oldItem?.Id));
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

        var newValues = (short[])attrs.Values.Clone();
        for (byte i = 0; i < (byte)Attribute.Count; i++)
            newValues[i] -= oldItem.EquipAttribute[i];

        var newSlots = (Guid[])equip.Slots.Clone();
        newSlots[equipSlot] = Guid.Empty;

        world.Set(entityId, new EquipmentState(newSlots));
        world.Set(entityId, new AttributesComponent(newValues));

        tick.Events.Emit(new ItemEquippedEvent(tick.TickNumber, entityId, equipSlot, null, oldItem.Id));
    }
}
