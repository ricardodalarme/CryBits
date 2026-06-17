using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Iguina;
using CryBits.Client.Managers;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using System.Text.Json;
using Ent = Iguina.Entities.Entity;

namespace CryBits.Client.UI.Game;

internal sealed class GameScreen
{
    public static short HotbarChange;
    public static short InventoryChange;

    private readonly UISystem _ui;
    private readonly GameContext _context;
    private readonly MenuConfig _config;

    private Panel? _rootPanel;
    private bool _isVisible;

    // Views
    private readonly BarsView _barsView;
    private readonly HotbarView _hotbarView;
    private readonly MenusView _menusView;
    private readonly ChatView _chatView;
    private readonly CharacterView _characterView;
    private readonly InventoryView _inventoryView;
    private readonly OptionsView _optionsView;

    private readonly InformationView _informationView;
    private readonly DropItemView _dropItemView;
    private readonly PartyInvitationView _partyInvitationView;
    private readonly TradeInvitationView _tradeInvitationView;
    private readonly TradeAmountView _tradeAmountView;
    private readonly ShopSellView _shopSellView;
    private readonly DraggableSlotView _draggableSlotView;
    private readonly ShopView _shopView;
    private readonly TradeView _tradeView;

    public bool IsVisible => _isVisible;

    public GameScreen(UISystem ui, Renderer renderer, CharacterRenderer characterRenderer,
        EquipmentRenderer equipmentRenderer, ItemRenderer itemRenderer,
        GameContext context, DefinitionCatalog catalog, AudioManager audioManager,
        InputManager inputManager)
    {
        _ui = ui;
        _context = context;
        _config = LoadConfig();

        _barsView = new BarsView(context);
        _hotbarView = new HotbarView(ui, context, catalog, itemRenderer);
        _menusView = new MenusView();
        _chatView = new ChatView(context);
        _characterView = new CharacterView(ui, context, catalog, characterRenderer, equipmentRenderer);
        _inventoryView = new InventoryView(ui, context, catalog, itemRenderer);
        _optionsView = new OptionsView(audioManager, context);

        _informationView = new InformationView(ui, itemRenderer, catalog);
        _dropItemView = new DropItemView(ui);
        _partyInvitationView = new PartyInvitationView(ui);
        _tradeInvitationView = new TradeInvitationView(ui);
        _tradeAmountView = new TradeAmountView(ui);
        _shopSellView = new ShopSellView(ui);
        _draggableSlotView = new DraggableSlotView(ui, itemRenderer, inputManager, context, catalog);
        _shopView = new ShopView(ui, itemRenderer, catalog);
        _tradeView = new TradeView(ui, itemRenderer, context, catalog);
    }

    private static MenuConfig LoadConfig()
    {
        var path = System.IO.Path.Combine(AppContext.BaseDirectory, "IguinaTheme", "game_ui_config.json");
        if (!File.Exists(path)) return new MenuConfig();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<MenuConfig>(json) ?? new MenuConfig();
    }

    public void Show()
    {
        if (_isVisible) return;

        _rootPanel = new Panel(_ui);
        _rootPanel.Size.SetPixels(800, 608);
        _rootPanel.Anchor = Anchor.TopLeft;
        _rootPanel.Offset.SetPixels(0, 0);
        _ui.Root.AddChild(_rootPanel);

        // Build entities from config
        var reg = new Dictionary<string, Ent>();
        foreach (var screen in _config.Screens)
        {
            var (panel, screenReg) = MenuLoader.BuildScreen(_ui, screen, _rootPanel);
            foreach (var (k, v) in screenReg)
                reg[k] = v;
        }

        // Wire views from config registry
        _barsView.Wire(reg);
        _hotbarView.Wire(reg);
        _menusView.Wire(reg);
        _chatView.Wire(reg);
        _characterView.Wire(reg);
        _inventoryView.Wire(reg);
        _optionsView.Wire(reg);

        // Build views that need programmatic entity creation (slot grids)
        _hotbarView.Build(_rootPanel);
        _characterView.Build(_rootPanel);
        _inventoryView.Build(_rootPanel);

        _informationView.Build(_rootPanel);
        _dropItemView.Build(_rootPanel);
        _partyInvitationView.Build(_rootPanel);
        _tradeInvitationView.Build(_rootPanel);
        _tradeAmountView.Build(_rootPanel);
        _shopSellView.Build(_rootPanel);
        _draggableSlotView.Build(_rootPanel);
        _shopView.Build(_rootPanel);
        _tradeView.Build(_rootPanel);

        _menusView.SetPanelToggles(_characterView, _inventoryView, _optionsView);

        GameEvents.BarsUpdated += _barsView.Update;
        GameEvents.CharacterUpdated += _characterView.Update;
        GameEvents.ChatToggle += _chatView.Toggle;

        _isVisible = true;
    }

    public void Hide()
    {
        _isVisible = false;
        _rootPanel?.RemoveSelf();
        _rootPanel = null;
    }

    public void UpdateBars() => _barsView.Update();
    public void UpdateCharacter() => _characterView.Update();
}
