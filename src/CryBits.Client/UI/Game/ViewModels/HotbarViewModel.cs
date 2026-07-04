using CryBits.Client.Core;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
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

internal sealed class HotbarViewModel(
    GameContext context,
    IntentSender intentSender,
    DefinitionCatalog catalog)
{
    public HotbarItemViewModel[] Slots { get; private set; } = new HotbarItemViewModel[MaxHotbar];

    public void Refresh()
    {
        var hotbar = context.LocalPlayer.GetHotbar();
        if (hotbar == null) return;

        var inv = context.LocalPlayer.GetInventory();

        for (var i = 0; i < hotbar.Slots.Length; i++)
        {
            var slot = hotbar.Slots[i];
            if (Slots[i] == null)
            {
                Slots[i] = new HotbarItemViewModel { Index = (short)i };
            }

            Slots[i].Type = slot.Type;
            Slots[i].Slot = slot.Slot;

            if (slot.Type == SlotType.Item && inv != null && slot.Slot >= 0 && slot.Slot < inv.Slots.Length)
            {
                var itemId = inv.Slots[slot.Slot].ItemId;
                Slots[i].Definition = itemId != Guid.Empty ? catalog.Items.Get(itemId) : null;
            }
            else
            {
                Slots[i].Definition = null;
            }
        }
    }

    public void Swap(short oldSlot, short newSlot)
    {
        intentSender.Send(new HotbarSwapIntent(default, oldSlot, (byte)newSlot));
    }

    public void AddItem(short slot, short inventorySlot)
    {
        intentSender.Send(new HotbarAddIntent(default, (byte)slot, SlotType.Item, inventorySlot));
    }

    public void Remove(short slot)
    {
        intentSender.Send(new HotbarAddIntent(default, (byte)slot, default, 0));
    }

    public void Use(short slot)
    {
        intentSender.Send(new HotbarUseIntent(default, (byte)slot));
    }
}
