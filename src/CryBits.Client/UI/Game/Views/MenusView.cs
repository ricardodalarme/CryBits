using Myra.Events;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Game.Views;

internal class MenusView(UiContext uiContext) : ViewBase
{
    private Button CharacterButton => uiContext.Get<Button>("MenuCharacter");
    private Button InventoryButton => uiContext.Get<Button>("MenuInventory");
    private Button OptionsButton => uiContext.Get<Button>("MenuOptions");

    public override void Bind()
    {
        CharacterButton.Click += OnCharacterPressed;
        InventoryButton.Click += OnInventoryPressed;
        OptionsButton.Click += OnOptionsPressed;
    }

    public override void Unbind()
    {
        CharacterButton.Click -= OnCharacterPressed;
        InventoryButton.Click -= OnInventoryPressed;
        OptionsButton.Click -= OnOptionsPressed;
    }

    private void OnCharacterPressed(object? sender, MyraEventArgs e)
    {
        uiContext.Registry["CharacterPanel"].Visible = !uiContext.Registry["CharacterPanel"].Visible;
        uiContext.Registry["InventoryPanel"].Visible = false;
        uiContext.Registry["OptionsPanel"].Visible = false;
    }

    private void OnInventoryPressed(object? sender, MyraEventArgs e)
    {
        uiContext.Registry["InventoryPanel"].Visible = !uiContext.Registry["InventoryPanel"].Visible;
        uiContext.Registry["CharacterPanel"].Visible = false;
        uiContext.Registry["OptionsPanel"].Visible = false;
    }

    private void OnOptionsPressed(object? sender, MyraEventArgs e)
    {
        uiContext.Registry["OptionsPanel"].Visible = !uiContext.Registry["OptionsPanel"].Visible;
        uiContext.Registry["CharacterPanel"].Visible = false;
        uiContext.Registry["InventoryPanel"].Visible = false;
    }
}
