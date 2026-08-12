using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.UI;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using Iguina.Entities;
using Microsoft.Xna.Framework;

namespace CryBits.Client.UI.Menu.Views;

internal class CreateCharacterView(
    UiContext uiContext,
    AccountSender accountSender,
    PortraitRenderer characterRenderer,
    DefinitionCatalog catalog,
    MenuScreen menuScreen) : ViewBase
{
    private Panel CreateCharacterPanel => uiContext.Get<Panel>("CreateCharacter");
    private TextInput NameTextBox => uiContext.Get<TextInput>("CreateName");
    private Button CreateButton => uiContext.Get<Button>("CreateBtn");
    private Button ChangeClassRightButton => uiContext.Get<Button>("CreateClassRight");
    private Button ChangeClassLeftButton => uiContext.Get<Button>("CreateClassLeft");
    private Button TextureChangeLeftButton => uiContext.Get<Button>("CreateTexLeft");
    private Button TextureChangeRightButton => uiContext.Get<Button>("CreateTexRight");
    private RadioButton GenderMaleRadio => uiContext.Get<RadioButton>("CreateMale");
    private RadioButton GenderFemaleRadio => uiContext.Get<RadioButton>("CreateFemale");
    private Picture FacePicture => uiContext.Get<Picture>("CreateFace");
    private Picture SpritePicture => uiContext.Get<Picture>("CreateSprite");
    private Button BackButton => uiContext.Get<Button>("CreateBackBtn");
    private Label ClassNameLabel => uiContext.Get<Label>("CreateClassName");
    private Label ClassDescLabel => uiContext.Get<Label>("CreateClassDesc");

    private byte _currentClass;
    private byte _currentTexture;

    public void Open()
    {
        NameTextBox.Value = string.Empty;
        GenderMaleRadio.Checked = true;
        GenderFemaleRadio.Checked = false;
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
        CreateButton.Events.OnClick += OnCreatePressed;
        ChangeClassRightButton.Events.OnClick += OnChangeClassRightPressed;
        ChangeClassLeftButton.Events.OnClick += OnChangeClassLeftPressed;
        TextureChangeLeftButton.Events.OnClick += OnChangeTextureLeftPressed;
        TextureChangeRightButton.Events.OnClick += OnChangeTextureRight;
        GenderMaleRadio.Events.OnValueChanged += OnGenderChanged;
        GenderFemaleRadio.Events.OnValueChanged += OnGenderChanged;
        BackButton.Events.OnClick += OnBackPressed;

        FacePicture.OnRenderPicture += RenderFace;
        SpritePicture.OnRenderPicture += RenderSprite;
        uiContext.PostDraw += FacePicture.Render;
        uiContext.PostDraw += SpritePicture.Render;

        UpdateClassLabels();
    }

    public override void Unbind()
    {
        CreateButton.Events.OnClick -= OnCreatePressed;
        ChangeClassRightButton.Events.OnClick -= OnChangeClassRightPressed;
        ChangeClassLeftButton.Events.OnClick -= OnChangeClassLeftPressed;
        TextureChangeLeftButton.Events.OnClick -= OnChangeTextureLeftPressed;
        TextureChangeRightButton.Events.OnClick -= OnChangeTextureRight;
        GenderMaleRadio.Events.OnValueChanged -= OnGenderChanged;
        GenderFemaleRadio.Events.OnValueChanged -= OnGenderChanged;
        BackButton.Events.OnClick -= OnBackPressed;
        FacePicture.OnRenderPicture -= RenderFace;
        SpritePicture.OnRenderPicture -= RenderSprite;
        uiContext.PostDraw -= FacePicture.Render;
        uiContext.PostDraw -= SpritePicture.Render;
    }

    private void RenderFace()
    {
        var textureNum = GetCurrentTextureNum();
        if (textureNum <= 0) return;
        var pos = FacePicture.LastBoundingRect;
        characterRenderer.DrawFace(textureNum, new Vector2(pos.X, pos.Y));
    }

    private void RenderSprite()
    {
        var textureNum = GetCurrentTextureNum();
        if (textureNum <= 0) return;
        var pos = SpritePicture.LastBoundingRect;
        characterRenderer.DrawCharacter(textureNum, new Vector2(pos.X, pos.Y), Direction.Down, 1);
    }

    private void OnCreatePressed(Entity _)
    {
        var classId = catalog.Classes.Count == 0 ? Guid.Empty : catalog.Classes.ElementAt(_currentClass).Key;
        accountSender.CreateCharacter(
            name: NameTextBox.Value,
            isMale: GenderMaleRadio.Checked,
            classId: classId,
            textureNum: _currentTexture
        );
    }

    private void OnChangeClassRightPressed(Entity _)
    {
        if (_currentClass == catalog.Classes.Count - 1) _currentClass = 0;
        else _currentClass++;
        UpdateClassLabels();
    }

    private void OnChangeClassLeftPressed(Entity _)
    {
        if (_currentClass == 0) _currentClass = (byte)(catalog.Classes.Count - 1);
        else _currentClass--;
        UpdateClassLabels();
    }

    private void OnChangeTextureRight(Entity _)
    {
        var @class = catalog.Classes.ElementAt(_currentClass).Value;
        var texList = GenderMaleRadio.Checked ? @class.TextureMale : @class.TextureFemale;
        if (_currentTexture == texList.Count - 1) _currentTexture = 0;
        else _currentTexture++;
    }

    private void OnChangeTextureLeftPressed(Entity _)
    {
        var @class = catalog.Classes.ElementAt(_currentClass).Value;
        var texList = GenderMaleRadio.Checked ? @class.TextureMale : @class.TextureFemale;
        if (_currentTexture == 0) _currentTexture = (byte)(texList.Count - 1);
        else _currentTexture--;
    }

    private void OnGenderChanged(Entity _)
    {
        _currentTexture = 0;
        UpdateClassLabels();
    }

    private void OnBackPressed(Entity _)
    {
        menuScreen.ShowSelectCharacter([]);
    }

    private short GetCurrentTextureNum()
    {
        if (catalog.Classes.Count == 0) return 0;
        var @class = catalog.Classes.ElementAt(_currentClass).Value;
        if (GenderMaleRadio.Checked && @class.TextureMale.Count > 0)
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
    }
}
