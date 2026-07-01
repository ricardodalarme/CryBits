using CryBits.Client.Framework;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Logic;
using CryBits.Client.UI.Menu;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using Iguina.Entities;

namespace CryBits.Client.UI.Game;

internal class GameScreen
{
    public static GameScreen Instance { get; } = new();

    internal readonly CharacterView CharacterView = new(IguinaContext.Instance, GameContext.Instance, IntentSender.Instance, EquipmentRenderer.Instance, CharacterRenderer.Instance);
    internal readonly ChatView ChatView = new(IguinaContext.Instance);
    internal readonly DraggableSlotView DraggableSlotView = new(IguinaContext.Instance, ItemRenderer.Instance, InputManager.Instance, GameContext.Instance, DefinitionCatalog.Instance);
    internal readonly DropItemView DropItemView = new(IguinaContext.Instance, IntentSender.Instance);
    internal readonly HotbarView HotbarView = new(IguinaContext.Instance, IntentSender.Instance, ItemRenderer.Instance, GameContext.Instance, DefinitionCatalog.Instance);
    internal readonly TooltipView InformationView = new(ItemRenderer.Instance);
    internal readonly InventoryView InventoryView = new(IguinaContext.Instance, IntentSender.Instance, ItemRenderer.Instance, GameContext.Instance, DefinitionCatalog.Instance);
    internal readonly MenusView MenusView = new(IguinaContext.Instance);
    internal readonly OptionsView OptionsView = new(IguinaContext.Instance, AudioManager.Instance, GameContext.Instance);
    internal readonly PartyInvitationView PartyInvitationView = new(IguinaContext.Instance, IntentSender.Instance);
    internal readonly ShopSellView ShopSellView = new(IguinaContext.Instance, IntentSender.Instance);
    internal readonly ShopView ShopView = new(IguinaContext.Instance, IntentSender.Instance, ItemRenderer.Instance, DefinitionCatalog.Instance);
    internal readonly TradeAmountView TradeAmountView = new(IguinaContext.Instance, IntentSender.Instance);
    internal readonly TradeInvitationView TradeInvitationView = new(IguinaContext.Instance, IntentSender.Instance);
    internal readonly TradeView TradeView = new(IguinaContext.Instance, IntentSender.Instance, ItemRenderer.Instance, GameContext.Instance, DefinitionCatalog.Instance);
    internal readonly MapNameView MapNameView = new(IguinaContext.Instance, GameContext.Instance);
    internal readonly MetricsView MetricsView = new(IguinaContext.Instance);
    internal readonly PartyView PartyView = new(IguinaContext.Instance, GameContext.Instance);

    public static short? HotbarChange;
    public static short? InventoryChange;

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
        TradeAmountView,
        TradeInvitationView,
        TradeView
    ];

    public void Bind()
    {
        foreach (var view in Views)
            view.Bind();
        GameInput.Instance.Bind();
    }

    public void Unbind()
    {
        GameInput.Instance.Unbind();
        foreach (var view in Views)
            view.Unbind();
    }

    public void Open()
    {
        MenuScreen.Instance.Unbind();
        IguinaContext.Instance.LoadScreen("Game");
        Bind();
        ResetPanels();
        IguinaContext.Instance.CurrentScreen = ScreenType.Game;

        Chat.Order = [];
        Chat.VisibilityTimer = Environment.TickCount64 + Chat.SleepTimer;
        ChatView.MessageTextInput.Value = string.Empty;
        OptionsView.SoundsCheckbox.Checked = Options.Instance.Sounds;
        OptionsView.MusicsCheckbox.Checked = Options.Instance.Musics;
        OptionsView.ChatCheckbox.Checked = Options.Instance.Chat;
        OptionsView.MetricsCheckbox.Checked = Options.Instance.ShowMetrics;
        OptionsView.TradeCheckbox.Checked = Options.Instance.Trade;
        OptionsView.PartyCheckbox.Checked = Options.Instance.Party;
        TooltipView.Hide();
    }

    public void ResetPanels()
    {
        foreach (var name in new[] { "CharacterPanel", "InventoryPanel", "OptionsPanel", "ChatPanel", "Drop", "PartyInvitation", "ShopSell" })
            if (IguinaContext.Instance.TryGet<Panel>(name, out var p)) p.Visible = false;

        TradeView.Panel.Visible = false;
        TradeView.ConfirmOfferButton.Visible = true;
        TradeView.AcceptOfferButton.Visible = false;
        TradeView.DeclineOfferButton.Visible = false;
        TradeView.OfferDisabledPanel.Visible = false;
        ShopView.Panel.Visible = false;
    }
}
