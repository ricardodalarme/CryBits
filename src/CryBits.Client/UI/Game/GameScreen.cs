using CryBits.Client.Core;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Input;
using CryBits.Client.Rendering;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.UI.Game.ViewModels;
using Iguina.Entities;

namespace CryBits.Client.UI.Game;

internal class GameScreen
{
    internal readonly CharacterView CharacterView;
    internal readonly ChatView ChatView;
    internal readonly DraggableSlotView DraggableSlotView;
    internal readonly DropItemView DropItemView;
    internal readonly HotbarView HotbarView;
    internal readonly TooltipView InformationView;
    internal readonly InventoryView InventoryView;
    internal readonly MenusView MenusView;
    internal readonly OptionsView OptionsView;
    internal readonly PartyInvitationView PartyInvitationView;
    internal readonly ShopSellView ShopSellView;
    internal readonly ShopView ShopView;
    internal readonly TradeAmountView TradeAmountView;
    internal readonly TradeInvitationView TradeInvitationView;
    internal readonly TradeView TradeView;
    internal readonly MapNameView MapNameView;
    internal readonly MetricsView MetricsView;
    internal readonly PartyView PartyView;
    internal readonly StatsView StatsView;

    internal readonly UiContext UiContext;

    internal GameScreen(GameSession session, UiContext uiContext, SpriteBatch spriteBatch,
        ItemIconRenderer itemRenderer, EquipmentSlotRenderer equipmentRenderer, PortraitRenderer characterRenderer,
        InputManager inputManager, AudioManager audioManager, TooltipView tooltip,
        Chat chat, GameInput gameInput,
        StatsViewModel statsViewModel, CharacterViewModel characterViewModel, InventoryViewModel inventoryViewModel,
        HotbarViewModel hotbarViewModel, TradeViewModel tradeViewModel, PartyViewModel partyViewModel, ShopViewModel shopViewModel)
    {
        UiContext = uiContext;
        ShopView = new(uiContext, itemRenderer, tooltip, shopViewModel);
        InformationView = tooltip;
        InventoryView = new(uiContext, itemRenderer, tooltip, ShopView, this, inventoryViewModel);
        CharacterView = new(uiContext, equipmentRenderer, characterRenderer, tooltip, characterViewModel);
        ChatView = new(uiContext, chat);
        DraggableSlotView = new(uiContext, itemRenderer, inputManager, this, inventoryViewModel, hotbarViewModel);
        DropItemView = new(uiContext, session.IntentSender);
        HotbarView = new(uiContext, itemRenderer, tooltip, InventoryView, this, hotbarViewModel);
        MenusView = new(uiContext);
        OptionsView = new(uiContext, audioManager, session.World, chat);
        PartyInvitationView = new(uiContext, session.IntentSender);
        ShopSellView = new(uiContext, shopViewModel);
        TradeAmountView = new(uiContext, session.IntentSender);
        TradeInvitationView = new(uiContext, session.IntentSender);
        TradeView = new(uiContext, itemRenderer, InventoryView, this, tradeViewModel);
        MapNameView = new(uiContext, session.World);
        MetricsView = new(uiContext);
        PartyView = new(uiContext, spriteBatch, partyViewModel);
        StatsView = new(uiContext, statsViewModel);
        _chat = chat;
        _gameInput = gameInput;
    }

    private readonly Chat _chat;
    private readonly GameInput _gameInput;

    public short? HotbarChange;
    public short? InventoryChange;

    private List<ViewBase> Views =>
    [
        CharacterView,
        ChatView,
        DraggableSlotView,
        DropItemView,
        HotbarView,
        InformationView,
        InventoryView,
        MapNameView,
        MenusView,
        MetricsView,
        OptionsView,
        PartyView,
        PartyInvitationView,
        ShopSellView,
        ShopView,
        StatsView,
        TradeAmountView,
        TradeInvitationView,
        TradeView
    ];

    public void Bind()
    {
        foreach (var view in Views)
            view.Bind();
        _gameInput.Bind();
    }

    public void Unbind()
    {
        _gameInput.Unbind();
        foreach (var view in Views)
            view.Unbind();
    }

    public void Open()
    {
        UiContext.LoadScreen("Game");
        Bind();
        ResetPanels();
        UiContext.CurrentScreen = ScreenType.Game;

        _chat.Order.Clear();
        _chat.VisibilityTimer = Environment.TickCount64 + Chat.SleepTimer;
        ChatView.MessageTextInput.Value = string.Empty;
        OptionsView.SoundsCheckbox.Checked = Options.Instance.Sounds;
        OptionsView.MusicsCheckbox.Checked = Options.Instance.Musics;
        OptionsView.ChatCheckbox.Checked = Options.Instance.Chat;
        OptionsView.MetricsCheckbox.Checked = Options.Instance.ShowMetrics;
        OptionsView.TradeCheckbox.Checked = Options.Instance.Trade;
        OptionsView.PartyCheckbox.Checked = Options.Instance.Party;
        InformationView.Hide();
    }

    public void ResetPanels()
    {
        foreach (var name in new[] { "CharacterPanel", "InventoryPanel", "OptionsPanel", "ChatPanel", "Drop", "PartyInvitation", "ShopSell" })
            if (UiContext.TryGet<Panel>(name, out var p)) p.Visible = false;

        TradeView.Close();
        ShopView.Close();
    }
}
