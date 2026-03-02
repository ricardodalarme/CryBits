using System;
using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Logic;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.UI.Menu;
using CryBits.Client.UI.Menu.Views;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Packets.Server;

using OptionsView = CryBits.Client.UI.Game.Views.OptionsView;

namespace CryBits.Client.Network.Handlers;

internal class AccountHandler(AudioManager audioManager)
{
    [PacketHandler]
    internal void Join(JoinPacket packet)
    {
        // Clear entity collections
        Player.List = [];
        Item.List = [];
        Shop.List = [];
        Npc.List = [];
        Map.List = [];
        MapInstance.List = [];

        // Initialize local player from server data
        Player.Me = new Me(packet.Name);
        Player.List.Add(Player.Me);
    }

    [PacketHandler]
    internal void CreateCharacter(CreateCharacterPacket _)
    {
        // Reset character-creation inputs
        CreateCharacterView.NameTextBox.Text = string.Empty;
        CreateCharacterView.GenderMaleCheckBox.Checked = true;
        CreateCharacterView.GenderFemaleCheckBox.Checked = false;
        CreateCharacterView.CurrentClass = 0;
        CreateCharacterView.CurrentTexture = 0;

        // Show character creation panel
        MenuScreen.CloseMenus();
        CreateCharacterView.CreateCharacterPanel.Visible = true;
    }

    [PacketHandler]
    internal void Characters(CharactersPacket packet)
    {
        // Resize character list
        SelectCharacterView.Characters = new SelectCharacterView.TempCharacter[packet.Characters.Length];

        for (byte i = 0; i < SelectCharacterView.Characters.Length; i++)
        {
            // Read character data
            SelectCharacterView.Characters[i] = new SelectCharacterView.TempCharacter
            {
                Name = packet.Characters[i].Name,
                TextureNum = packet.Characters[i].TextureNum
            };
        }

        SelectCharacterView.UpdateButtonVisibility();
    }

    [PacketHandler]
    internal void JoinGame(JoinGamePacket _)
    {
        // Reset UI state and options
        Chat.Order = [];
        Chat.LinesFirst = 0;
        GameLoop.ChatTimer = Environment.TickCount + Chat.SleepTimer;
        ChatView.MessageTextBox.Text = string.Empty;
        OptionsView.SoundsCheckBox.Checked = Options.Sounds;
        OptionsView.MusicsCheckBox.Checked = Options.Musics;
        OptionsView.ChatCheckBox.Checked = Options.Chat;
        OptionsView.MetricsCheckBox.Checked = Options.ShowMetrics;
        OptionsView.TradeCheckBox.Checked = Options.Trade;
        OptionsView.PartyCheckBox.Checked = Options.Party;
        GameLoop.ChatTimer = GameLoop.ChatTimer = Environment.TickCount + 10000;
        InformationView.Hide();

        // Reset UI panels
        CharacterView.Panel.Visible = false;
        InventoryView.Panel.Visible = false;
        OptionsView.Panel.Visible = false;
        ChatView.Panel.Visible = false;
        DropItemView.Panel.Visible = false;
        PartyInvitationView.Panel.Visible = false;
        TradeView.Panel.Visible = false;
        TradeView.ConfirmOfferButton.Visible = true;
        TradeView.AcceptOfferButton.Visible = false;
        TradeView.DeclineOfferButton.Visible = false;
        TradeView.OfferDisabledPanel.Visible = false;
        ShopView.Panel.Visible = false;
        ShopSellView.Panel.Visible = false;

        // Enter the game
        audioManager.StopMusic();
        Screen.Current = Screens.Game;
    }
}
