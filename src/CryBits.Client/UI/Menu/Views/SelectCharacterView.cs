using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Iguina;
using CryBits.Client.Network.Senders;
using Iguina;
using Iguina.Entities;
using System.Drawing;
using static CryBits.Definitions.Globals;
using Dir = CryBits.Definitions.Common.Direction;
using Ent = Iguina.Entities.Entity;

namespace CryBits.Client.UI.Menu.Views;

internal sealed class SelectCharacterView(UISystem ui, CharacterRenderer characterRenderer)
{
    private Panel? _panel;
    private Label? _characterNameLabel;
    private Panel? _facePanel;
    private Panel? _spritePanel;
    private Button? _useButton;
    private Button? _deleteButton;
    private Button? _createButton;

    public event Action? CreateRequested;

    public void Build(Panel root, ScreenData config)
    {
        var (panel, reg) = MenuLoader.BuildScreen(ui, config, root);
        _panel = panel;
        _characterNameLabel = reg["CharName"] as Label;
        _facePanel = reg["CharFace"] as Panel;
        _spritePanel = reg["CharSprite"] as Panel;

        ((Button)reg["CharLeft"]).Events.OnClick += OnLeft;
        ((Button)reg["CharRight"]).Events.OnClick += OnRight;
        ((Button)reg["CharUse"]).Events.OnClick += _ => AccountSender.Instance.CharacterUse(MenuState.CurrentCharacter);
        ((Button)reg["CharDelete"]).Events.OnClick += _ => AccountSender.Instance.CharacterDelete(MenuState.CurrentCharacter);
        ((Button)reg["CharCreate"]).Events.OnClick += _ => CreateRequested?.Invoke();

        _useButton = reg["CharUse"] as Button;
        _deleteButton = reg["CharDelete"] as Button;
        _createButton = reg["CharCreate"] as Button;

        _facePanel!.Events.AfterDraw += OnRenderFace;
        _spritePanel!.Events.AfterDraw += OnRenderSprite;

        Refresh();
    }

    public void Destroy()
    {
        _panel?.RemoveSelf();
        _panel = null;
        _characterNameLabel = null;
        _facePanel = null;
        _spritePanel = null;
    }

    public void Refresh()
    {
        if (_characterNameLabel == null) return;

        var characters = MenuState.Characters;
        var current = MenuState.CurrentCharacter;
        var hasCharacter = characters != null && current < characters.Length;

        _characterNameLabel.Text = characters == null || characters.Length == 0
            ? "(1) None"
            : $"({current + 1}) {(hasCharacter ? characters[current].Name : "None")}";

        if (_useButton != null) _useButton.Visible = hasCharacter;
        if (_deleteButton != null) _deleteButton.Visible = hasCharacter;
        if (_createButton != null) _createButton.Visible = !hasCharacter;
        if (_facePanel != null) _facePanel.Visible = hasCharacter;
        if (_spritePanel != null) _spritePanel.Visible = hasCharacter;
    }

    private void OnLeft(Ent _)
    {
        if (MenuState.Characters == null) return;
        MenuState.CurrentCharacter = MenuState.CurrentCharacter == 0
            ? MenuState.Characters.Length
            : MenuState.CurrentCharacter - 1;
        Refresh();
    }

    private void OnRight(Ent _)
    {
        if (MenuState.Characters == null) return;
        MenuState.CurrentCharacter = MenuState.CurrentCharacter >= MenuState.Characters.Length - 1
            ? 0
            : MenuState.CurrentCharacter + 1;
        Refresh();
    }

    private void OnRenderFace(Ent _)
    {
        if (MenuState.Characters == null) return;
        var current = MenuState.CurrentCharacter;
        if (current >= MenuState.Characters.Length) return;
        var rect = _facePanel!.LastVisibleBoundingRect;
        var texNum = MenuState.Characters[current].TextureNum;
        if (texNum > 0) characterRenderer.DrawFace(texNum, new Point(rect.X, rect.Y));
    }

    private void OnRenderSprite(Ent _)
    {
        if (MenuState.Characters == null) return;
        var current = MenuState.CurrentCharacter;
        if (current >= MenuState.Characters.Length) return;
        var rect = _spritePanel!.LastVisibleBoundingRect;
        var texNum = MenuState.Characters[current].TextureNum;
        if (texNum > 0) characterRenderer.DrawCharacter(texNum, new Point(rect.X, rect.Y), Dir.Down, AnimationStopped);
    }
}
