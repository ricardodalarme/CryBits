using CryBits.Client.Input;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Items;

namespace CryBits.Client.UI.Game.Views;

internal class DraggableSlotView(
    UiContext uiContext,
    ItemIconRenderer itemRenderer,
    InputManager inputManager,
    GameScreen gameScreen,
    InventoryViewModel inventoryViewModel,
    HotbarViewModel hotbarViewModel) : ViewBase
{
    public override void Bind() => uiContext.PostDraw += OnPostDraw;

    public override void Unbind() => uiContext.PostDraw -= OnPostDraw;

    private void OnPostDraw()
    {
        var pos = new System.Drawing.Point(
             inputManager.MousePosition.X + 6,
             inputManager.MousePosition.Y + 6
         );

        if (gameScreen.HotbarChange is { } hotSlot)
        {
            var hbSlot = hotbarViewModel.Slots[hotSlot];
            if (hbSlot is { Slot: > 0, Type: SlotType.Item } && hbSlot.Definition is { } item) itemRenderer.DrawItem(item, 1, pos);
        }
        else if (gameScreen.InventoryChange is { } invSlot)
        {
            var itemVM = inventoryViewModel.Slots[invSlot];
            if (itemVM is { Definition: { } item }) itemRenderer.DrawItem(item, 1, pos);
        }
    }
}
