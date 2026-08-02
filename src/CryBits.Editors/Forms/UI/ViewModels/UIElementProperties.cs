using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using PropertyModels.Collections;
using PropertyModels.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal abstract class UIElementProperties : ReactiveObject
{
    private static readonly Dictionary<string, global::Iguina.Defs.Anchor> AnchorMap = new()
    {
        ["TopLeft"] = global::Iguina.Defs.Anchor.TopLeft,
        ["TopCenter"] = global::Iguina.Defs.Anchor.TopCenter,
        ["TopRight"] = global::Iguina.Defs.Anchor.TopRight,
        ["BottomLeft"] = global::Iguina.Defs.Anchor.BottomLeft,
        ["BottomCenter"] = global::Iguina.Defs.Anchor.BottomCenter,
        ["BottomRight"] = global::Iguina.Defs.Anchor.BottomRight,
        ["CenterLeft"] = global::Iguina.Defs.Anchor.CenterLeft,
        ["Center"] = global::Iguina.Defs.Anchor.Center,
        ["CenterRight"] = global::Iguina.Defs.Anchor.CenterRight,
        ["AutoLTR"] = global::Iguina.Defs.Anchor.AutoLTR,
        ["AutoInlineLTR"] = global::Iguina.Defs.Anchor.AutoInlineLTR,
        ["AutoRTL"] = global::Iguina.Defs.Anchor.AutoRTL,
        ["AutoInlineRTL"] = global::Iguina.Defs.Anchor.AutoInlineRTL,
        ["AutoCenter"] = global::Iguina.Defs.Anchor.AutoCenter
    };

    protected readonly Element _config;
    protected readonly Entity _entity;
    private readonly SelectableList<string> _anchorOptions;

    protected UIElementProperties(Element config, Entity entity)
    {
        _config = config;
        _entity = entity;
        _anchorOptions = [.. AnchorMap.Keys];
        _anchorOptions.SelectedValue = GetAnchorName() ?? "TopLeft";
        _anchorOptions.SelectionChanged += OnAnchorChanged;
    }

    private string? GetAnchorName()
    {
        foreach (var kv in AnchorMap.Where(kv => kv.Value == _entity.Anchor)) return kv.Key;
        return "TopLeft";
    }

    private void OnAnchorChanged(object? sender, EventArgs e)
    {
        if (_anchorOptions.SelectedValue is not { } name) return;
        if (AnchorMap.TryGetValue(name, out var anchor))
        {
            _config.Anchor = name;
            _entity.Anchor = anchor;
        }
    }

    [Category("Identity")]
    [DisplayName("Type")]
    [ReadOnly(true)]
    public string ElementType => GetTypeDiscriminator();

    public abstract string GetTypeDiscriminator();

    [Category("Identity")]
    [DisplayName("Name")]
    public string Name
    {
        get => _config.Name;
        set
        {
            _config.Name = value;
            RaisePropertyChanged(nameof(Name));
        }
    }

    [Category("Layout")]
    [Range(-9999, 9999)]
    public int X
    {
        get => _entity.Offset.X.GetValueInPixels(800);
        set
        {
            _config.X = value;
            _entity.Offset.SetPixels(value, _entity.Offset.Y.GetValueInPixels(608));
            RaisePropertyChanged(nameof(X));
        }
    }

    [Category("Layout")]
    [Range(-9999, 9999)]
    public int Y
    {
        get => _entity.Offset.Y.GetValueInPixels(608);
        set
        {
            _config.Y = value;
            _entity.Offset.SetPixels(_entity.Offset.X.GetValueInPixels(800), value);
            RaisePropertyChanged(nameof(Y));
        }
    }

    [Category("Layout")]
    [Range(0, 9999)]
    public int Width
    {
        get => _entity.Size.X.GetValueInPixels(800);
        set
        {
            _config.Width = value;
            _entity.Size.SetPixels(value, _entity.Size.Y.GetValueInPixels(608));
            RaisePropertyChanged(nameof(Width));
        }
    }

    [Category("Layout")]
    [Range(0, 9999)]
    public int Height
    {
        get => _entity.Size.Y.GetValueInPixels(608);
        set
        {
            _config.Height = value;
            _entity.Size.SetPixels(_entity.Size.X.GetValueInPixels(800), value);
            RaisePropertyChanged(nameof(Height));
        }
    }

    [Category("Layout")] public ISelectableList Anchor => _anchorOptions;

    [Category("State")]
    public bool Visible
    {
        get => _entity.Visible;
        set
        {
            _config.Visible = value;
            _entity.Visible = value;
            RaisePropertyChanged(nameof(Visible));
        }
    }
}
