using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using System.Drawing;

namespace CryBits.Client.UI.Game.Views;

internal class DraggableSlotView(ItemRenderer itemRenderer, InputManager inputManager, GameContext context, DefinitionCatalog catalog) : IView
{
    private readonly DefinitionCatalog _catalog = catalog;
    private static Picture DraggableSlotPicture => Tools.Pictures["DraggableSlot"];

    public void Bind() => DraggableSlotPicture.OnRender += OnRender;

    public void Unbind() => DraggableSlotPicture.OnRender -= OnRender;

    private void OnRender(Point _)
    {
        var pos = new Point(
            inputManager.MousePosition.X + 6,
            inputManager.MousePosition.Y + 6
        );

        if (GameScreen.HotbarChange >= 0)
        {
            var hotbar = context.LocalPlayer.GetHotbar();
            var inv = context.LocalPlayer.GetInventory();
            if (hotbar == null || inv == null) return;
            var hotbarSlot = hotbar.Slots[GameScreen.HotbarChange];
            if (hotbarSlot is HotbarSlot { Type: SlotType.Item } h)
            {
                var itemId = inv.Slots[h.Slot].ItemId;
                if (_catalog.Items.Get(itemId) is { } item) itemRenderer.DrawItem(item, 1, pos);
            }
        }
        else if (GameScreen.InventoryChange > 0)
        {
            var inv = context.LocalPlayer.GetInventory();
            if (inv == null) return;
            var itemId = inv.Slots[GameScreen.InventoryChange].ItemId;
            if (_catalog.Items.Get(itemId) is { } item) itemRenderer.DrawItem(item, 1, pos);
        }
    }
}
