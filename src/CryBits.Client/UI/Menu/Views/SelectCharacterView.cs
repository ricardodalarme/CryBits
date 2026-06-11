using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Enums;
using System.Drawing;
using static CryBits.Globals;

namespace CryBits.Client.UI.Menu.Views;

internal class SelectCharacterView(AccountSender accountSender, CharacterRenderer characterRenderer) : IView
{
    internal static Panel SelectCharacterPanel => Tools.Panels["SelectCharacter"];
    internal static Button UseButton => Tools.Buttons["Character_Use"];
    internal static Button CreateButton => Tools.Buttons["Character_Create"];
    internal static Button DeleteButton => Tools.Buttons["Character_Delete"];
    internal static Button ChangeRightButton => Tools.Buttons["Character_ChangeRight"];
    private static Button ChangeLeftButton => Tools.Buttons["Character_ChangeLeft"];
    private static Picture FacePicture => Tools.Pictures["SelectCharacter_Face"];
    private static Picture SpritePicture => Tools.Pictures["SelectCharacter_Sprite"];
    private static Label NameLabel => Tools.Labels["SelectCharacter_Name"];

    public static TempCharacter[] Characters;
    public static int CurrentCharacter = 1;

    public struct TempCharacter
    {
        public string Name;
        public short TextureNum;
    }

    public void Bind()
    {
        FacePicture.OnRender += OnRenderFace;
        SpritePicture.OnRender += OnRenderSprite;
        UseButton.OnMouseUp += OnUsePressed;
        CreateButton.OnMouseUp += OnCreatePressed;
        DeleteButton.OnMouseUp += OnDeletePressed;
        ChangeRightButton.OnMouseUp += OnChangeRightPressed;
        ChangeLeftButton.OnMouseUp += OnChangeLeftPressed;
        UpdateButtonVisibility();
    }

    public void Unbind()
    {
        FacePicture.OnRender -= OnRenderFace;
        SpritePicture.OnRender -= OnRenderSprite;
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
        UpdateButtonVisibility();
    }

    private void OnChangeLeftPressed()
    {
        if (CurrentCharacter == 0)
            CurrentCharacter = Characters.Length;
        else
            CurrentCharacter--;
        UpdateButtonVisibility();
    }

    public static bool UpdateButtonVisibility()
    {
        var visibility = Characters != null && CurrentCharacter < Characters.Length;
        CreateButton.Visible = !visibility;
        DeleteButton.Visible = visibility;
        UseButton.Visible = visibility;
        FacePicture.Visible = visibility;
        SpritePicture.Visible = visibility;
        UpdateNameLabel();
        return visibility;
    }

    private static void UpdateNameLabel()
    {
        var index = CurrentCharacter + 1;
        var hasCharacter = Characters != null && CurrentCharacter < Characters.Length;
        NameLabel.Text = hasCharacter
            ? $"({index}) {Characters[CurrentCharacter].Name}"
            : $"({index}) None";
    }

    private void OnRenderFace(Point pos)
    {
        var textureNum = Characters[CurrentCharacter].TextureNum;
        if (textureNum > 0) characterRenderer.DrawFace(textureNum, pos);
    }

    private void OnRenderSprite(Point pos)
    {
        var textureNum = Characters[CurrentCharacter].TextureNum;
        if (textureNum <= 0) return;
        characterRenderer.DrawCharacter(textureNum, pos, Direction.Down, AnimationStopped);
    }
}
