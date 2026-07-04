using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Common;
using Iguina.Entities;
using System.Drawing;

namespace CryBits.Client.UI.Menu.Views;

internal class SelectCharacterView(UiContext uiContext, AccountSender accountSender, CharacterRenderer characterRenderer) : ViewBase
{
    internal Panel SelectCharacterPanel => uiContext.Get<Panel>("SelectCharacter");
    internal Button UseButton => uiContext.Get<Button>("CharUse");
    internal Button CreateButton => uiContext.Get<Button>("CharCreate");
    internal Button DeleteButton => uiContext.Get<Button>("CharDelete");
    internal Button ChangeRightButton => uiContext.Get<Button>("CharRight");
    private Button ChangeLeftButton => uiContext.Get<Button>("CharLeft");
    private Picture FacePicture => uiContext.Get<Picture>("CharFace");
    private Picture SpritePicture => uiContext.Get<Picture>("CharSprite");
    private Label CharNameLabel => uiContext.Get<Label>("CharName");

    public TempCharacter[] Characters = [];
    public int CurrentCharacter;

    public struct TempCharacter
    {
        public string Name;
        public short TextureNum;
    }

    public override void Bind()
    {
        UseButton.Events.OnClick += OnUsePressed;
        CreateButton.Events.OnClick += OnCreatePressed;
        DeleteButton.Events.OnClick += OnDeletePressed;
        ChangeRightButton.Events.OnClick += OnChangeRightPressed;
        ChangeLeftButton.Events.OnClick += OnChangeLeftPressed;

        FacePicture.OnRenderPicture += RenderFace;
        SpritePicture.OnRenderPicture += RenderSprite;
        uiContext.PostDraw += FacePicture.Render;
        uiContext.PostDraw += SpritePicture.Render;

        UpdateButtonVisibility();
    }

    public override void Unbind()
    {
        UseButton.Events.OnClick -= OnUsePressed;
        CreateButton.Events.OnClick -= OnCreatePressed;
        DeleteButton.Events.OnClick -= OnDeletePressed;
        ChangeRightButton.Events.OnClick -= OnChangeRightPressed;
        ChangeLeftButton.Events.OnClick -= OnChangeLeftPressed;
        FacePicture.OnRenderPicture -= RenderFace;
        SpritePicture.OnRenderPicture -= RenderSprite;
        uiContext.PostDraw -= FacePicture.Render;
        uiContext.PostDraw -= SpritePicture.Render;
    }

    private void RenderFace()
    {
        if (CurrentCharacter >= Characters.Length) return;
        var textureNum = Characters[CurrentCharacter].TextureNum;
        if (textureNum <= 0) return;
        var pos = FacePicture.LastBoundingRect;
        characterRenderer.DrawFace(textureNum, new Point(pos.X, pos.Y));
    }

    private void RenderSprite()
    {
        if (CurrentCharacter >= Characters.Length) return;
        var textureNum = Characters[CurrentCharacter].TextureNum;
        if (textureNum <= 0) return;
        var pos = SpritePicture.LastBoundingRect;
        characterRenderer.DrawCharacter(textureNum, new Point(pos.X, pos.Y), Direction.Down, 1);
    }

    private void OnUsePressed(Entity _) => accountSender.CharacterUse(CurrentCharacter);
    private void OnDeletePressed(Entity _) => accountSender.CharacterDelete(CurrentCharacter);
    private void OnCreatePressed(Entity _) => accountSender.CharacterCreate();

    private void OnChangeRightPressed(Entity _)
    {
        if (CurrentCharacter == Characters.Length - 1) CurrentCharacter = 0; else CurrentCharacter++;
        UpdateButtonVisibility();
    }

    private void OnChangeLeftPressed(Entity _)
    {
        if (CurrentCharacter == 0) CurrentCharacter = Characters.Length; else CurrentCharacter--;
        UpdateButtonVisibility();
    }

    public bool UpdateButtonVisibility()
    {
        var visibility = CurrentCharacter < Characters.Length;
        CreateButton.Visible = !visibility;
        DeleteButton.Visible = visibility;
        UseButton.Visible = visibility;
        UpdateNameLabel();
        return visibility;
    }

    private void UpdateNameLabel()
    {
        var index = CurrentCharacter + 1;
        var hasCharacter = CurrentCharacter < Characters.Length;
        CharNameLabel.Text = hasCharacter
            ? $"({index}) {Characters[CurrentCharacter].Name}"
            : $"({index}) None";
    }
}
