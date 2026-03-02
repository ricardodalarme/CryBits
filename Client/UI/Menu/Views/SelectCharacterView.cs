using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;

namespace CryBits.Client.UI.Menu.Views;

internal class SelectCharacterView(AccountSender accountSender) : IView
{
    internal static Panel SelectCharacterPanel => Tools.Panels["SelectCharacter"];
    internal static Button UseButton => Tools.Buttons["Character_Use"];
    internal static Button CreateButton => Tools.Buttons["Character_Create"];
    internal static Button DeleteButton => Tools.Buttons["Character_Delete"];
    internal static Button ChangeRightButton => Tools.Buttons["Character_ChangeRight"];
    private static Button ChangeLeftButton => Tools.Buttons["Character_ChangeLeft"];

    public static TempCharacter[] Characters;
    public static int CurrentCharacter = 1;

    public struct TempCharacter
    {
        public string Name;
        public short TextureNum;
    }

    public void Bind()
    {
        UseButton.OnMouseUp += OnUsePressed;
        CreateButton.OnMouseUp += OnCreatePressed;
        DeleteButton.OnMouseUp += OnDeletePressed;
        ChangeRightButton.OnMouseUp += OnChangeRightPressed;
        ChangeLeftButton.OnMouseUp += OnChangeLeftPressed;
    }

    public void Unbind()
    {
        UseButton.OnMouseUp -= OnUsePressed;
        CreateButton.OnMouseUp -= OnCreatePressed;
        DeleteButton.OnMouseUp -= OnDeletePressed;
        ChangeRightButton.OnMouseUp -= OnChangeRightPressed;
        ChangeLeftButton.OnMouseUp -= OnChangeLeftPressed;
    }

    private void OnUsePressed()
    {
        accountSender.CharacterUse(CurrentCharacter);
    }

    private void OnDeletePressed()
    {
        accountSender.CharacterDelete(CurrentCharacter);
    }

    private void OnCreatePressed()
    {
        accountSender.CharacterCreate();
    }

    private void OnChangeRightPressed()
    {
        if (CurrentCharacter == Characters.Length - 1)
            CurrentCharacter = 0;
        else
            CurrentCharacter++;
    }

    private void OnChangeLeftPressed()
    {
        if (CurrentCharacter == 0)
            CurrentCharacter = Characters.Length;
        else
            CurrentCharacter--;
    }

    public static bool UpdateButtonVisibility()
    {
        var visibility = Characters != null && CurrentCharacter < Characters.Length;
        CreateButton.Visible = !visibility;
        DeleteButton.Visible = visibility;
        UseButton.Visible = visibility;
        return visibility;
    }
}
