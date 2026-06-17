using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using DrawingPoint = System.Drawing.Point;
using Ent = global::Iguina.Entities.Entity;

namespace CryBits.Client.UI.Game.Views;

internal sealed class DraggableSlotView
{
    private readonly ItemRenderer _itemRenderer;
    private readonly InputManager _inputManager;
    private readonly GameContext _context;
    private readonly DefinitionCatalog _catalog;
    private readonly UISystem _ui;

    private Panel? _dragPanel;

    public DraggableSlotView(UISystem ui, ItemRenderer itemRenderer, InputManager inputManager, GameContext context, DefinitionCatalog catalog)
    {
        _ui = ui;
        _itemRenderer = itemRenderer;
        _inputManager = inputManager;
        _context = context;
        _catalog = catalog;
    }

    public void Wire(System.Collections.Generic.Dictionary<string, global::Iguina.Entities.Entity> reg) { }
    public void Build(Panel root)
    {
        _dragPanel = new Panel(_ui);
        _dragPanel.Size.SetPixels(32, 32);
        _dragPanel.Anchor = Anchor.TopLeft;
        _dragPanel.Offset.SetPixels(0, 0);
        _dragPanel.Events.AfterDraw += OnRender;
        root.AddChild(_dragPanel);
    }

    private void OnRender(Ent _)
    {
        var pos = new DrawingPoint(
            _inputManager.MousePosition.X + 6,
            _inputManager.MousePosition.Y + 6
        );

        _dragPanel!.Offset.SetPixels(pos.X, pos.Y);

        if (GameScreen.HotbarChange >= 0)
        {
            var hotbarSlot = _context.LocalPlayer.GetHotbar().Slots[GameScreen.HotbarChange];
            if (hotbarSlot is HotbarSlot { Type: SlotType.Item } h)
                _itemRenderer.DrawItem(_catalog.Items.Get(_context.LocalPlayer.GetInventory().Slots[h.Slot] is ItemSlot s ? s.ItemId : Guid.Empty), 1, pos);
        }
        else if (GameScreen.InventoryChange > 0)
        {
            _itemRenderer.DrawItem(_catalog.Items.Get(_context.LocalPlayer.GetInventory().Slots[GameScreen.InventoryChange] is ItemSlot s ? s.ItemId : Guid.Empty), 1, pos);
        }
    }
}
