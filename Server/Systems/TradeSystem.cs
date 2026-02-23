using System;
using System.Drawing;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.World;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>System that owns all trade lifecycle logic.</summary>
internal static class TradeSystem
{
    private static CryBits.Server.ECS.Core.World World => ServerContext.Instance.World;

    // Helper: resolve a Player from an entity ID stored inside TradeComponent
    private static Player? PlayerFromEntityId(int? entityId)
    {
        if (entityId == null) return null;
        var session = GameWorld.Current.Sessions
            .Find(s => s.IsPlaying && s.Character?.EntityId == entityId.Value);
        return session?.Character;
    }

    /// <summary>Sends a trade invitation from <paramref name="player"/> to the named target.</summary>
    internal static void Invite(Player player, string targetName)
    {
        var invited = Player.Find(targetName);

        if (invited == null)
        {
            ChatSender.Message(player, "The player isn't connected.", Color.White);
            return;
        }

        if (invited == player)
        {
            ChatSender.Message(player, "You can't be invited.", Color.White);
            return;
        }

        var invitedTrade = World.Get<TradeComponent>(invited.EntityId);
        if (invitedTrade.PartnerId != null)
        {
            ChatSender.Message(player, "The player is already part of a trade.", Color.White);
            return;
        }

        if (!string.IsNullOrEmpty(invitedTrade.PendingRequest))
        {
            ChatSender.Message(player, "The player is analyzing an invitation of another trade.", Color.White);
            return;
        }

        if (player.Has<ShopComponent>())
        {
            ChatSender.Message(player, "You can't start a trade while in the shop.", Color.White);
            return;
        }

        if (invited.Has<ShopComponent>())
        {
            ChatSender.Message(player, "The player is in the shop.", Color.White);
            return;
        }

        var playerPos  = player.Get<PositionComponent>();
        var invitedPos = invited.Get<PositionComponent>();
        if (Math.Abs(playerPos.X - invitedPos.X) + Math.Abs(playerPos.Y - invitedPos.Y) != 1)
        {
            ChatSender.Message(player, "You need to be close to the player to start trade.", Color.White);
            return;
        }

        var pd = player.Get<PlayerDataComponent>();
        invitedTrade.PendingRequest = pd.Name;
        TradeSender.TradeInvitation(invited, pd.Name);
    }

    /// <summary>Accepts the pending trade invitation for <paramref name="player"/>.</summary>
    internal static void Accept(Player player)
    {
        var trade = World.Get<TradeComponent>(player.EntityId);
        var invited = Player.Find(trade.PendingRequest);

        if (trade.PartnerId != null)
        {
            ChatSender.Message(player, "You are already part of a trade.", Color.White);
            return;
        }

        if (invited == null)
        {
            ChatSender.Message(player, "Who invited you is no longer available.", Color.White);
            return;
        }

        var playerPos  = player.Get<PositionComponent>();
        var invitedPos = invited.Get<PositionComponent>();
        if (Math.Abs(playerPos.X - invitedPos.X) + Math.Abs(playerPos.Y - invitedPos.Y) != 1)
        {
            ChatSender.Message(player, "You need to be close to the player to accept the trade.", Color.White);
            return;
        }

        if (invited.Has<ShopComponent>())
        {
            ChatSender.Message(player, "Who invited you is in the shop.", Color.White);
            return;
        }

        var invitedTrade = World.Get<TradeComponent>(invited.EntityId);
        trade.PartnerId    = invited.EntityId;
        invitedTrade.PartnerId = player.EntityId;

        var invitedPd = invited.Get<PlayerDataComponent>();
        var playerPd  = player.Get<PlayerDataComponent>();

        ChatSender.Message(player, "You have accepted " + invitedPd.Name + "'s trade request.", Color.White);
        ChatSender.Message(invited, playerPd.Name + " has accepted your trade request.", Color.White);

        trade.PendingRequest = string.Empty;
        trade.Offer          = new TradeSlot[MaxInventory];
        invitedTrade.Offer   = new TradeSlot[MaxInventory];

        TradeSender.Trade(player, true);
        TradeSender.Trade(invited, true);
    }

    /// <summary>Declines the pending trade invitation for <paramref name="player"/>.</summary>
    internal static void Decline(Player player)
    {
        var trade   = World.Get<TradeComponent>(player.EntityId);
        var invited = Player.Find(trade.PendingRequest);
        if (invited != null)
        {
            var pd = player.Get<PlayerDataComponent>();
            ChatSender.Message(invited, pd.Name + " decline the trade.", Color.White);
        }
        trade.PendingRequest = string.Empty;
    }

    /// <summary>Cancels the active trade for <paramref name="player"/>, notifying both sides.</summary>
    public static void Leave(Player player)
    {
        var trade = World.Get<TradeComponent>(player.EntityId);
        if (trade.PartnerId == null) return;

        var partner = PlayerFromEntityId(trade.PartnerId);
        if (partner != null)
        {
            var partnerTrade = World.Get<TradeComponent>(partner.EntityId);
            partnerTrade.PartnerId = null;
            TradeSender.Trade(partner, false);
        }

        trade.PartnerId = null;
        TradeSender.Trade(player, false);
    }

    /// <summary>Adds or removes an item from <paramref name="player"/>'s trade offer.</summary>
    internal static void Offer(Player player, short slot, short inventorySlot, short amount)
    {
        var inv   = player.Get<InventoryComponent>();
        var trade = World.Get<TradeComponent>(player.EntityId);
        amount = Math.Min(amount, inv.Slots[inventorySlot].Amount);

        if (inventorySlot != 0)
        {
            for (byte i = 0; i < MaxInventory; i++)
                if (trade.Offer![i].SlotNum == inventorySlot)
                    return;

            trade.Offer![slot].SlotNum = inventorySlot;
            trade.Offer[slot].Amount   = amount;
        }
        else
            trade.Offer![slot] = new TradeSlot();

        var partner = PlayerFromEntityId(trade.PartnerId);
        TradeSender.TradeOffer(player);
        if (partner != null) TradeSender.TradeOffer(partner, false);
    }

    /// <summary>Updates the trade offer state (accept / decline / waiting) for <paramref name="player"/>.</summary>
    internal static void OfferState(Player player, TradeStatus state)
    {
        var trade   = World.Get<TradeComponent>(player.EntityId);
        var partner = PlayerFromEntityId(trade.PartnerId);
        if (partner == null) return;

        switch (state)
        {
            case TradeStatus.Accepted:
                if (player.TotalTradeItems > partner.TotalInventoryFree)
                {
                    ChatSender.Message(partner,
                        partner.Get<PlayerDataComponent>().Name + " don't have enough space in their inventory to do this trade.", Color.Red);
                    break;
                }

                if (partner.TotalTradeItems > player.TotalInventoryFree)
                {
                    ChatSender.Message(partner, "You don't have enough space in your inventory to do this trade.",
                        Color.Red);
                    break;
                }

                ChatSender.Message(partner, "The offer was accepted.", Color.Green);

                var partnerTrade = World.Get<TradeComponent>(partner.EntityId);
                var playerInv    = player.Get<InventoryComponent>();
                var partnerInv   = partner.Get<InventoryComponent>();

                ItemSlot[] yourInventory   = (ItemSlot[])playerInv.Slots.Clone(),
                           theirInventory  = (ItemSlot[])partnerInv.Slots.Clone();

                // Remove offered items from both participants
                var pt = player;
                for (byte j = 0; j < 2; j++, pt = pt == player ? partner : player)
                {
                    var ptTrade = World.Get<TradeComponent>(pt.EntityId);
                    var ptInv   = pt.Get<InventoryComponent>();
                    for (byte i = 0; i < MaxInventory; i++)
                        InventorySystem.TakeItem(pt, ptInv.Slots[ptTrade.Offer![i].SlotNum], ptTrade.Offer[i].Amount);
                }

                for (byte i = 0; i < MaxInventory; i++)
                {
                    if (trade.Offer![i].SlotNum > 0)
                        InventorySystem.GiveItem(partner, yourInventory[trade.Offer[i].SlotNum].Item,
                            trade.Offer[i].Amount);
                    if (partnerTrade.Offer![i].SlotNum > 0)
                        InventorySystem.GiveItem(player, theirInventory[partnerTrade.Offer[i].SlotNum].Item,
                            partnerTrade.Offer[i].Amount);
                }

                PlayerSender.PlayerInventory(player);
                PlayerSender.PlayerInventory(partner);

                trade.Offer        = new TradeSlot[MaxInventory];
                partnerTrade.Offer = new TradeSlot[MaxInventory];
                TradeSender.TradeOffer(partner);
                TradeSender.TradeOffer(partner, false);
                break;

            case TradeStatus.Declined:
                ChatSender.Message(partner, "The offer was declined.", Color.Red);
                break;

            case TradeStatus.Waiting:
                ChatSender.Message(partner, player.Get<PlayerDataComponent>().Name + " send you a offer.", Color.White);
                break;
        }

        TradeSender.TradeState(partner, state);
    }
}
    /// <summary>Sends a trade invitation from <paramref name="player"/> to the named target.</summary>
    internal static void Invite(Player player, string targetName)
    {
        var invited = Player.Find(targetName);

        if (invited == null)
        {
            ChatSender.Message(player, "The player isn't connected.", Color.White);
            return;
        }

        if (invited == player)
        {
            ChatSender.Message(player, "You can't be invited.", Color.White);
            return;
        }

        if (invited.Trade != null)
        {
            ChatSender.Message(player, "The player is already part of a trade.", Color.White);
            return;
        }

        if (!string.IsNullOrEmpty(invited.TradeRequest))
        {
            ChatSender.Message(player, "The player is analyzing an invitation of another trade.", Color.White);
            return;
        }

        if (player.Shop != null)
        {
            ChatSender.Message(player, "You can't start a trade while in the shop.", Color.White);
            return;
        }

        if (invited.Shop != null)
        {
            ChatSender.Message(player, "The player is in the shop.", Color.White);
            return;
        }

        if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
        {
            ChatSender.Message(player, "You need to be close to the player to start trade.", Color.White);
            return;
        }

        invited.TradeRequest = player.Name;
        TradeSender.TradeInvitation(invited, player.Name);
    }

    /// <summary>Accepts the pending trade invitation for <paramref name="player"/>.</summary>
    internal static void Accept(Player player)
    {
        var invited = Player.Find(player.TradeRequest);

        if (player.Trade != null)
        {
            ChatSender.Message(player, "You are already part of a trade.", Color.White);
            return;
        }

        if (invited == null)
        {
            ChatSender.Message(player, "Who invited you is no longer available.", Color.White);
            return;
        }

        if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
        {
            ChatSender.Message(player, "You need to be close to the player to accept the trade.", Color.White);
            return;
        }

        if (invited.Shop != null)
        {
            ChatSender.Message(player, "Who invited you is in the shop.", Color.White);
            return;
        }

        player.Trade = invited;
        invited.Trade = player;
        ChatSender.Message(player, "You have accepted " + invited.Name + "'s trade request.", Color.White);
        ChatSender.Message(invited, player.Name + " has accepted your trade request.", Color.White);

        player.TradeRequest = string.Empty;
        player.TradeOffer = new TradeSlot[MaxInventory];
        invited.TradeOffer = new TradeSlot[MaxInventory];

        TradeSender.Trade(player, true);
        TradeSender.Trade(invited, true);
    }

    /// <summary>Declines the pending trade invitation for <paramref name="player"/>.</summary>
    internal static void Decline(Player player)
    {
        var invited = Player.Find(player.TradeRequest);
        if (invited != null) ChatSender.Message(invited, player.Name + " decline the trade.", Color.White);
        player.TradeRequest = string.Empty;
    }

    /// <summary>Cancels the active trade for <paramref name="player"/>, notifying both sides.</summary>
    public static void Leave(Player player)
    {
        if (player.Trade == null) return;

        player.Trade.Trade = null;
        TradeSender.Trade(player.Trade, false);
        player.Trade = null;
        TradeSender.Trade(player, false);
    }

    /// <summary>Adds or removes an item from <paramref name="player"/>'s trade offer.</summary>
    internal static void Offer(Player player, short slot, short inventorySlot, short amount)
    {
        amount = Math.Min(amount, player.Inventory[inventorySlot].Amount);

        if (inventorySlot != 0)
        {
            for (byte i = 0; i < MaxInventory; i++)
                if (player.TradeOffer[i].SlotNum == inventorySlot)
                    return;

            player.TradeOffer[slot].SlotNum = inventorySlot;
            player.TradeOffer[slot].Amount = amount;
        }
        else
            player.TradeOffer[slot] = new TradeSlot();

        TradeSender.TradeOffer(player);
        TradeSender.TradeOffer(player.Trade, false);
    }

    /// <summary>Updates the trade offer state (accept / decline / waiting) for <paramref name="player"/>.</summary>
    internal static void OfferState(Player player, TradeStatus state)
    {
        var invited = player.Trade;

        switch (state)
        {
            case TradeStatus.Accepted:
                if (player.TotalTradeItems > invited.TotalInventoryFree)
                {
                    ChatSender.Message(invited,
                        invited.Name + " don't have enough space in their inventory to do this trade.", Color.Red);
                    break;
                }

                if (invited.TotalTradeItems > player.TotalInventoryFree)
                {
                    ChatSender.Message(invited, "You don't have enough space in your inventory to do this trade.",
                        Color.Red);
                    break;
                }

                ChatSender.Message(invited, "The offer was accepted.", Color.Green);

                ItemSlot[] yourInventory = (ItemSlot[])player.Inventory.Clone(),
                    theirInventory = (ItemSlot[])invited.Inventory.Clone();

                var to = player;
                for (byte j = 0; j < 2; j++, to = to == player ? invited : player)
                    for (byte i = 0; i < MaxInventory; i++)
                        InventorySystem.TakeItem(to, to.Inventory[to.TradeOffer[i].SlotNum], to.TradeOffer[i].Amount);

                for (byte i = 0; i < MaxInventory; i++)
                {
                    if (player.TradeOffer[i].SlotNum > 0)
                        InventorySystem.GiveItem(invited, yourInventory[player.TradeOffer[i].SlotNum].Item,
                            player.TradeOffer[i].Amount);
                    if (invited.TradeOffer[i].SlotNum > 0)
                        InventorySystem.GiveItem(player, theirInventory[invited.TradeOffer[i].SlotNum].Item,
                            invited.TradeOffer[i].Amount);
                }

                PlayerSender.PlayerInventory(player);
                PlayerSender.PlayerInventory(invited);

                player.TradeOffer = new TradeSlot[MaxInventory];
                invited.TradeOffer = new TradeSlot[MaxInventory];
                TradeSender.TradeOffer(invited);
                TradeSender.TradeOffer(invited, false);
                break;

            case TradeStatus.Declined:
                ChatSender.Message(invited, "The offer was declined.", Color.Red);
                break;

            case TradeStatus.Waiting:
                ChatSender.Message(invited, player.Name + " send you a offer.", Color.White);
                break;
        }

        TradeSender.TradeState(invited, state);
    }
}
