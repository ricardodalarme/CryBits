using System.Linq;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Entities;

namespace CryBits.Client.UI.Menu.Views;

internal class CreateCharacterView(NetworkClient networkClient, AccountSender accountSender) : IView
{
    internal static Panel CreateCharacterPanel => Tools.Panels["CreateCharacter"];
    internal static TextBox NameTextBox => Tools.TextBoxes["CreateCharacter_Name"];
    private static Button CreateButton => Tools.Buttons["CreateCharacter"];
    private static Button ChangeClassRightButton => Tools.Buttons["CreateCharacter_ChangeRight"];
    private static Button ChangeClassLeftButton => Tools.Buttons["CreateCharacter_ChangeLeft"];
    private static Button TextureChangeLeftButton => Tools.Buttons["CreateCharacter_Texture_ChangeLeft"];
    private static Button TextureChangeRightButton => Tools.Buttons["CreateCharacter_Texture_ChangeRight"];
    internal static CheckBox GenderMaleCheckBox => Tools.CheckBoxes["GenderMale"];
    internal static CheckBox GenderFemaleCheckBox => Tools.CheckBoxes["GenderFemale"];
    private static Button BackButton => Tools.Buttons["CreateCharacter_Back"];

    // State
    public static byte CurrentClass;
    public static byte CurrentTexture;

    public void Bind()
    {
        CreateButton.OnMouseUp += OnCreatePressed;
        ChangeClassRightButton.OnMouseUp += OnChangeClassRightPressed;
        ChangeClassLeftButton.OnMouseUp += OnChangeClassLeftPressed;
        TextureChangeLeftButton.OnMouseUp += OnChangeTextureLeftPressed;
        TextureChangeRightButton.OnMouseUp += OnChangeTextureRight;
        GenderMaleCheckBox.OnMouseUp += OnGenderMaleChanged;
        GenderFemaleCheckBox.OnMouseUp += OnGenderFemaleChanged;
        BackButton.OnMouseUp += OnBackPressed;
    }

    public void Unbind()
    {
        CreateButton.OnMouseUp -= OnCreatePressed;
        ChangeClassRightButton.OnMouseUp -= OnChangeClassRightPressed;
        ChangeClassLeftButton.OnMouseUp -= OnChangeClassLeftPressed;
        TextureChangeLeftButton.OnMouseUp -= OnChangeTextureLeftPressed;
        TextureChangeRightButton.OnMouseUp -= OnChangeTextureRight;
        GenderMaleCheckBox.OnMouseUp -= OnGenderMaleChanged;
        GenderFemaleCheckBox.OnMouseUp -= OnGenderFemaleChanged;
        BackButton.OnMouseUp -= OnBackPressed;
    }

    private void OnCreatePressed()
    {
        // Open character creation
        if (networkClient.TryConnect())
            accountSender.CreateCharacter(
                name: NameTextBox.Text,
                isMale: GenderMaleCheckBox.Checked,
                @class: CurrentClass,
                textureNum: CurrentTexture
            );
    }

    private void OnChangeClassRightPressed()
    {
        // Cycle selected class to the right
        if (CurrentClass == Class.List.Count - 1)
            CurrentClass = 0;
        else
            CurrentClass++;
    }

    private void OnChangeClassLeftPressed()
    {
        // Cycle selected class to the left
        if (CurrentClass == 0)
            CurrentClass = (byte)Class.List.Count;
        else
            CurrentClass--;
    }

    private void OnChangeTextureRight()
    {
        var @class = Class.List.ElementAt(CurrentClass).Value;
        var texList = GenderMaleCheckBox.Checked ? @class.TextureMale : @class.TextureFemale;

        if (CurrentTexture == texList.Count - 1)
            CurrentTexture = 0;
        else
            CurrentTexture++;
    }

    private void OnChangeTextureLeftPressed()
    {
        var @class = Class.List.ElementAt(CurrentClass).Value;
        var texList = GenderMaleCheckBox.Checked ? @class.TextureMale : @class.TextureFemale;

        if (CurrentTexture == 0)
            CurrentTexture = (byte)(texList.Count - 1);
        else
            CurrentTexture--;
    }

    private void OnGenderMaleChanged()
    {
        GenderFemaleCheckBox.Checked = !GenderMaleCheckBox.Checked;
        CurrentTexture = 0;
    }

    private void OnGenderFemaleChanged()
    {
        GenderMaleCheckBox.Checked = !GenderFemaleCheckBox.Checked;
        CurrentTexture = 0;
    }

    private void OnBackPressed()
    {
        // Open character panel
        MenuScreen.CloseMenus();
        SelectCharacterView.SelectCharacterPanel.Visible = true;
    }
}
