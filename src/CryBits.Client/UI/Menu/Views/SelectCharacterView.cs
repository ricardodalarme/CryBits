using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.UI;
using CryBits.Definitions.Common;
using Iguina.Entities;
using System.Drawing;

namespace CryBits.Client.UI.Menu.Views;

internal class SelectCharacterView(UiContext uiContext, AccountSender accountSender, PortraitRenderer characterRenderer)
    : ViewBase
{
    private Panel SelectCharacterPanel => uiContext.Get<Panel>("SelectCharacter");
    private Button UseButton => uiContext.Get<Button>("CharUse");
    private Button CreateButton => uiContext.Get<Button>("CharCreate");
    private Button DeleteButton => uiContext.Get<Button>("CharDelete");
    private Button ChangeRightButton => uiContext.Get<Button>("CharRight");
    private Button ChangeLeftButton => uiContext.Get<Button>("CharLeft");
    private Picture FacePicture => uiContext.Get<Picture>("CharFace");
    private Picture SpritePicture => uiContext.Get<Picture>("CharSprite");
    private Label CharNameLabel => uiContext.Get<Label>("CharName");

    private TempCharacter[] _characters = [];
    private int _currentCharacter;

    public struct TempCharacter
    {
        public string Name;
        public short TextureNum;
    }

    public void Open(TempCharacter[] characters)
    {
        _characters = characters;
        _currentCharacter = 0;
        SelectCharacterPanel.Visible = true;
        Bind();
    }

    public void Close()
    {
        SelectCharacterPanel.Visible = false;
        Unbind();
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
        if (_currentCharacter >= _characters.Length) return;
        var textureNum = _characters[_currentCharacter].TextureNum;
        if (textureNum <= 0) return;
        var pos = FacePicture.LastBoundingRect;
        characterRenderer.DrawFace(textureNum, new Point(pos.X, pos.Y));
    }

    private void RenderSprite()
    {
        if (_currentCharacter >= _characters.Length) return;
        var textureNum = _characters[_currentCharacter].TextureNum;
        if (textureNum <= 0) return;
        var pos = SpritePicture.LastBoundingRect;
        characterRenderer.DrawCharacter(textureNum, new Point(pos.X, pos.Y), Direction.Down, 1);
    }

    private void OnUsePressed(Entity _) => accountSender.CharacterUse(_currentCharacter);
    private void OnDeletePressed(Entity _) => accountSender.CharacterDelete(_currentCharacter);
    private void OnCreatePressed(Entity _) => accountSender.CharacterCreate();

    private void OnChangeRightPressed(Entity _)
    {
        if (_currentCharacter == _characters.Length - 1) _currentCharacter = 0;
        else _currentCharacter++;
        UpdateButtonVisibility();
    }

    private void OnChangeLeftPressed(Entity _)
    {
        if (_currentCharacter == 0) _currentCharacter = _characters.Length;
        else _currentCharacter--;
        UpdateButtonVisibility();
    }

    private bool UpdateButtonVisibility()
    {
        var visibility = _currentCharacter < _characters.Length;
        CreateButton.Visible = !visibility;
        DeleteButton.Visible = visibility;
        UseButton.Visible = visibility;
        UpdateNameLabel();
        return visibility;
    }

    private void UpdateNameLabel()
    {
        var index = _currentCharacter + 1;
        var hasCharacter = _currentCharacter < _characters.Length;
        CharNameLabel.Text = hasCharacter
            ? $"({index}) {_characters[_currentCharacter].Name}"
            : $"({index}) None";
    }
}
