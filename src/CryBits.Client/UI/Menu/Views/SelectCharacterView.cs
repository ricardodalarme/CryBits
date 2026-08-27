using CryBits.Client.Framework.Assets;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.Entities;
using CryBits.Definitions.Common;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace CryBits.Client.UI.Menu.Views;

internal class SelectCharacterView(UiContext uiContext, AccountSender accountSender)
    : ViewBase
{
    private Panel SelectCharacterPanel => uiContext.Get<Panel>("SelectCharacter");
    private Button UseButton => uiContext.Get<Button>("CharUse");
    private Button CreateButton => uiContext.Get<Button>("CharCreate");
    private Button DeleteButton => uiContext.Get<Button>("CharDelete");
    private Button ChangeRightButton => uiContext.Get<Button>("CharRight");
    private Button ChangeLeftButton => uiContext.Get<Button>("CharLeft");
    private Image FaceImage => uiContext.Get<Image>("CharFace");
    private Image SpriteImage => uiContext.Get<Image>("CharSprite");
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
        UseButton.Click += OnUsePressed;
        CreateButton.Click += OnCreatePressed;
        DeleteButton.Click += OnDeletePressed;
        ChangeRightButton.Click += OnChangeRightPressed;
        ChangeLeftButton.Click += OnChangeLeftPressed;

        UpdateButtonVisibility();
    }

    public override void Unbind()
    {
        UseButton.Click -= OnUsePressed;
        CreateButton.Click -= OnCreatePressed;
        DeleteButton.Click -= OnDeletePressed;
        ChangeRightButton.Click -= OnChangeRightPressed;
        ChangeLeftButton.Click -= OnChangeLeftPressed;
    }

    private void UpdateRenderables()
    {
        if (_currentCharacter < _characters.Length)
        {
            var textureNum = _characters[_currentCharacter].TextureNum;
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
        else
        {
            FaceImage.Renderable = null;
            SpriteImage.Renderable = null;
        }
    }

    private void OnUsePressed(object? sender, MyraEventArgs e) => accountSender.CharacterUse(_currentCharacter);
    private void OnDeletePressed(object? sender, MyraEventArgs e) => accountSender.CharacterDelete(_currentCharacter);
    private void OnCreatePressed(object? sender, MyraEventArgs e) => accountSender.CharacterCreate();

    private void OnChangeRightPressed(object? sender, MyraEventArgs e)
    {
        if (_currentCharacter == _characters.Length - 1) _currentCharacter = 0;
        else _currentCharacter++;
        UpdateButtonVisibility();
    }

    private void OnChangeLeftPressed(object? sender, MyraEventArgs e)
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
        UpdateRenderables();
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
