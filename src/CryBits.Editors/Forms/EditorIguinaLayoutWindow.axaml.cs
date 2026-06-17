using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Iguina;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Iguina;
using Iguina.Defs;
using Iguina.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using Ent = Iguina.Entities.Entity;

namespace CryBits.Editors.Forms;

internal sealed class EntityNode : INotifyPropertyChanged
{
    private string _header = string.Empty;
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Header
    {
        get => _header;
        set { _header = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Header))); }
    }
    public Ent? Entity { get; set; }
    public Element? ConfigElement { get; set; }
    public EntityNode? Parent { get; set; }
    public ObservableCollection<EntityNode> Children { get; } = [];
    public override string ToString() => Header;
}

internal partial class EditorIguinaLayoutWindow : Window
{
    private readonly IguinaEditorPreview _preview;
    private readonly DispatcherTimer _timer;
    private WriteableBitmap? _bitmap;
    private EntityNode? _selectedNode;
    private MenuConfig? _currentConfig;
    private string _configPath = string.Empty;
    private bool _loading;

    private static readonly string[] AnchorNames =
    [
        "TopLeft","TopCenter","TopRight","BottomLeft","BottomCenter","BottomRight",
        "CenterLeft","Center","CenterRight",
        "AutoLTR","AutoInlineLTR","AutoRTL","AutoInlineRTL","AutoCenter"
    ];
    private static readonly Anchor[] AnchorValues =
    [
        Anchor.TopLeft, Anchor.TopCenter, Anchor.TopRight,
        Anchor.BottomLeft, Anchor.BottomCenter, Anchor.BottomRight,
        Anchor.CenterLeft, Anchor.Center, Anchor.CenterRight,
        Anchor.AutoLTR, Anchor.AutoInlineLTR, Anchor.AutoRTL, Anchor.AutoInlineRTL, Anchor.AutoCenter
    ];

    public EditorIguinaLayoutWindow()
    {
        InitializeComponent();

        _preview = new IguinaEditorPreview(800, 608);

        var themeDir = ResolveThemeDir();
        if (themeDir != null)
        {
            foreach (var f in Directory.GetFiles(themeDir, "*.json").Where(f => !f.EndsWith("system_style.json")))
                cmbFiles.Items.Add(System.IO.Path.GetFileName(f));
            if (cmbFiles.Items.Count > 0) cmbFiles.SelectedIndex = 0;
        }

        foreach (var a in AnchorNames) cmbAnchor.Items.Add(a);
        cmbAnchor.SelectedIndex = 0;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();
    }

    protected override void OnClosed(EventArgs e) { _timer?.Stop(); _preview.Dispose(); base.OnClosed(e); }

    private static string? ResolveThemeDir()
    {
        var p = Directories.IguinaTheme.FullName;
        return Directory.Exists(p) ? p : null;
    }

    private void OnRenderTick(object? s, EventArgs e)
    {
        _preview.Draw();
        SfmlRenderBlit.Blit(_preview.Target, ref _bitmap, imgPreview);
    }

    private void cmbFiles_SelectionChanged(object? s, SelectionChangedEventArgs e) => LoadConfig();
    private void cmbScreens_SelectionChanged(object? s, SelectionChangedEventArgs e) => SelectScreen();

    private void cmbMode_SelectionChanged(object? s, SelectionChangedEventArgs e)
    {
        var isPreview = cmbMode.SelectedIndex == 0;
        previewBorder.IsVisible = isPreview;
        jsonBorder.IsVisible = !isPreview;
        if (!isPreview) UpdateRawJson();
    }

    // ─── Load config ─────────────────────────────────────────────────────────

    private void LoadConfig()
    {
        if (cmbFiles.SelectedItem == null) return;
        _configPath = Path.Combine(ResolveThemeDir() ?? "", cmbFiles.SelectedItem.ToString() ?? "");
        if (!File.Exists(_configPath)) return;

        _preview.Clear();
        _currentConfig = JsonSerializer.Deserialize<MenuConfig>(File.ReadAllText(_configPath)) ?? new MenuConfig();

        // Populate screen dropdown
        cmbScreens.Items.Clear();
        foreach (var screen in _currentConfig.Screens)
            cmbScreens.Items.Add(screen.Name);
        if (cmbScreens.Items.Count > 0) cmbScreens.SelectedIndex = 0;

        _selectedNode = null;
        UpdateRawJson();
    }

    private void SelectScreen()
    {
        if (_currentConfig == null || cmbScreens.SelectedItem == null) return;

        _preview.Clear();
        _selectedNode = null;

        var screenName = cmbScreens.SelectedItem.ToString() ?? "";
        var screen = _currentConfig.Screens.FirstOrDefault(s => s.Name == screenName);
        if (screen == null) return;

        var (panel, reg) = MenuLoader.BuildScreen(_preview.UISystem, screen, _preview.UISystem.Root);

        var rootNode = new EntityNode { Header = $"[Screen] {screen.Name}", Entity = panel };
        foreach (var el in screen.Elements)
            rootNode.Children.Add(MakeElementNode(el, reg));
        treEntities.ItemsSource = new ObservableCollection<EntityNode> { rootNode };
        UpdateRawJson();
    }

    private static EntityNode MakeElementNode(Element el, Dictionary<string, Ent> reg)
    {
        var node = new EntityNode { Header = $"[{el.Type}] {el.Name}", ConfigElement = el };
        if (reg.TryGetValue(el.Name, out var entity))
            node.Entity = entity;
        foreach (var child in el.Children)
            node.Children.Add(MakeElementNode(child, reg));
        return node;
    }

    // ─── Raw JSON ────────────────────────────────────────────────────────────

    private void UpdateRawJson()
    {
        if (_currentConfig == null) return;
        txtRawJson.Text = JsonSerializer.Serialize(_currentConfig, new JsonSerializerOptions { WriteIndented = true });
    }

    // ─── Tree selection ──────────────────────────────────────────────────────

    private void treEntities_SelectionChanged(object? s, SelectionChangedEventArgs e)
    {
        _selectedNode = e.AddedItems.Count > 0 ? e.AddedItems[0] as EntityNode : null;
        UpdateProperties();
    }

    // ─── Properties ──────────────────────────────────────────────────────────

    private void UpdateProperties()
    {
        var en = _selectedNode;
        if (en?.Entity == null)
        {
            pnlProps.IsVisible = false; lblNoSelection.IsVisible = true;
            return;
        }
        _loading = true;
        lblNoSelection.IsVisible = false; pnlProps.IsVisible = true;
        lblType.Text = en.Entity.GetType().Name;
        txtElemName.Text = en.ConfigElement?.Name ?? "";

        // Layout
        numX.Value = en.Entity.Offset.X.GetValueInPixels((int)_preview.Target.Size.X);
        numY.Value = en.Entity.Offset.Y.GetValueInPixels((int)_preview.Target.Size.Y);
        numW.Value = en.Entity.Size.X.GetValueInPixels(800);
        numH.Value = en.Entity.Size.Y.GetValueInPixels(608);
        var ai = Array.FindIndex(AnchorValues, a => a == en.Entity.Anchor);
        cmbAnchor.SelectedIndex = ai >= 0 ? ai : 0;

        // State
        chkVisible.IsChecked = en.Entity.Visible;
        chkEnabled.IsChecked = en.Entity.Enabled;

        // Texture
        txtTextureId.Text = en.ConfigElement?.Texture ?? "";
        numSrcX.Value = en.ConfigElement?.SrcX ?? 0;
        numSrcY.Value = en.ConfigElement?.SrcY ?? 0;
        numSrcW.Value = en.ConfigElement?.SrcW ?? 32;
        numSrcH.Value = en.ConfigElement?.SrcH ?? 32;

        // Text
        txtText.Text = en.Entity is Paragraph p ? p.Text : (en.ConfigElement?.Text ?? "");
        numFontSize.Value = en.ConfigElement?.FontSize ?? 0;
        txtTextFillColor.Text = en.ConfigElement?.TextFillColor ?? "";
        txtTextOutlineColor.Text = en.ConfigElement?.TextOutlineColor ?? "";
        numTextOutlineWidth.Value = en.ConfigElement?.TextOutlineWidth ?? 0;

        // TextInput
        txtPlaceholder.Text = en.ConfigElement?.PlaceholderText ?? "";
        numMaxLength.Value = en.ConfigElement?.MaxLength ?? 0;
        chkMasked.IsChecked = en.ConfigElement?.Masked ?? false;

        // Checked
        chkChecked.IsChecked = en.ConfigElement?.Checked ?? false;

        // Padding
        numPadL.Value = en.ConfigElement?.PaddingLeft ?? 0;
        numPadR.Value = en.ConfigElement?.PaddingRight ?? 0;
        numPadT.Value = en.ConfigElement?.PaddingTop ?? 0;
        numPadB.Value = en.ConfigElement?.PaddingBottom ?? 0;

        // SlotGrid
        numCols.Value = en.ConfigElement?.Columns ?? 0;
        numRows.Value = en.ConfigElement?.Rows ?? 0;
        numSlotSize.Value = en.ConfigElement?.SlotSize ?? 32;
        numSlotPad.Value = en.ConfigElement?.SlotPadding ?? 0;

        _loading = false;
    }

    private void ApplyProp(Action<Ent> apply)
    {
        if (_loading || _selectedNode?.Entity == null) return;
        apply(_selectedNode.Entity);
    }

    private void ApplyConfig(Action<Element> apply)
    {
        if (_loading || _selectedNode?.ConfigElement == null) return;
        apply(_selectedNode.ConfigElement);
    }

    // Layout property handlers
    private void numX_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyProp(ent => ent.Offset.SetPixels((int)(e.NewValue ?? 0), ent.Offset.Y.GetValueInPixels(800)));
    private void numY_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyProp(ent => ent.Offset.SetPixels(ent.Offset.X.GetValueInPixels(800), (int)(e.NewValue ?? 0)));
    private void numW_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyProp(ent => ent.Size.SetPixels((int)(e.NewValue ?? 0), ent.Size.Y.GetValueInPixels(608)));
    private void numH_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyProp(ent => ent.Size.SetPixels(ent.Size.X.GetValueInPixels(800), (int)(e.NewValue ?? 0)));
    private void cmbAnchor_SelectionChanged(object? s, SelectionChangedEventArgs e)
    {
        if (_loading || _selectedNode?.Entity == null) return;
        var idx = cmbAnchor.SelectedIndex;
        if (idx >= 0 && idx < AnchorValues.Length)
            _selectedNode.Entity.Anchor = AnchorValues[idx];
    }

    // State
    private void chkVisible_IsCheckedChanged(object? s, RoutedEventArgs e)
        => ApplyProp(ent => ent.Visible = chkVisible.IsChecked ?? true);
    private void chkEnabled_IsCheckedChanged(object? s, RoutedEventArgs e)
        => ApplyProp(ent => ent.Enabled = chkEnabled.IsChecked ?? true);

    // Name
    private void txtElemName_TextChanged(object? s, TextChangedEventArgs e)
        => ApplyConfig(el => el.Name = txtElemName.Text ?? "");

    // Texture
    private void txtTextureId_TextChanged(object? s, TextChangedEventArgs e)
        => ApplyConfig(el => el.Texture = txtTextureId.Text ?? "");
    private void numSrcX_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.SrcX = (int)(e.NewValue ?? 0));
    private void numSrcY_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.SrcY = (int)(e.NewValue ?? 0));
    private void numSrcW_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.SrcW = (int)(e.NewValue ?? 32));
    private void numSrcH_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.SrcH = (int)(e.NewValue ?? 32));

    // Text
    private void txtText_TextChanged(object? s, TextChangedEventArgs e)
    {
        ApplyConfig(el => el.Text = txtText.Text ?? "");
        ApplyProp(ent => { if (ent is Paragraph p) p.Text = txtText.Text ?? ""; });
    }
    private void numFontSize_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.FontSize = (int)(e.NewValue ?? 0));
    private void txtTextFillColor_TextChanged(object? s, TextChangedEventArgs e)
        => ApplyConfig(el => el.TextFillColor = txtTextFillColor.Text ?? "");
    private void txtTextOutlineColor_TextChanged(object? s, TextChangedEventArgs e)
        => ApplyConfig(el => el.TextOutlineColor = txtTextOutlineColor.Text ?? "");
    private void numTextOutlineWidth_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.TextOutlineWidth = (int)(e.NewValue ?? 0));

    // TextInput
    private void txtPlaceholder_TextChanged(object? s, TextChangedEventArgs e)
        => ApplyConfig(el => el.PlaceholderText = txtPlaceholder.Text ?? "");
    private void numMaxLength_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.MaxLength = (int)(e.NewValue ?? 0));
    private void chkMasked_IsCheckedChanged(object? s, RoutedEventArgs e)
        => ApplyConfig(el => el.Masked = chkMasked.IsChecked ?? false);

    // Checked
    private void chkChecked_IsCheckedChanged(object? s, RoutedEventArgs e)
        => ApplyConfig(el => el.Checked = chkChecked.IsChecked ?? false);

    // Padding
    private void numPadL_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.PaddingLeft = (int)(e.NewValue ?? 0));
    private void numPadR_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.PaddingRight = (int)(e.NewValue ?? 0));
    private void numPadT_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.PaddingTop = (int)(e.NewValue ?? 0));
    private void numPadB_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.PaddingBottom = (int)(e.NewValue ?? 0));

    // SlotGrid
    private void numCols_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.Columns = (int)(e.NewValue ?? 0));
    private void numRows_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.Rows = (int)(e.NewValue ?? 0));
    private void numSlotSize_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.SlotSize = (int)(e.NewValue ?? 32));
    private void numSlotPad_ValueChanged(object? s, NumericUpDownValueChangedEventArgs e)
        => ApplyConfig(el => el.SlotPadding = (int)(e.NewValue ?? 0));

    // ─── CRUD ────────────────────────────────────────────────────────────────

    private void butAdd_Click(object? s, RoutedEventArgs e)
    {
        if (_currentConfig == null || cmbScreens.SelectedItem == null) return;
        var screenName = cmbScreens.SelectedItem.ToString() ?? "";
        var screen = _currentConfig.Screens.FirstOrDefault(s => s.Name == screenName);
        if (screen == null) return;

        var el = new Element { Name = "NewElement", Type = "Panel", X = 0, Y = 0, Width = 100, Height = 100 };
        screen.Elements.Add(el);
        _selectedNode = null;
        SelectScreen();
    }

    private void butRemove_Click(object? s, RoutedEventArgs e)
    {
        if (_selectedNode?.ConfigElement == null || _currentConfig == null || cmbScreens.SelectedItem == null) return;
        var screenName = cmbScreens.SelectedItem.ToString() ?? "";
        var screen = _currentConfig.Screens.FirstOrDefault(s => s.Name == screenName);
        if (screen == null) return;

        RemoveElement(screen.Elements, _selectedNode.ConfigElement);
        _selectedNode = null;
        SelectScreen();
    }

    private static bool RemoveElement(List<Element> list, Element target)
    {
        if (list.Remove(target)) return true;
        foreach (var el in list)
            if (RemoveElement(el.Children, target)) return true;
        return false;
    }

    private void butUp_Click(object? s, RoutedEventArgs e) { Reorder(-1); }
    private void butDown_Click(object? s, RoutedEventArgs e) { Reorder(1); }

    private void Reorder(int dir)
    {
        if (_selectedNode?.ConfigElement == null || _currentConfig == null || cmbScreens.SelectedItem == null) return;
        var screenName = cmbScreens.SelectedItem.ToString() ?? "";
        var screen = _currentConfig.Screens.FirstOrDefault(s => s.Name == screenName);
        if (screen == null) return;

        if (ReorderInList(screen.Elements, _selectedNode.ConfigElement, dir))
        {
            SelectScreen();
        }
    }

    private static bool ReorderInList(List<Element> list, Element target, int dir)
    {
        var idx = list.IndexOf(target);
        if (idx >= 0)
        {
            var newIdx = idx + dir;
            if (newIdx < 0 || newIdx >= list.Count) return false;
            list.RemoveAt(idx);
            list.Insert(newIdx, target);
            return true;
        }
        foreach (var el in list)
            if (ReorderInList(el.Children, target, dir)) return true;
        return false;
    }

    // ─── Save ────────────────────────────────────────────────────────────────

    private void butSave_Click(object? s, RoutedEventArgs e)
    {
        if (_currentConfig == null || string.IsNullOrEmpty(_configPath)) return;
        SaveToCurrentConfig();
        var json = JsonSerializer.Serialize(_currentConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configPath, json);
        UpdateRawJson();
    }

    private void SaveToCurrentConfig()
    {
        if (_selectedNode?.ConfigElement == null || _selectedNode?.Entity == null) return;
        var el = _selectedNode.ConfigElement;
        el.X = _selectedNode.Entity.Offset.X.GetValueInPixels(800);
        el.Y = _selectedNode.Entity.Offset.Y.GetValueInPixels(608);
        el.Width = _selectedNode.Entity.Size.X.GetValueInPixels(800);
        el.Height = _selectedNode.Entity.Size.Y.GetValueInPixels(608);
    }
}
