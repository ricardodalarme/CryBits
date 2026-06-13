using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using System.Drawing;
using System.Linq;
using static CryBits.Globals;

namespace CryBits.Client.UI.Menu.Views;

internal class CreateCharacterView(AccountSender accountSender, CharacterRenderer characterRenderer, DefinitionCatalog catalog) : IView
{
    private readonly DefinitionCatalog _catalog = catalog;
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
    private static Picture FacePicture => Tools.Pictures["CreateCharacter_Face"];
    private static Picture SpritePicture => Tools.Pictures["CreateCharacter_Sprite"];
    private static Label ClassNameLabel => Tools.Labels["CreateCharacter_ClassName"];
    private static Label ClassDescLabel => Tools.Labels["CreateCharacter_ClassDescription"];

    // State
    public static byte CurrentClass;
    public static byte CurrentTexture;

    public void Bind()
    {
        FacePicture.OnRender += OnRenderFace;
        SpritePicture.OnRender += OnRenderSprite;
        CreateButton.OnMouseUp += OnCreatePressed;
        ChangeClassRightButton.OnMouseUp += OnChangeClassRightPressed;
        ChangeClassLeftButton.OnMouseUp += OnChangeClassLeftPressed;
        TextureChangeLeftButton.OnMouseUp += OnChangeTextureLeftPressed;
        TextureChangeRightButton.OnMouseUp += OnChangeTextureRight;
        GenderMaleCheckBox.OnMouseUp += OnGenderMaleChanged;
        GenderFemaleCheckBox.OnMouseUp += OnGenderFemaleChanged;
        BackButton.OnMouseUp += OnBackPressed;
        UpdateClassLabels(_catalog);
    }

    public void Unbind()
    {
        FacePicture.OnRender -= OnRenderFace;
        SpritePicture.OnRender -= OnRenderSprite;
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
        if (CurrentClass == _catalog.Classes.Count - 1)
            CurrentClass = 0;
        else
            CurrentClass++;
        UpdateClassLabels(_catalog);
    }

    private void OnChangeClassLeftPressed()
    {
        // Cycle selected class to the left
        if (CurrentClass == 0)
            CurrentClass = (byte)_catalog.Classes.Count;
        else
            CurrentClass--;
        UpdateClassLabels(_catalog);
    }

    private void OnChangeTextureRight()
    {
        var @class = _catalog.Classes.ElementAt(CurrentClass).Value;
        var texList = GenderMaleCheckBox.Checked ? @class.TextureMale : @class.TextureFemale;

        if (CurrentTexture == texList.Count - 1)
            CurrentTexture = 0;
        else
            CurrentTexture++;
    }

    private void OnChangeTextureLeftPressed()
    {
        var @class = _catalog.Classes.ElementAt(CurrentClass).Value;
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
        UpdateClassLabels(_catalog);
    }

    private void OnGenderFemaleChanged()
    {
        GenderMaleCheckBox.Checked = !GenderFemaleCheckBox.Checked;
        CurrentTexture = 0;
        UpdateClassLabels(_catalog);
    }

    private void OnBackPressed()
    {
        // Open character panel
        MenuScreen.CloseMenus();
        SelectCharacterView.SelectCharacterPanel.Visible = true;
    }

    private short GetCurrentTextureNum()
    {
        if (_catalog.Classes.Count == 0) return 0;
        var @class = _catalog.Classes.ElementAt(CurrentClass).Value;
        if (GenderMaleCheckBox.Checked && @class.TextureMale.Count > 0)
            return @class.TextureMale[CurrentTexture];
        if (@class.TextureFemale.Count > 0)
            return @class.TextureFemale[CurrentTexture];
        return 0;
    }

    internal static void UpdateClassLabels(DefinitionCatalog catalog)
    {
        if (catalog.Classes.Count == 0) return;
        var @class = catalog.Classes.ElementAt(CurrentClass).Value;
        ClassNameLabel.Text = @class.Name;
        ClassDescLabel.Text = @class.Description;
    }

    private void OnRenderFace(Point pos)
    {
        var textureNum = GetCurrentTextureNum();
        if (textureNum > 0) characterRenderer.DrawFace(textureNum, pos);
    }

    private void OnRenderSprite(Point pos)
    {
        var textureNum = GetCurrentTextureNum();
        if (textureNum > 0)
            characterRenderer.DrawCharacter(textureNum, pos, Direction.Down, AnimationStopped);
    }
}
