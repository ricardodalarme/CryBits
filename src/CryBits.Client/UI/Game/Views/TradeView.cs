using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Slots;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using static CryBits.Definitions.Globals;
using DrawingPoint = System.Drawing.Point;
using IguinaRect = Iguina.Defs.Rectangle;

namespace CryBits.Client.UI.Game.Views;

internal sealed class TradeView
{
    private static TradeView? _instance;
    private readonly TradeSender _tradeSender;
    private readonly ItemRenderer _itemRenderer;
    private readonly GameContext _context;
    private readonly DefinitionCatalog _catalog;
    private readonly UISystem _ui;

    private Panel? _panel;
    private Panel? _offerDisabledPanel;
    private Button? _closeButton;
    private Button? _acceptOfferButton;
    private Button? _declineOfferButton;
    private Button? _confirmOfferButton;
    private IguinaSlotGrid? _ownGrid;
    private IguinaSlotGrid? _theirGrid;

    public static short OwnSlot;
    public static short InventorySlot;

    public TradeView(UISystem ui, ItemRenderer itemRenderer, GameContext context, DefinitionCatalog catalog)
    {
        _instance = this;
        _ui = ui;
        _tradeSender = TradeSender.Instance;
        _itemRenderer = itemRenderer;
        _context = context;
        _catalog = catalog;
    }

    public void Wire(System.Collections.Generic.Dictionary<string, global::Iguina.Entities.Entity> reg) { }
    public void Build(Panel root)
    {
        _panel = new Panel(_ui);
        _panel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/19.png",
            SourceRect = new IguinaRect { Width = 420, Height = 320 }
        };
        _panel.Size.SetPixels(420, 320);
        _panel.Anchor = Anchor.TopLeft;
        _panel.Offset.SetPixels(213, 130);
        _panel.Visible = false;
        root.AddChild(_panel);

        _closeButton = new Button(_ui);
        _closeButton.OverrideStyles.Icon = new IconTexture
        {
            TextureId = "Textures/Buttons/13.png",
            SourceRect = new IguinaRect { Width = 32, Height = 32 }
        };
        _closeButton.Size.SetPixels(32, 32);
        _closeButton.Anchor = Anchor.TopLeft;
        _closeButton.Offset.SetPixels(358, 10);
        _closeButton.Paragraph.Text = string.Empty;
        _closeButton.Events.OnClick += _ => OnClosePressed();
        _panel.AddChild(_closeButton);

        _confirmOfferButton = new Button(_ui);
        _confirmOfferButton.OverrideStyles.Icon = new IconTexture
        {
            TextureId = "Textures/Buttons/29.png",
            SourceRect = new IguinaRect { Width = 32, Height = 32 }
        };
        _confirmOfferButton.Size.SetPixels(32, 32);
        _confirmOfferButton.Anchor = Anchor.TopLeft;
        _confirmOfferButton.Offset.SetPixels(6, 268);
        _confirmOfferButton.Paragraph.Text = string.Empty;
        _confirmOfferButton.Events.OnClick += _ => OnConfirmOfferPressed();
        _panel.AddChild(_confirmOfferButton);

        _acceptOfferButton = new Button(_ui);
        _acceptOfferButton.OverrideStyles.Icon = new IconTexture
        {
            TextureId = "Textures/Buttons/27.png",
            SourceRect = new IguinaRect { Width = 32, Height = 32 }
        };
        _acceptOfferButton.Size.SetPixels(32, 32);
        _acceptOfferButton.Anchor = Anchor.TopLeft;
        _acceptOfferButton.Offset.SetPixels(6, 268);
        _acceptOfferButton.Paragraph.Text = string.Empty;
        _acceptOfferButton.Visible = false;
        _acceptOfferButton.Events.OnClick += _ => OnAcceptOfferPressed();
        _panel.AddChild(_acceptOfferButton);

        _declineOfferButton = new Button(_ui);
        _declineOfferButton.OverrideStyles.Icon = new IconTexture
        {
            TextureId = "Textures/Buttons/28.png",
            SourceRect = new IguinaRect { Width = 32, Height = 32 }
        };
        _declineOfferButton.Size.SetPixels(32, 32);
        _declineOfferButton.Anchor = Anchor.TopLeft;
        _declineOfferButton.Offset.SetPixels(191, 268);
        _declineOfferButton.Paragraph.Text = string.Empty;
        _declineOfferButton.Visible = false;
        _declineOfferButton.Events.OnClick += _ => OnDeclineOfferPressed();
        _panel.AddChild(_declineOfferButton);

        _offerDisabledPanel = new Panel(_ui);
        _offerDisabledPanel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/22.png",
            SourceRect = new IguinaRect { Width = 32, Height = 32 }
        };
        _offerDisabledPanel.Size.SetPixels(32, 32);
        _offerDisabledPanel.Anchor = Anchor.TopLeft;
        _offerDisabledPanel.Offset.SetPixels(6, 268);
        _offerDisabledPanel.Visible = false;
        _panel.AddChild(_offerDisabledPanel);

        _ownGrid = new IguinaSlotGrid(_ui, 5, 6, 32, 4, 7, 50, _panel);
        _ownGrid.SlotRender += OnRenderOwnSlot;
        _ownGrid.SlotLeftClick += OnGridMouseLeftClick;
        _ownGrid.SlotRightClick += OnGridMouseRightClick;

        _theirGrid = new IguinaSlotGrid(_ui, 5, 6, 32, 4, 192, 50, _panel);
        _theirGrid.SlotRender += OnRenderTheirSlot;
    }

    public static bool PanelVisible
    {
        get => _instance?._panel?.Visible ?? false;
        set { if (_instance?._panel != null) _instance._panel.Visible = value; }
    }

    public static bool ConfirmOfferButtonVisible
    {
        get => _instance?._confirmOfferButton?.Visible ?? false;
        set { if (_instance?._confirmOfferButton != null) _instance._confirmOfferButton.Visible = value; }
    }

    public static bool AcceptOfferButtonVisible
    {
        get => _instance?._acceptOfferButton?.Visible ?? false;
        set { if (_instance?._acceptOfferButton != null) _instance._acceptOfferButton.Visible = value; }
    }

    public static bool DeclineOfferButtonVisible
    {
        get => _instance?._declineOfferButton?.Visible ?? false;
        set { if (_instance?._declineOfferButton != null) _instance._declineOfferButton.Visible = value; }
    }

    public static bool OfferDisabledPanelVisible
    {
        get => _instance?._offerDisabledPanel?.Visible ?? false;
        set { if (_instance?._offerDisabledPanel != null) _instance._offerDisabledPanel.Visible = value; }
    }

    private void OnRenderOwnSlot(int slot)
    {
        if (_context.LocalPlayer.Entity == Arch.Core.Entity.Null) return;
        var rect = _ownGrid!.GetSlotRect(slot);
        _itemRenderer.DrawItem(_catalog.Items.Get(_context.LocalPlayer.GetTrade().Offer[slot]?.ItemId ?? Guid.Empty), _context.LocalPlayer.GetTrade().Offer[slot]?.Amount ?? 0, new DrawingPoint(rect.X, rect.Y));
    }

    private void OnRenderTheirSlot(int slot)
    {
        if (_context.LocalPlayer.Entity == Arch.Core.Entity.Null) return;
        var rect = _theirGrid!.GetSlotRect(slot);
        _itemRenderer.DrawItem(_catalog.Items.Get(_context.LocalPlayer.GetTrade().TheirOffer[slot]?.ItemId ?? Guid.Empty), _context.LocalPlayer.GetTrade().TheirOffer[slot]?.Amount ?? 0, new DrawingPoint(rect.X, rect.Y));
    }

    private void OnGridMouseLeftClick(int slot)
    {
        if (!PanelVisible) return;
        if (_context.LocalPlayer.GetTrade().Offer[slot]?.ItemId == Guid.Empty) return;
        _tradeSender.TradeOffer((short)slot, 0);
    }

    private void OnGridMouseRightClick(int slot)
    {
        if (GameScreen.InventoryChange <= 0) return;

        if (_context.LocalPlayer.GetInventory().Slots[GameScreen.InventoryChange]?.Amount == 1)
            _tradeSender.TradeOffer((short)slot, GameScreen.InventoryChange);
        else
        {
            OwnSlot = (short)slot;
            InventorySlot = GameScreen.InventoryChange;
            TradeAmountView.AmountText = string.Empty;
            TradeAmountView.PanelVisible = true;
        }
    }

    private void OnClosePressed()
    {
        _tradeSender.TradeLeave();
        _panel!.Visible = false;
    }

    private void OnAcceptOfferPressed()
    {
        ConfirmOfferButtonVisible = true;
        AcceptOfferButtonVisible = DeclineOfferButtonVisible = false;
        OfferDisabledPanelVisible = false;
        _tradeSender.TradeOfferState(TradeStatus.Accepted);

        ref var trade = ref _context.LocalPlayer.GetTrade();
        trade.Offer = new ItemSlot?[MaxInventory];
        trade.TheirOffer = new ItemSlot?[MaxInventory];
    }

    private void OnDeclineOfferPressed()
    {
        ConfirmOfferButtonVisible = true;
        AcceptOfferButtonVisible = DeclineOfferButtonVisible = false;
        OfferDisabledPanelVisible = false;
        _tradeSender.TradeOfferState(TradeStatus.Declined);
    }

    private void OnConfirmOfferPressed()
    {
        ConfirmOfferButtonVisible = AcceptOfferButtonVisible = DeclineOfferButtonVisible = false;
        OfferDisabledPanelVisible = true;
        _tradeSender.TradeOfferState(TradeStatus.Confirmed);
    }
}
