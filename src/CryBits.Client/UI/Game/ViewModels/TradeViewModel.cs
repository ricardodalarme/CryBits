using CryBits.Client.Components;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Intents;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.UI.Game.ViewModels;

internal sealed class TradeOfferItemViewModel
{
    public short SlotNum { get; set; }
    public short Amount { get; set; }
    public Guid ItemId { get; set; }
    public Item? Definition { get; set; }
}

internal sealed class TradeViewModel(World world, IntentSender intentSender, DefinitionCatalog catalog)
{
    private readonly World _world = world;
    private readonly IntentSender _intentSender = intentSender;
    private readonly DefinitionCatalog _catalog = catalog;

    public bool IsOpen { get; set; }

    public TradeOfferItemViewModel[] OwnOffer { get; private set; } = new TradeOfferItemViewModel[MaxInventory];

    public TradeOfferItemViewModel[] TheirOffer { get; private set; } = new TradeOfferItemViewModel[MaxInventory];

    public void OfferItem(short slot, short inventorySlot, short amount)
    {
        _intentSender.Send(new TradeOfferIntent(default, (byte)slot, inventorySlot, amount));
    }

    public void RemoveOfferItem(short slot)
    {
        var localPlayerId = _world.Entities.All.FirstOrDefault(s => s.Has<LocalPlayerTag>())?.Id;

        if (localPlayerId == null || slot >= OwnOffer.Length) return;
        var inv = _world.Get<InventoryState>(localPlayerId.Value);
        if (inv == null) return;

        var offer = OwnOffer[slot];
        if (offer == null || inv.Slots[offer.SlotNum].ItemId == Guid.Empty) return;

        _intentSender.Send(new TradeOfferIntent(default, (byte)slot, 0, 0));
    }

    public void Close()
    {
        _intentSender.Send(new TradeLeaveIntent(default));
    }

    public void Accept()
    {
        _intentSender.Send(new TradeOfferStateIntent(default, TradeStatus.Accepted));
        OwnOffer = new TradeOfferItemViewModel[MaxInventory];
    }

    public void Decline()
    {
        _intentSender.Send(new TradeOfferStateIntent(default, TradeStatus.Declined));
    }

    public void Confirm()
    {
        _intentSender.Send(new TradeOfferStateIntent(default, TradeStatus.Confirmed));
    }

    public void UpdateOwnOffer(PacketsTradeOfferItem[] items)
    {
        for (var i = 0; i < MaxInventory; i++)
        {
            var itemId = items[i].ItemId;
            OwnOffer[i] = new TradeOfferItemViewModel
            {
                SlotNum = (short)i,
                Amount = items[i].Amount,
                ItemId = itemId,
                Definition = itemId != Guid.Empty ? _catalog.Items.Get(itemId) : null
            };
        }
    }

    public void UpdateTheirOffer(PacketsTradeOfferItem[] items)
    {
        for (var i = 0; i < MaxInventory; i++)
        {
            var itemId = items[i].ItemId;
            TheirOffer[i] = new TradeOfferItemViewModel
            {
                SlotNum = (short)i,
                Amount = items[i].Amount,
                ItemId = itemId,
                Definition = itemId != Guid.Empty ? _catalog.Items.Get(itemId) : null
            };
        }
    }

    public void ResetOffers()
    {
        OwnOffer = new TradeOfferItemViewModel[MaxInventory];
        TheirOffer = new TradeOfferItemViewModel[MaxInventory];
    }
}
