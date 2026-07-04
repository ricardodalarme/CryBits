using CryBits.Client.Core;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Common;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Intents;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.UI.Game.ViewModels;

internal sealed class TradeViewModel(GameContext context, IntentSender intentSender)
{
    public bool IsOpen { get; set; }

    public TradeSlot[] OwnOffer { get; set; } = new TradeSlot[MaxInventory];

    public TradeSlot[] TheirOffer { get; set; } = new TradeSlot[MaxInventory];

    public void OfferItem(short slot, short inventorySlot, short amount)
    {
        intentSender.Send(new TradeOfferIntent(default, (byte)slot, inventorySlot, amount));
    }

    public void RemoveOfferItem(short slot)
    {
        var inv = context.LocalPlayer.GetInventory();
        if (inv == null || slot >= OwnOffer.Length) return;
        if (inv.Slots[OwnOffer[slot].SlotNum].ItemId == Guid.Empty) return;

        intentSender.Send(new TradeOfferIntent(default, (byte)slot, 0, 0));
    }

    public void Close()
    {
        intentSender.Send(new TradeLeaveIntent(default));
    }

    public void Accept()
    {
        intentSender.Send(new TradeOfferStateIntent(default, TradeStatus.Accepted));
        OwnOffer = new TradeSlot[MaxInventory];
    }

    public void Decline()
    {
        intentSender.Send(new TradeOfferStateIntent(default, TradeStatus.Declined));
    }

    public void Confirm()
    {
        intentSender.Send(new TradeOfferStateIntent(default, TradeStatus.Confirmed));
    }
}
