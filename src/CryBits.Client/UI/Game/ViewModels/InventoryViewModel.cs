using CryBits.Client.Core;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
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

internal sealed class InventoryViewModel(
    GameContext context,
    IntentSender intentSender,
    DefinitionCatalog catalog)
{
    public InventoryItemViewModel[] Slots { get; private set; } = new InventoryItemViewModel[MaxInventory];

    public void Refresh()
    {
        var inv = context.LocalPlayer.GetInventory();
        if (inv == null) return;

        for (var i = 0; i < inv.Slots.Length; i++)
        {
            var slot = inv.Slots[i];
            if (Slots[i] == null)
            {
                Slots[i] = new InventoryItemViewModel { Index = (short)i };
            }

            Slots[i].ItemId = slot.ItemId;
            Slots[i].Amount = slot.Amount;
            Slots[i].Definition = slot.ItemId != Guid.Empty ? catalog.Items.Get(slot.ItemId) : null;
        }
    }

    public void Swap(short oldSlot, short newSlot)
    {
        intentSender.Send(new InventorySwapIntent(default, oldSlot, newSlot));
    }

    public void Use(short slot)
    {
        intentSender.Send(new InventoryUseIntent(default, (byte)slot));
    }

    public void Drop(short slot, short amount)
    {
        intentSender.Send(new DropItemIntent(default, (byte)slot, amount));
    }

    public void Sell(short slot, short amount)
    {
        intentSender.Send(new ShopSellIntent(default, (byte)slot, amount));
    }
}
