using System;
using System.Drawing;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Systems;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Network.Handlers;

internal static class TradeHandler
{
    internal static void TradeInvite(Player player, NetDataReader data)
    {
        var name = data.GetString();

        // Encontra o jogador
        var invited = Player.Find(name);

        // Verifica se o jogador está convectado
        if (invited == null)
        {
            ChatSender.Message(player, "The player isn't connected.", Color.White);
            return;
        }

        // Verifica se não está tentando se convidar
        if (invited == player)
        {
            ChatSender.Message(player, "You can't be invited.", Color.White);
            return;
        }

        // Verifica se já tem um grupo
        if (invited.Trade != null)
        {
            ChatSender.Message(player, "The player is already part of a trade.", Color.White);
            return;
        }

        // Verifica se o jogador já está analisando um convite para algum grupo
        if (!string.IsNullOrEmpty(invited.TradeRequest))
        {
            ChatSender.Message(player, "The player is analyzing an invitation of another trade.", Color.White);
            return;
        }

        // Verifica se os jogadores não estão em com a loja aberta
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

        // Verifica se os jogadores estão pertods um do outro
        if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
        {
            ChatSender.Message(player, "You need to be close to the player to start trade.", Color.White);
            return;
        }

        // Convida o jogador
        invited.TradeRequest = player.Name;
        TradeSender.TradeInvitation(invited, player.Name);
    }

    internal static void TradeAccept(Player player)
    {
        var invited = Player.Find(player.TradeRequest);

        // Verifica se já tem um grupo
        if (player.Trade != null)
        {
            ChatSender.Message(player, "You are already part of a trade.", Color.White);
            return;
        }

        // Verifica se quem chamou ainda está disponível
        if (invited == null)
        {
            ChatSender.Message(player, "Who invited you is no longer available.", Color.White);
            return;
        }

        // Verifica se os jogadores estão pertods um do outro
        if (Math.Abs(player.X - invited.X) + Math.Abs(player.Y - invited.Y) != 1)
        {
            ChatSender.Message(player, "You need to be close to the player to accept the trade.", Color.White);
            return;
        }

        // Verifica se  os jogadores não estão em com a loja aberta
        if (invited.Shop != null)
        {
            ChatSender.Message(player, "Who invited you is in the shop.", Color.White);
            return;
        }

        // Entra na troca
        player.Trade = invited;
        invited.Trade = player;
        ChatSender.Message(player, "You have accepted " + invited.Name + "'s trade request.", Color.White);
        ChatSender.Message(invited, player.Name + " has accepted your trade request.", Color.White);

        // Limpa os dadoss
        player.TradeRequest = string.Empty;
        player.TradeOffer = new TradeSlot[MaxInventory];
        invited.TradeOffer = new TradeSlot[MaxInventory];

        // Envia os dados para o grupo
        TradeSender.Trade(player, true);
        TradeSender.Trade(invited, true);
    }

    internal static void TradeDecline(Player player)
    {
        var invited = Player.Find(player.TradeRequest);

        // Recusa o convite
        if (invited != null) ChatSender.Message(invited, player.Name + " decline the trade.", Color.White);
        player.TradeRequest = string.Empty;
    }

    internal static void TradeLeave(Player player)
    {
        TradeSystem.Leave(player);
    }

    internal static void TradeOffer(Player player, NetDataReader data)
    {
        short slot = data.GetShort(), inventorySlot = data.GetShort();
        var amount = Math.Min(data.GetShort(), player.Inventory[inventorySlot].Amount);

        // Adiciona o item à troca
        if (inventorySlot != 0)
        {
            // Evita itens repetidos
            for (byte i = 0; i < MaxInventory; i++)
                if (player.TradeOffer[i].SlotNum == inventorySlot)
                    return;

            player.TradeOffer[slot].SlotNum = inventorySlot;
            player.TradeOffer[slot].Amount = amount;
        }
        // Remove o item da troca
        else
            player.TradeOffer[slot] = new TradeSlot();

        // Envia os dados ao outro jogador
        TradeSender.TradeOffer(player);
        TradeSender.TradeOffer(player.Trade, false);
    }

    internal static void TradeOfferState(Player player, NetDataReader data)
    {
        var state = (TradeStatus)data.GetByte();
        var invited = player.Trade;

        switch (state)
        {
            case TradeStatus.Accepted:
                // Verifica se os jogadores têm espaço disponivel para trocar os itens
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

                // Mensagem de confirmação
                ChatSender.Message(invited, "The offer was accepted.", Color.Green);

                // Dados da oferta
                ItemSlot[] yourInventory = (ItemSlot[])player.Inventory.Clone(),
                    theirInventory = (ItemSlot[])invited.Inventory.Clone();

                // Remove os itens do inventário dos jogadores
                var to = player;
                for (byte j = 0; j < 2; j++, to = to == player ? invited : player)
                for (byte i = 0; i < MaxInventory; i++)
                    to.TakeItem(to.Inventory[to.TradeOffer[i].SlotNum], to.TradeOffer[i].Amount);

                // Dá os itens aos jogadores
                for (byte i = 0; i < MaxInventory; i++)
                {
                    if (player.TradeOffer[i].SlotNum > 0)
                        invited.GiveItem(yourInventory[player.TradeOffer[i].SlotNum].Item, player.TradeOffer[i].Amount);
                    if (invited.TradeOffer[i].SlotNum > 0)
                        player.GiveItem(theirInventory[invited.TradeOffer[i].SlotNum].Item,
                            invited.TradeOffer[i].Amount);
                }

                // Envia os dados do inventário aos jogadores
                PlayerSender.PlayerInventory(player);
                PlayerSender.PlayerInventory(invited);

                // Limpa a troca
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

        // Envia os dados
        TradeSender.TradeState(invited, state);
    }
}
