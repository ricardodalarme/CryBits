using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;

namespace CryBits.Client.UI.Game.Views;

internal class MenusView : IView
{
    private static Button CharacterButton => Tools.Buttons["Menu_Character"];
    private static Button InventoryButton => Tools.Buttons["Menu_Inventory"];
    private static Button OptionsButton => Tools.Buttons["Menu_Options"];

    public void Bind()
    {
        CharacterButton.OnMouseUp += OnCharacterPressed;
        InventoryButton.OnMouseUp += OnInventoryPressed;
        OptionsButton.OnMouseUp += OnOptionsPressed;
    }

    public void Unbind()
    {
        CharacterButton.OnMouseUp -= OnCharacterPressed;
        InventoryButton.OnMouseUp -= OnInventoryPressed;
        OptionsButton.OnMouseUp -= OnOptionsPressed;
    }

    private void OnCharacterPressed()
    {
        CharacterView.Panel.Visible = !CharacterView.Panel.Visible;
        InventoryView.Panel.Visible = false;
        OptionsView.Panel.Visible = false;
    }

    private void OnInventoryPressed()
    {
        InventoryView.Panel.Visible = !InventoryView.Panel.Visible;
        CharacterView.Panel.Visible = false;
        OptionsView.Panel.Visible = false;
    }

    private void OnOptionsPressed()
    {
        OptionsView.Panel.Visible = !OptionsView.Panel.Visible;
        CharacterView.Panel.Visible = false;
        InventoryView.Panel.Visible = false;
    }
}
