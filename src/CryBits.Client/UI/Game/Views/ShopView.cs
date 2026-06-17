using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Shops;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using DrawingPoint = System.Drawing.Point;
using IguinaRect = Iguina.Defs.Rectangle;

namespace CryBits.Client.UI.Game.Views;

internal sealed class ShopView
{
    private static ShopView? _instance;
    private readonly ShopSender _shopSender;
    private readonly ItemRenderer _itemRenderer;
    private readonly DefinitionCatalog _catalog;
    private readonly UISystem _ui;

    private Panel? _panel;
    private Button? _closeButton;
    private Label? _nameLabel;
    private Label? _currencyLabel;
    private IguinaSlotGrid? _grid;

    private const int PanelOffsetX = 269;
    private const int PanelOffsetY = 193;
    public static Shop? OpenedShop;

    public ShopView(UISystem ui, ItemRenderer itemRenderer, DefinitionCatalog catalog)
    {
        _instance = this;
        _ui = ui;
        _itemRenderer = itemRenderer;
        _catalog = catalog;
        _shopSender = ShopSender.Instance;
    }

    public void Wire(System.Collections.Generic.Dictionary<string, global::Iguina.Entities.Entity> reg) { }
    public void Build(Panel root)
    {
        _panel = new Panel(_ui);
        _panel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/21.png",
            SourceRect = new IguinaRect { Width = 280, Height = 260 }
        };
        _panel.Size.SetPixels(280, 260);
        _panel.Anchor = Anchor.TopLeft;
        _panel.Offset.SetPixels(269, 193);
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
        _closeButton.Offset.SetPixels(245, 9);
        _closeButton.Paragraph.Text = string.Empty;
        _closeButton.Events.OnClick += _ => OnClosePressed();
        _panel.AddChild(_closeButton);

        _nameLabel = new Label(_ui);
        _nameLabel.Anchor = Anchor.TopLeft;
        _nameLabel.Offset.SetPixels(131, 28);
        _panel.AddChild(_nameLabel);

        _currencyLabel = new Label(_ui);
        _currencyLabel.Anchor = Anchor.TopLeft;
        _currencyLabel.Offset.SetPixels(10, 195);
        _panel.AddChild(_currencyLabel);

        _grid = new IguinaSlotGrid(_ui, 7, 4, 32, 4, 7, 50, _panel);
        _grid.SlotRender += OnRenderSlot;
        _grid.SlotDoubleClick += OnGridMouseDoubleClick;
        _grid.SlotHover += OnGridSlotHover;
        _grid.SlotLeave += _ => InformationView.Hide();
    }

    public static bool PanelVisible
    {
        get => _instance?._panel?.Visible ?? false;
        set { if (_instance?._panel != null) _instance._panel.Visible = value; }
    }

    private void OnRenderSlot(int slot)
    {
        if (OpenedShop == null || slot >= OpenedShop.Sold.Count) return;
        var item = _catalog.Items.Get(OpenedShop.Sold[slot].ItemId);
        var rect = _grid!.GetSlotRect(slot);
        _itemRenderer.DrawItem(item, OpenedShop.Sold[slot].Amount, new DrawingPoint(rect.X, rect.Y));
    }

    private void OnGridMouseDoubleClick(int slot)
    {
        if (OpenedShop == null) return;
        _shopSender.ShopBuy((byte)slot);
    }

    private void OnClosePressed()
    {
        InformationView.Hide();
        _panel!.Visible = false;
        _shopSender.ShopClose();
    }

    private void OnGridSlotHover(int slot)
    {
        if (OpenedShop == null || slot >= OpenedShop.Sold.Count) return;
        var item = _catalog.Items.Get(OpenedShop.Sold[slot].ItemId);
        if (item == null) return;
        InformationView.Show(item.Id,
            new DrawingPoint(PanelOffsetX - 186, PanelOffsetY + 5),
            "Price: " + OpenedShop.Sold[slot].Price);
    }

    public static void Open(Shop shop)
    {
        if (_instance == null || shop == null) return;
        OpenedShop = shop;
        _instance._nameLabel!.Text = shop.Name;
        _instance._currencyLabel!.Text = "Currency: " + (DefinitionCatalog.Instance.Items.Get(shop.CurrencyId)?.Name ?? "Unknown");
        _instance._panel!.Visible = true;
    }
}
