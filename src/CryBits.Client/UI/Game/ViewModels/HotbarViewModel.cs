using CryBits.Client.Components;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.UI.Game.ViewModels;

internal sealed class HotbarItemViewModel
{
    public short Index { get; set; }
    public SlotType Type { get; set; }
    public short Slot { get; set; }
    public Item? Definition { get; set; }
}

internal sealed class HotbarViewModel : IDisposable
{
    private readonly World _world;
    private readonly IntentSender _intentSender;

    private readonly DefinitionCatalog _catalog;
    private readonly IDisposable _hotbarSubscription;
    private readonly IDisposable _inventorySubscription;

    public HotbarItemViewModel[] Slots { get; } = new HotbarItemViewModel[MaxHotbar];

    public HotbarViewModel(World world, IntentSender intentSender, DefinitionCatalog catalog)
    {
        _world = world;
        _catalog = catalog;
        _intentSender = intentSender;

        _hotbarSubscription = world.Events.On<HotbarState>()
            .With<LocalPlayerTag>()
            .OnChanged(OnHotbarChanged);

        _inventorySubscription = world.Events.On<InventoryState>()
            .With<LocalPlayerTag>()
            .OnChanged(OnInventoryChanged);
    }

    private void OnHotbarChanged(ComponentChanged<HotbarState> evt)
    {
        Rebuild(evt.Entity, evt.Component);
    }

    private void OnInventoryChanged(ComponentChanged<InventoryState> evt)
    {
        var hotbar = _world.Get<HotbarState>(evt.Entity);
        if (hotbar != null) Rebuild(evt.Entity, hotbar);
    }

    private void Rebuild(EntityId entity, HotbarState hotbar)
    {
        var inv = _world.Get<InventoryState>(entity);

        for (var i = 0; i < hotbar.Slots.Length; i++)
        {
            var slot = hotbar.Slots[i];
            Slots[i] ??= new HotbarItemViewModel { Index = (short)i };

            Slots[i].Type = slot.Type;
            Slots[i].Slot = slot.Slot;

            if (slot.Type == SlotType.Item && inv != null && slot.Slot >= 0 && slot.Slot < inv.Slots.Length)
            {
                var itemId = inv.Slots[slot.Slot].ItemId;
                Slots[i].Definition = itemId != Guid.Empty ? _catalog.Items.Get(itemId) : null;
            }
            else
            {
                Slots[i].Definition = null;
            }
        }
    }

    public void Swap(short oldSlot, short newSlot)
    {
        _intentSender.Send(new HotbarSwapIntent(default, oldSlot, (byte)newSlot));
    }

    public void AddItem(short slot, short inventorySlot)
    {
        _intentSender.Send(new HotbarAddIntent(default, (byte)slot, SlotType.Item, inventorySlot));
    }

    public void Remove(short slot)
    {
        _intentSender.Send(new HotbarAddIntent(default, (byte)slot, default, 0));
    }

    public void Use(short slot)
    {
        _intentSender.Send(new HotbarUseIntent(default, (byte)slot));
    }

    public void Dispose()
    {
        _hotbarSubscription.Dispose();
        _inventorySubscription.Dispose();
    }
}
