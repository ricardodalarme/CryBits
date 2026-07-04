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

internal sealed class InventoryItemViewModel
{
    public short Index { get; set; }
    public Guid ItemId { get; set; }
    public short Amount { get; set; }
    public Item? Definition { get; set; }
}

internal sealed class InventoryViewModel : IDisposable
{
    private readonly DefinitionCatalog _catalog;
    private readonly IntentSender _intentSender;

    private readonly IDisposable _inventorySubscription;

    public InventoryItemViewModel[] Slots { get; private set; } = new InventoryItemViewModel[MaxInventory];

    public InventoryViewModel(World world, IntentSender intentSender, DefinitionCatalog catalog)
    {
        _catalog = catalog;
        _intentSender = intentSender;

        _inventorySubscription = world.Events.On<InventoryState>()
            .With<LocalPlayerTag>()
            .OnChanged(OnInventoryChanged);
    }

    private void OnInventoryChanged(ComponentChanged<InventoryState> evt)
    {
        for (var i = 0; i < evt.Component.Slots.Length; i++)
        {
            var slot = evt.Component.Slots[i];
            if (Slots[i] == null)
                Slots[i] = new InventoryItemViewModel { Index = (short)i };

            Slots[i].ItemId = slot.ItemId;
            Slots[i].Amount = slot.Amount;
            Slots[i].Definition = slot.ItemId != Guid.Empty ? _catalog.Items.Get(slot.ItemId) : null;
        }
    }

    public void Swap(short oldSlot, short newSlot)
    {
        _intentSender.Send(new InventorySwapIntent(default, oldSlot, newSlot));
    }

    public void Use(short slot)
    {
        _intentSender.Send(new InventoryUseIntent(default, (byte)slot));
    }

    public void Drop(short slot, short amount)
    {
        _intentSender.Send(new DropItemIntent(default, (byte)slot, amount));
    }

    public void Sell(short slot, short amount)
    {
        _intentSender.Send(new ShopSellIntent(default, (byte)slot, amount));
    }

    public void Dispose() => _inventorySubscription.Dispose();
}
