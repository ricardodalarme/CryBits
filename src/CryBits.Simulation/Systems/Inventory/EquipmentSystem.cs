using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Simulation.Systems.Inventory;

public sealed class EquipmentSystem : ISimulationSystem
{
    public void Execute(World world, Tick tick)
    {
        foreach (var intent in tick.Intents.All)
            if (intent is EquipmentRemoveIntent remove)
                Unequip(world, tick, remove.SourceEntityId, remove.Slot);

        var events = tick.Events.Events;
        foreach (var t in events)
        {
            if (t is not ItemUsedEvent use) continue;
            if (!world.Has<PlayerTag>(use.PlayerId)) continue;
            var item = world.Catalog.Items.Get(use.ItemId);
            if (item is not { Type: ItemType.Equipment }) continue;
            Equip(world, tick, use.PlayerId, item);
            tick.Events.Emit(new ItemTakenEvent(tick.TickNumber, use.PlayerId, (byte)use.SlotIndex, 1));
        }
    }

    private void Equip(World world, Tick tick, EntityId entityId, Item item)
    {
        if (!world.IsAlive(entityId)) return;
        var equip = world.Get<EquipmentState>(entityId)!;
        var attrs = world.Get<AttributesComponent>(entityId)!;

        var oldItemId = equip.Slots[item.EquipType];
        var oldItem = oldItemId != Guid.Empty ? world.Catalog.Items.Get(oldItemId) : null;

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
        if (!world.IsAlive(entityId)) return;
        var equip = world.Get<EquipmentState>(entityId)!;
        var attrs = world.Get<AttributesComponent>(entityId)!;

        var oldItemId = equip.Slots[equipSlot];
        if (oldItemId == Guid.Empty) return;
        var oldItem = world.Catalog.Items.Get(oldItemId);
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
