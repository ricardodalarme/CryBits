using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Iguina;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using Iguina;
using Iguina.Entities;
using System.Drawing;
using static CryBits.Definitions.Globals;
using Dir = CryBits.Definitions.Common.Direction;
using Ent = Iguina.Entities.Entity;

namespace CryBits.Client.UI.Menu.Views;

internal sealed class CreateCharacterView(UISystem ui, CharacterRenderer characterRenderer, DefinitionCatalog catalog)
{
    private Panel? _panel;
    private TextInput? _nameInput;
    private Label? _classNameLabel;
    private Label? _classDescLabel;
    private Panel? _facePanel;
    private Panel? _spritePanel;
    private Checkbox? _maleCheckbox;
    private Checkbox? _femaleCheckbox;

    public event Action? SelectCharacterRequested;

    public void Build(Panel root, ScreenData config)
    {
        var (panel, reg) = MenuLoader.BuildScreen(ui, config, root);
        _panel = panel;
        _nameInput = reg["CreateName"] as TextInput;
        _maleCheckbox = reg["CreateMale"] as Checkbox;
        _femaleCheckbox = reg["CreateFemale"] as Checkbox;
        _classNameLabel = reg["CreateClassName"] as Label;
        _classDescLabel = reg["CreateClassDesc"] as Label;
        _facePanel = reg["CreateFace"] as Panel;
        _spritePanel = reg["CreateSprite"] as Panel;

        _maleCheckbox!.ExclusiveSelection = true;
        _maleCheckbox.CanClickToUncheck = false;
        _femaleCheckbox!.ExclusiveSelection = true;
        _femaleCheckbox.CanClickToUncheck = false;

        _maleCheckbox.Events.OnChecked += OnMaleChanged;
        _maleCheckbox.Events.OnUnchecked += OnMaleChanged;
        _femaleCheckbox.Events.OnChecked += OnFemaleChanged;
        _femaleCheckbox.Events.OnUnchecked += OnFemaleChanged;

        ((Button)reg["CreateClassLeft"]).Events.OnClick += OnClassLeft;
        ((Button)reg["CreateClassRight"]).Events.OnClick += OnClassRight;
        ((Button)reg["CreateTexLeft"]).Events.OnClick += OnTexLeft;
        ((Button)reg["CreateTexRight"]).Events.OnClick += OnTexRight;
        ((Button)reg["CreateBtn"]).Events.OnClick += OnCreate;
        ((Button)reg["CreateBackBtn"]).Events.OnClick += _ => SelectCharacterRequested?.Invoke();

        _facePanel!.Events.AfterDraw += OnRenderFace;
        _spritePanel!.Events.AfterDraw += OnRenderSprite;

        // Initialize with defaults
        _nameInput!.Value = string.Empty;

        UpdateLabels();
    }

    public void Destroy()
    {
        _panel?.RemoveSelf();
        _panel = null;
        _nameInput = null;
        _classNameLabel = null;
        _classDescLabel = null;
        _facePanel = null;
        _spritePanel = null;
        _maleCheckbox = null;
        _femaleCheckbox = null;
    }

    public void Refresh()
    {
        if (_classNameLabel != null && _classDescLabel != null)
            UpdateLabels();
    }

    private void UpdateLabels()
    {
        if (catalog.Classes.Count == 0) return;
        var @class = catalog.Classes.ElementAt(MenuState.CurrentClass).Value;
        _classNameLabel!.Text = @class.Name;
        _classDescLabel!.Text = @class.Description;
    }

    private short GetTextureNum()
    {
        if (catalog.Classes.Count == 0) return 0;
        var @class = catalog.Classes.ElementAt(MenuState.CurrentClass).Value;
        var isMale = _maleCheckbox?.Checked ?? true;
        if (isMale && @class.TextureMale.Count > 0) return @class.TextureMale[MenuState.CurrentTexture];
        if (@class.TextureFemale.Count > 0) return @class.TextureFemale[MenuState.CurrentTexture];
        return 0;
    }

    private void OnMaleChanged(Ent _)
    {
        _femaleCheckbox!.Checked = !_maleCheckbox!.Checked;
        MenuState.CurrentTexture = 0;
        UpdateLabels();
    }

    private void OnFemaleChanged(Ent _)
    {
        _maleCheckbox!.Checked = !_femaleCheckbox!.Checked;
        MenuState.CurrentTexture = 0;
        UpdateLabels();
    }

    private void OnClassLeft(Ent _)
    {
        MenuState.CurrentClass = MenuState.CurrentClass == 0
            ? (byte)catalog.Classes.Count
            : (byte)(MenuState.CurrentClass - 1);
        MenuState.CurrentTexture = 0;
        UpdateLabels();
    }

    private void OnClassRight(Ent _)
    {
        MenuState.CurrentClass = MenuState.CurrentClass >= catalog.Classes.Count - 1
            ? (byte)0
            : (byte)(MenuState.CurrentClass + 1);
        MenuState.CurrentTexture = 0;
        UpdateLabels();
    }

    private void OnTexLeft(Ent _)
    {
        if (catalog.Classes.Count == 0) return;
        var @class = catalog.Classes.ElementAt(MenuState.CurrentClass).Value;
        var list = (_maleCheckbox?.Checked ?? true) ? @class.TextureMale : @class.TextureFemale;
        MenuState.CurrentTexture = MenuState.CurrentTexture == 0
            ? (byte)(list.Count - 1)
            : (byte)(MenuState.CurrentTexture - 1);
    }

    private void OnTexRight(Ent _)
    {
        if (catalog.Classes.Count == 0) return;
        var @class = catalog.Classes.ElementAt(MenuState.CurrentClass).Value;
        var list = (_maleCheckbox?.Checked ?? true) ? @class.TextureMale : @class.TextureFemale;
        MenuState.CurrentTexture = MenuState.CurrentTexture >= list.Count - 1
            ? (byte)0
            : (byte)(MenuState.CurrentTexture + 1);
    }

    private void OnCreate(Ent _)
    {
        var name = _nameInput?.Value ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            ui.MessageBoxes.ShowInfoMessageBox("Error", "Enter a character name.", null, "OK");
            return;
        }
        AccountSender.Instance.CreateCharacter(
            name, _maleCheckbox?.Checked ?? true,
            MenuState.CurrentClass, MenuState.CurrentTexture);
    }

    private void OnRenderFace(Ent _)
    {
        var rect = _facePanel!.LastVisibleBoundingRect;
        var texNum = GetTextureNum();
        if (texNum > 0) characterRenderer.DrawFace(texNum, new Point(rect.X, rect.Y));
    }

    private void OnRenderSprite(Ent _)
    {
        var rect = _spritePanel!.LastVisibleBoundingRect;
        var texNum = GetTextureNum();
        if (texNum > 0) characterRenderer.DrawCharacter(texNum, new Point(rect.X, rect.Y), Dir.Down, AnimationStopped);
    }
}
