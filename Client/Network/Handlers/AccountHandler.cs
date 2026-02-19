using System;
using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Logic;
using CryBits.Client.UI;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Handlers;

internal static class AccountHandler
{
    internal static void Join(NetDataReader data)
    {
        // Reseta alguns valores
        Player.List = [];
        Item.List = [];
        Shop.List = [];
        Npc.List = [];
        Map.List = [];
        TempMap.List = [];

        // Definir os valores que são enviados do servidor
        Player.Me = new Me(data.GetString());
        Player.List.Add(Player.Me);
    }

    internal static void CreateCharacter()
    {
        // Reseta os valores
        TextBoxes.CreateCharacterName.Text = string.Empty;
        CheckBoxes.GenderMale.Checked = true;
        CheckBoxes.GenderFemale.Checked = false;
        PanelsEvents.CreateCharacterClass = 0;
        PanelsEvents.CreateCharacterTex = 0;

        // Abre o painel de criação de personagem
        PanelsEvents.MenuClose();
        Panels.CreateCharacter.Visible = true;
    }

    internal static void Characters(NetDataReader data)
    {
        // Redimensiona a lista
        PanelsEvents.Characters = new PanelsEvents.TempCharacter[data.GetByte()];

        for (byte i = 0; i < PanelsEvents.Characters.Length; i++)
        {
            // Recebe os dados do personagem
            PanelsEvents.Characters[i] = new PanelsEvents.TempCharacter
            {
                Name = data.GetString(),
                TextureNum = data.GetShort()
            };
        }
    }

    internal static void JoinGame()
    {
        // Reseta os valores
        Chat.Order = [];
        Chat.LinesFirst = 0;
        Loop.ChatTimer = Environment.TickCount + Chat.SleepTimer;
        TextBoxes.Chat.Text = string.Empty;
        CheckBoxes.OptionsSounds.Checked = Options.Sounds;
        CheckBoxes.OptionsMusics.Checked = Options.Musics;
        CheckBoxes.OptionsChat.Checked = Options.Chat;
        CheckBoxes.OptionsFps.Checked = Options.Fps;
        CheckBoxes.OptionsLatency.Checked = Options.Latency;
        CheckBoxes.OptionsTrade.Checked = Options.Trade;
        CheckBoxes.OptionsParty.Checked = Options.Party;
        Loop.ChatTimer = Loop.ChatTimer = Environment.TickCount + 10000;
        PanelsEvents.InformationId = Guid.Empty;

        // Reseta a interface
        Panels.MenuCharacter.Visible = false;
        Panels.MenuInventory.Visible = false;
        Panels.MenuOptions.Visible = false;
        Panels.Chat.Visible = false;
        Panels.Drop.Visible = false;
        Panels.PartyInvitation.Visible = false;
        Panels.Trade.Visible = false;
        Buttons.TradeOfferConfirm.Visible = true;
        Buttons.TradeOfferAccept.Visible = Buttons.TradeOfferDecline.Visible = false;
        Panels.TradeOfferDisable.Visible = false;
        Panels.Shop.Visible = false;
        Panels.ShopSell.Visible = false;

        // Abre o jogo
        Music.Stop();
        Screen.Current = Screens.Game;
    }
}
