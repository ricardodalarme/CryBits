using CryBits.Client.Framework.Assets;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.Entities;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Menu.Views;

internal class CreateCharacterView(
    UiContext uiContext,
    AccountSender accountSender,
    DefinitionCatalog catalog,
    MenuScreen menuScreen) : ViewBase
{
    private Panel CreateCharacterPanel => uiContext.Get<Panel>("CreateCharacter");
    private TextBox NameTextBox => uiContext.Get<TextBox>("CreateName");
    private Button CreateButton => uiContext.Get<Button>("CreateBtn");
    private Button ChangeClassRightButton => uiContext.Get<Button>("CreateClassRight");
    private Button ChangeClassLeftButton => uiContext.Get<Button>("CreateClassLeft");
    private Button TextureChangeLeftButton => uiContext.Get<Button>("CreateTexLeft");
    private Button TextureChangeRightButton => uiContext.Get<Button>("CreateTexRight");
    private RadioButton GenderMaleRadio => uiContext.Get<RadioButton>("CreateMale");
    private RadioButton GenderFemaleRadio => uiContext.Get<RadioButton>("CreateFemale");
    private Image FaceImage => uiContext.Get<Image>("CreateFace");
    private Image SpriteImage => uiContext.Get<Image>("CreateSprite");
    private Button BackButton => uiContext.Get<Button>("CreateBackBtn");
    private Label ClassNameLabel => uiContext.Get<Label>("CreateClassName");
    private Label ClassDescLabel => uiContext.Get<Label>("CreateClassDesc");

    private byte _currentClass;
    private byte _currentTexture;

    public void Open()
    {
        NameTextBox.Text = string.Empty;
        GenderMaleRadio.IsPressed = true;
        GenderFemaleRadio.IsPressed = false;
        _currentClass = 0;
        _currentTexture = 0;

        CreateCharacterPanel.Visible = true;
        Bind();
    }

    public void Close()
    {
        CreateCharacterPanel.Visible = false;
        Unbind();
    }

    public override void Bind()
    {
        CreateButton.Click += OnCreatePressed;
        ChangeClassRightButton.Click += OnChangeClassRightPressed;
        ChangeClassLeftButton.Click += OnChangeClassLeftPressed;
        TextureChangeLeftButton.Click += OnChangeTextureLeftPressed;
        TextureChangeRightButton.Click += OnChangeTextureRight;
        GenderMaleRadio.Click += OnGenderChanged;
        GenderFemaleRadio.Click += OnGenderChanged;
        BackButton.Click += OnBackPressed;

        UpdateClassLabels();
    }

    public override void Unbind()
    {
        CreateButton.Click -= OnCreatePressed;
        ChangeClassRightButton.Click -= OnChangeClassRightPressed;
        ChangeClassLeftButton.Click -= OnChangeClassLeftPressed;
        TextureChangeLeftButton.Click -= OnChangeTextureLeftPressed;
        TextureChangeRightButton.Click -= OnChangeTextureRight;
        GenderMaleRadio.Click -= OnGenderChanged;
        GenderFemaleRadio.Click -= OnGenderChanged;
        BackButton.Click -= OnBackPressed;
    }

    private void UpdateRenderables()
    {
        var textureNum = GetCurrentTextureNum();
        if (textureNum > 0 && Textures.Faces[textureNum] is { } faceTex)
        {
            FaceImage.Renderable = new TextureRegion(faceTex);
        }
        else
        {
            FaceImage.Renderable = null;
        }

        if (textureNum > 0 && Textures.Characters[textureNum] is { } charTex)
        {
            var sheet = SpriteSheet.Default;
            var frameW = sheet.FrameW(charTex.Width);
            var frameH = sheet.FrameH(charTex.Height);
            var line = sheet.RowForDirection(Direction.Down);
            var recSource = new Rectangle(1 * frameW, line * frameH, frameW, frameH);
            SpriteImage.Renderable = new TextureRegion(charTex, recSource);
        }
        else
        {
            SpriteImage.Renderable = null;
        }
    }

    private void OnCreatePressed(object? sender, MyraEventArgs e)
    {
        var classId = catalog.Classes.Count == 0 ? Guid.Empty : catalog.Classes.ElementAt(_currentClass).Key;
        accountSender.CreateCharacter(
            name: NameTextBox.Text,
            isMale: GenderMaleRadio.IsPressed,
            classId: classId,
            textureNum: _currentTexture
        );
    }

    private void OnChangeClassRightPressed(object? sender, MyraEventArgs e)
    {
        if (_currentClass == catalog.Classes.Count - 1) _currentClass = 0;
        else _currentClass++;
        UpdateClassLabels();
    }

    private void OnChangeClassLeftPressed(object? sender, MyraEventArgs e)
    {
        if (_currentClass == 0) _currentClass = (byte)(catalog.Classes.Count - 1);
        else _currentClass--;
        UpdateClassLabels();
    }

    private void OnChangeTextureRight(object? sender, MyraEventArgs e)
    {
        if (catalog.Classes.Count == 0) return;
        var @class = catalog.Classes.ElementAt(_currentClass).Value;
        var texList = GenderMaleRadio.IsPressed ? @class.TextureMale : @class.TextureFemale;
        if (_currentTexture == texList.Count - 1) _currentTexture = 0;
        else _currentTexture++;
        UpdateRenderables();
    }

    private void OnChangeTextureLeftPressed(object? sender, MyraEventArgs e)
    {
        if (catalog.Classes.Count == 0) return;
        var @class = catalog.Classes.ElementAt(_currentClass).Value;
        var texList = GenderMaleRadio.IsPressed ? @class.TextureMale : @class.TextureFemale;
        if (_currentTexture == 0) _currentTexture = (byte)(texList.Count - 1);
        else _currentTexture--;
        UpdateRenderables();
    }

    private void OnGenderChanged(object? sender, MyraEventArgs e)
    {
        _currentTexture = 0;
        UpdateClassLabels();
    }

    private void OnBackPressed(object? sender, MyraEventArgs e)
    {
        menuScreen.ShowSelectCharacter([]);
    }

    private short GetCurrentTextureNum()
    {
        if (catalog.Classes.Count == 0) return 0;
        var @class = catalog.Classes.ElementAt(_currentClass).Value;
        if (GenderMaleRadio.IsPressed && @class.TextureMale.Count > 0)
            return @class.TextureMale[_currentTexture];
        if (@class.TextureFemale.Count > 0)
            return @class.TextureFemale[_currentTexture];
        return 0;
    }

    public void UpdateClassLabels()
    {
        if (catalog.Classes.Count == 0) return;
        var @class = catalog.Classes.ElementAt(_currentClass).Value;
        ClassNameLabel.Text = @class.Name;
        ClassDescLabel.Text = @class.Description;
        UpdateRenderables();
    }
}
