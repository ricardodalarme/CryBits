using Ent = global::Iguina.Entities.Entity;

namespace CryBits.Client.UI.Game.Views;

internal sealed class MenusView
{
    private CharacterView? _characterView;
    private InventoryView? _inventoryView;
    private OptionsView? _optionsView;

    public void Wire(Dictionary<string, Ent> reg) { }

    public void SetPanelToggles(CharacterView cv, InventoryView iv, OptionsView ov)
    {
        _characterView = cv;
        _inventoryView = iv;
        _optionsView = ov;
    }
}
