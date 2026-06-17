using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal sealed class InventoryView
{
    private readonly UISystem _ui;
    private readonly GameContext _context;
    private readonly DefinitionCatalog _catalog;
    private readonly ItemRenderer _itemRenderer;
    private Panel? _panel;
    private IguinaSlotGrid? _grid;

    public bool IsVisible => _panel?.Visible ?? false;
    public void SetVisible(bool visible) { if (_panel != null) _panel.Visible = visible; }

    public InventoryView(UISystem ui, GameContext context, DefinitionCatalog catalog, ItemRenderer itemRenderer)
    {
        _ui = ui;
        _context = context;
        _catalog = catalog;
        _itemRenderer = itemRenderer;
    }

    public void Wire(System.Collections.Generic.Dictionary<string, global::Iguina.Entities.Entity> reg) { }
    public void Build(Panel root)
    {
        // Inventory panel #12: 190x248 at (596, 310)
        _panel = new Panel(_ui);
        _panel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/12.png",
            SourceRect = new Rectangle { Width = 190, Height = 248 }
        };
        _panel.Size.SetPixels(190, 248);
        _panel.Anchor = Anchor.TopLeft;
        _panel.Offset.SetPixels(596, 310);
        _panel.Visible = false;
        root.AddChild(_panel);

        // Grid: 5 columns x 6 rows, 32x32, padding 4 at (603, 340) → relative (7, 30)
        _grid = new IguinaSlotGrid(_ui, 5, 6, 32, 4, 7, 30, _panel);

        _grid.SlotRender += OnSlotRender;
        _grid.SlotLeftClick += OnSlotLeftClick;
        _grid.SlotRightClick += OnSlotRightClick;
        _grid.SlotDoubleClick += OnSlotDoubleClick;
        _grid.SlotHover += OnSlotHover;
        _grid.SlotLeave += _ => UI.Game.Views.InformationView.Hide();
    }

    private void OnSlotRender(int slot)
    {
        if (_context.LocalPlayer.Entity == Arch.Core.Entity.Null) return;
        ref var inv = ref _context.LocalPlayer.GetInventory();
        var slotData = inv.Slots[slot];
        if (slotData == null) return;
        var item = _catalog.Items.Get(slotData.Value.ItemId);
        if (item != null)
        {
            var rect = _grid!.GetSlotRect(slot);
            _itemRenderer.DrawItem(item, slotData.Value.Amount, new System.Drawing.Point(rect.X, rect.Y));
        }
    }

    private void OnSlotLeftClick(int slot)
    {
        ref var inv = ref _context.LocalPlayer.GetInventory();
        if (inv.Slots[slot] == null) return;
        CryBits.Client.UI.Game.GameScreen.InventoryChange = (short)slot;
    }

    private void OnSlotRightClick(int slot)
    {
        ref var inv = ref _context.LocalPlayer.GetInventory();
        if (inv.Slots[slot] == null) return;

        var item = _catalog.Items.Get(inv.Slots[slot].Value.ItemId);
        if (item?.Bind != BindOn.Pickup)
            PlayerSender.Instance.DropItem((byte)slot, 1);
    }

    private void OnSlotDoubleClick(int slot)
    {
        ref var inv = ref _context.LocalPlayer.GetInventory();
        if (inv.Slots[slot] == null) return;

        var item = _catalog.Items.Get(inv.Slots[slot].Value.ItemId);
        if (item?.Bind == BindOn.Equip)
            PlayerSender.Instance.InventoryUse((byte)slot);
    }

    private void OnSlotHover(int slot)
    {
        if (_context.LocalPlayer.Entity == Arch.Core.Entity.Null) return;
        ref var inv = ref _context.LocalPlayer.GetInventory();
        if (inv.Slots[slot] == null) return;
        UI.Game.Views.InformationView.Show(inv.Slots[slot].Value.ItemId, System.Drawing.Point.Empty);
    }
}
