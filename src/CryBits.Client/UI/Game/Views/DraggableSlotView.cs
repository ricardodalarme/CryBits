using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Managers;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;

namespace CryBits.Client.UI.Game.Views;

internal class DraggableSlotView(IguinaContext uiContext, ItemRenderer itemRenderer, InputManager inputManager, GameContext context, DefinitionCatalog catalog) : ViewBase
{
    public override void Bind() => uiContext.PostDraw += OnPostDraw;

    public override void Unbind() => uiContext.PostDraw -= OnPostDraw;

    private void OnPostDraw()
    {
        var pos = new System.Drawing.Point(
             inputManager.MousePosition.X + 6,
             inputManager.MousePosition.Y + 6
         );

        if (GameScreen.HotbarChange is { } hotSlot)
        {
            var hotbar = context.LocalPlayer.GetHotbar();
            var inv = context.LocalPlayer.GetInventory();
            if (hotbar == null || inv == null) return;
            var hotbarSlot = hotbar.Slots[hotSlot];
            if (hotbarSlot is HotbarSlot { Type: SlotType.Item } h)
            {
                var itemId = inv.Slots[h.Slot].ItemId;
                if (catalog.Items.Get(itemId) is { } item) itemRenderer.DrawItem(item, 1, pos);
            }
        }
        else if (GameScreen.InventoryChange is { } invSlot)
        {
            var inv = context.LocalPlayer.GetInventory();
            if (inv == null) return;
            var itemId = inv.Slots[invSlot].ItemId;
            if (catalog.Items.Get(itemId) is { } item) itemRenderer.DrawItem(item, 1, pos);
        }
    }
}
