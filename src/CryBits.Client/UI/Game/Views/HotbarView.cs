using CryBits.Client.Components.Hotbar;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using Ent = global::Iguina.Entities.Entity;
using IguinaRect = Iguina.Defs.Rectangle;

namespace CryBits.Client.UI.Game.Views;

internal sealed class HotbarView
{
    private readonly UISystem _ui;
    private readonly GameContext _context;
    private readonly DefinitionCatalog _catalog;
    private readonly ItemRenderer _itemRenderer;
    private IguinaSlotGrid? _grid;

    public HotbarView(UISystem ui, GameContext context, DefinitionCatalog catalog, ItemRenderer itemRenderer)
    {
        _ui = ui;
        _context = context;
        _catalog = catalog;
        _itemRenderer = itemRenderer;
    }

    public void Wire(System.Collections.Generic.Dictionary<string, global::Iguina.Entities.Entity> reg) { }
    public void Build(Ent root)
    {
        var panel = new Panel(_ui);
        panel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/9.png",
            SourceRect = new IguinaRect { Width = 374, Height = 38 }
        };
        panel.Size.SetPixels(374, 38);
        panel.Anchor = Anchor.TopLeft;
        panel.Offset.SetPixels(417, 8);
        root.AddChild(panel);

        _grid = new IguinaSlotGrid(_ui, 10, 1, 32, 4, 8, 6, panel);

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
        if (!_context.World.TryGet<HotbarComponent>(_context.LocalPlayer.Entity, out var hotbar)) return;

        var hotbarSlot = hotbar.Slots[slot];
        if (hotbarSlot is HotbarSlot { Slot: > 0, Type: SlotType.Item } h)
        {
            var item = _catalog.Items.Get(_context.LocalPlayer.GetInventory().Slots[h.Slot] is ItemSlot s ? s.ItemId : Guid.Empty);
            if (item != null)
            {
                var rect = _grid!.GetSlotRect(slot);
                _itemRenderer.DrawItem(item, 1, new System.Drawing.Point(rect.X, rect.Y));
            }
        }
    }

    private void OnSlotLeftClick(int slot)
    {
        var hotbarSlot = _context.LocalPlayer.GetHotbar().Slots[slot];
        if (hotbarSlot is not HotbarSlot { Slot: not 0 }) return;
        CryBits.Client.UI.Game.GameScreen.HotbarChange = (short)slot;
    }

    private void OnSlotRightClick(int slot)
    {
        var hotbarSlot = _context.LocalPlayer.GetHotbar().Slots[slot];
        if (hotbarSlot is not HotbarSlot { Slot: not 0 }) return;
        PlayerSender.Instance.HotbarAdd((short)slot, 0, 0);
    }

    private void OnSlotDoubleClick(int slot)
    {
        var hotbarSlot = _context.LocalPlayer.GetHotbar().Slots[slot];
        if (hotbarSlot is not HotbarSlot { Slot: > 0 }) return;
        PlayerSender.Instance.HotbarUse((byte)slot);
        UI.Game.Views.DropItemView.PanelVisible = false;
    }

    private void OnSlotHover(int slot)
    {
        var hotbarSlot = _context.LocalPlayer.GetHotbar().Slots[slot];
        if (hotbarSlot is HotbarSlot { Slot: > 0, Type: SlotType.Item } h)
        {
            var item = _catalog.Items.Get(_context.LocalPlayer.GetInventory().Slots[h.Slot] is ItemSlot s ? s.ItemId : Guid.Empty);
            if (item == null) return;
            UI.Game.Views.InformationView.Show(item.Id, System.Drawing.Point.Empty);
        }
    }
}
