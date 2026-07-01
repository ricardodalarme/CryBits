using Iguina.Entities;

namespace CryBits.Client.UI.Game.Views;

internal class MenusView(IguinaContext uiContext) : ViewBase
{
    private Button CharacterButton => uiContext.Get<Button>("MenuCharacter");
    private Button InventoryButton => uiContext.Get<Button>("MenuInventory");
    private Button OptionsButton => uiContext.Get<Button>("MenuOptions");

    public override void Bind()
    {
        CharacterButton.Events.OnClick += OnCharacterPressed;
        InventoryButton.Events.OnClick += OnInventoryPressed;
        OptionsButton.Events.OnClick += OnOptionsPressed;
    }

    public override void Unbind()
    {
        CharacterButton.Events.OnClick -= OnCharacterPressed;
        InventoryButton.Events.OnClick -= OnInventoryPressed;
        OptionsButton.Events.OnClick -= OnOptionsPressed;
    }

    private void OnCharacterPressed(Entity _)
    {
        uiContext.Registry["CharacterPanel"].Visible = !uiContext.Registry["CharacterPanel"].Visible;
        uiContext.Registry["InventoryPanel"].Visible = false;
        uiContext.Registry["OptionsPanel"].Visible = false;
    }

    private void OnInventoryPressed(Entity _)
    {
        uiContext.Registry["InventoryPanel"].Visible = !uiContext.Registry["InventoryPanel"].Visible;
        uiContext.Registry["CharacterPanel"].Visible = false;
        uiContext.Registry["OptionsPanel"].Visible = false;
    }

    private void OnOptionsPressed(Entity _)
    {
        uiContext.Registry["OptionsPanel"].Visible = !uiContext.Registry["OptionsPanel"].Visible;
        uiContext.Registry["CharacterPanel"].Visible = false;
        uiContext.Registry["InventoryPanel"].Visible = false;
    }
}
