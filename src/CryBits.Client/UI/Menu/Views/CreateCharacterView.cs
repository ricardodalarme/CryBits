using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using Iguina.Entities;
using System.Drawing;

namespace CryBits.Client.UI.Menu.Views;

internal class CreateCharacterView(UiContext uiContext, AccountSender accountSender, CharacterRenderer characterRenderer, DefinitionCatalog catalog, MenuScreen menuScreen) : ViewBase
{
    internal Panel CreateCharacterPanel => uiContext.Get<Panel>("CreateCharacter");
    internal TextInput NameTextBox => uiContext.Get<TextInput>("CreateName");
    private Button CreateButton => uiContext.Get<Button>("CreateBtn");
    private Button ChangeClassRightButton => uiContext.Get<Button>("CreateClassRight");
    private Button ChangeClassLeftButton => uiContext.Get<Button>("CreateClassLeft");
    private Button TextureChangeLeftButton => uiContext.Get<Button>("CreateTexLeft");
    private Button TextureChangeRightButton => uiContext.Get<Button>("CreateTexRight");
    internal RadioButton GenderMaleRadio => uiContext.Get<RadioButton>("CreateMale");
    internal RadioButton GenderFemaleRadio => uiContext.Get<RadioButton>("CreateFemale");
    private Picture FacePicture => uiContext.Get<Picture>("CreateFace");
    private Picture SpritePicture => uiContext.Get<Picture>("CreateSprite");
    private Button BackButton => uiContext.Get<Button>("CreateBackBtn");
    private Label ClassNameLabel => uiContext.Get<Label>("CreateClassName");
    private Label ClassDescLabel => uiContext.Get<Label>("CreateClassDesc");

    public byte CurrentClass;
    public byte CurrentTexture;

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

        UpdateClassLabels(catalog);
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
        characterRenderer.DrawFace(textureNum, new Point(pos.X, pos.Y));
    }

    private void RenderSprite()
    {
        var textureNum = GetCurrentTextureNum();
        if (textureNum <= 0) return;
        var pos = SpritePicture.LastBoundingRect;
        characterRenderer.DrawCharacter(textureNum, new Point(pos.X, pos.Y), Direction.Down, 1);
    }

    private void OnCreatePressed(Entity _)
    {
        accountSender.CreateCharacter(
            name: NameTextBox.Value,
            isMale: GenderMaleRadio.Checked,
            @class: CurrentClass,
            textureNum: CurrentTexture
        );
    }

    private void OnChangeClassRightPressed(Entity _)
    {
        if (CurrentClass == catalog.Classes.Count - 1) CurrentClass = 0; else CurrentClass++;
        UpdateClassLabels(catalog);
    }

    private void OnChangeClassLeftPressed(Entity _)
    {
        if (CurrentClass == 0) CurrentClass = (byte)(catalog.Classes.Count - 1); else CurrentClass--;
        UpdateClassLabels(catalog);
    }

    private void OnChangeTextureRight(Entity _)
    {
        var @class = catalog.Classes.ElementAt(CurrentClass).Value;
        var texList = GenderMaleRadio.Checked ? @class.TextureMale : @class.TextureFemale;
        if (CurrentTexture == texList.Count - 1) CurrentTexture = 0; else CurrentTexture++;
    }

    private void OnChangeTextureLeftPressed(Entity _)
    {
        var @class = catalog.Classes.ElementAt(CurrentClass).Value;
        var texList = GenderMaleRadio.Checked ? @class.TextureMale : @class.TextureFemale;
        if (CurrentTexture == 0) CurrentTexture = (byte)(texList.Count - 1); else CurrentTexture--;
    }

    private void OnGenderChanged(Entity _)
    {
        CurrentTexture = 0;
        UpdateClassLabels(catalog);
    }

    private void OnBackPressed(Entity _)
    {
        menuScreen.CloseMenus();
        menuScreen.SelectCharacterView.SelectCharacterPanel.Visible = true;
    }

    private short GetCurrentTextureNum()
    {
        if (catalog.Classes.Count == 0) return 0;
        var @class = catalog.Classes.ElementAt(CurrentClass).Value;
        if (GenderMaleRadio.Checked && @class.TextureMale.Count > 0)
            return @class.TextureMale[CurrentTexture];
        if (@class.TextureFemale.Count > 0)
            return @class.TextureFemale[CurrentTexture];
        return 0;
    }

    internal void UpdateClassLabels(DefinitionCatalog catalog)
    {
        if (catalog.Classes.Count == 0) return;
        var @class = catalog.Classes.ElementAt(CurrentClass).Value;
        ClassNameLabel.Text = @class.Name;
        ClassDescLabel.Text = @class.Description;
    }
}
