using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Entities;
using CryBits.Editors.Graphics.Renderers;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Button = CryBits.Client.Framework.Interfacily.Components.Button;
using CheckBox = CryBits.Client.Framework.Interfacily.Components.CheckBox;
using Component = CryBits.Client.Framework.Interfacily.Components.Component;
using Label = CryBits.Client.Framework.Interfacily.Components.Label;
using Panel = CryBits.Client.Framework.Interfacily.Components.Panel;
using Picture = CryBits.Client.Framework.Interfacily.Components.Picture;
using Point = System.Drawing.Point;
using ProgressBar = CryBits.Client.Framework.Interfacily.Components.ProgressBar;
using Screen = CryBits.Client.Framework.Interfacily.Components.Screen;
using SlotGrid = CryBits.Client.Framework.Interfacily.Components.SlotGrid;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Editors.Forms;

// ─── ViewModel for the order tree ───────────────────────────────────────────
internal sealed class TreeItemVM : INotifyPropertyChanged
{
    private string _header = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Header
    {
        get => _header;
        set
        {
            _header = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Header)));
        }
    }

    public Component? Tag { get; set; }
    public InterfaceNode? SourceNode { get; set; }
    public TreeItemVM? Parent { get; set; }
    public ObservableCollection<TreeItemVM> Children { get; } = [];
    public override string ToString() => _header;
}

// ─── Editor window ───────────────────────────────────────────────────────────
internal partial class EditorInterfaceWindow : Window
{
    /// <summary>Opens the Interface editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorInterfaceWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    // Consumed by Renders.Instance.Interface()
    public static byte SelectedWindowIndex { get; private set; }

    private bool _loadingProps;
    private Component? _selectedComponent;
    private TreeItemVM? _selectedNode;
    private TreeItemVM _rootVM = new(); // virtual root for the current window

    private WriteableBitmap? _previewBitmap;
    private readonly DispatcherTimer? _timer;

    public EditorInterfaceWindow()
    {
        InitializeComponent();

        // Populate window combo from tree
        foreach (var node in InterfaceData.Instance.Tree.Nodes)
            cmbWindows.Items.Add(node.Text);

        if (cmbWindows.Items.Count > 0)
            cmbWindows.SelectedIndex = 0;

        // Create offscreen SFML render target (933 × 702 to match the WinForms canvas)
        InterfaceRenderer.Instance.WinInterface = new RenderTexture(new Vector2u(933, 702));

        // Start refresh timer
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        InterfaceRenderer.Instance.WinInterface = null;
        base.OnClosed(e);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // SFML render tick → WriteableBitmap
    // ──────────────────────────────────────────────────────────────────────────

    private void OnRenderTick(object? sender, EventArgs e)
    {
        if (InterfaceRenderer.Instance.WinInterface == null) return;
        if (InterfaceData.Instance.Tree.Nodes.Count == 0) return;

        InterfaceRenderer.Instance.Interface(InterfaceData.Instance.Tree.Nodes[SelectedWindowIndex]);
        SfmlRenderBlit.Blit(InterfaceRenderer.Instance.WinInterface, ref _previewBitmap, imgPreview);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Window combo
    // ──────────────────────────────────────────────────────────────────────────

    private void cmbWindows_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        SelectedWindowIndex = (byte)Math.Max(0, cmbWindows.SelectedIndex);
        _selectedNode = null;
        _selectedComponent = null;
        RebuildTree();
        UpdatePropertiesPanel();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Tree management
    // ──────────────────────────────────────────────────────────────────────────

    private static TreeItemVM BuildVM(InterfaceNode node, TreeItemVM? parent)
    {
        var vm = new TreeItemVM { Header = node.Text, Tag = node.Tag as Component, SourceNode = node, Parent = parent };
        foreach (var child in node.Nodes)
            vm.Children.Add(BuildVM(child, vm));
        return vm;
    }

    private void RebuildTree()
    {
        if (InterfaceData.Instance.Tree.Nodes.Count == 0 || SelectedWindowIndex >= InterfaceData.Instance.Tree.Nodes.Count) return;

        var sourceRoot = InterfaceData.Instance.Tree.Nodes[SelectedWindowIndex];
        _rootVM = BuildVM(sourceRoot, null);
        treOrder.ItemsSource = _rootVM.Children;
    }

    private void treOrder_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Use e.AddedItems — treOrder.SelectedItem may still be null at event time
        if (e.AddedItems.Count > 0)
            _selectedNode = e.AddedItems[0] as TreeItemVM;
        _selectedComponent = _selectedNode?.Tag;
        UpdatePropertiesPanel();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // New / Remove / Pin / Unpin / Up / Down
    // ──────────────────────────────────────────────────────────────────────────

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        cmbType.SelectedIndex = 0;
        pnlNew.IsVisible = true;
    }

    private void butCancelNew_Click(object? sender, RoutedEventArgs e)
    {
        pnlNew.IsVisible = false;
    }

    private void butConfirm_Click(object? sender, RoutedEventArgs e)
    {
        Component newComp = cmbType.SelectedIndex switch
        {
            (int)ToolType.Label => new Label(),
            (int)ToolType.Button => new Button(),
            (int)ToolType.Panel => new Panel(),
            (int)ToolType.CheckBox => new CheckBox(),
            (int)ToolType.TextBox => new TextBox(),
            (int)ToolType.ProgressBar => new ProgressBar(),
            (int)ToolType.SlotGrid => new SlotGrid(),
            (int)ToolType.Picture => new Picture(),
            _ => new Button()
        };
        newComp.Visible = true;

        // Add to canonical InterfaceNode tree
        var winNode = InterfaceData.Instance.Tree.Nodes[SelectedWindowIndex];
        var newTreeNode = new InterfaceNode(newComp.ToString()) { Tag = newComp };
        winNode.Nodes.Add(newTreeNode);

        // Add to VM tree
        var newVM = new TreeItemVM
        { Header = newComp.ToString(), Tag = newComp, SourceNode = newTreeNode, Parent = _rootVM };
        _rootVM.Children.Add(newVM);

        pnlNew.IsVisible = false;
        treOrder.SelectedItem = newVM;
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedNode?.Parent == null) return;

        // Sync to InterfaceNode tree
        _selectedNode.SourceNode?.Parent?.Nodes.Remove(_selectedNode.SourceNode);

        // Remove from VM
        _selectedNode.Parent.Children.Remove(_selectedNode);
        _selectedNode = null;
        _selectedComponent = null;
        UpdatePropertiesPanel();
    }

    private void butOrderPin_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedNode?.Parent == null) return;
        var parent = _selectedNode.Parent;
        var idx = parent.Children.IndexOf(_selectedNode);
        if (idx <= 0) return;

        var prevSibling = parent.Children[idx - 1];
        parent.Children.RemoveAt(idx);
        _selectedNode.Parent = prevSibling;
        prevSibling.Children.Add(_selectedNode);
    }

    private void butOrderUnpin_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedNode?.Parent == null) return;
        var parent = _selectedNode.Parent;
        var grandparent = parent.Parent;
        if (grandparent == null) return;

        var parentIdx = grandparent.Children.IndexOf(parent);
        parent.Children.Remove(_selectedNode);
        _selectedNode.Parent = grandparent;
        grandparent.Children.Insert(parentIdx + 1, _selectedNode);
    }

    private void butOrderUp_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedNode?.Parent == null) return;
        var parent = _selectedNode.Parent;
        var idx = parent.Children.IndexOf(_selectedNode);
        if (idx <= 0) return;
        parent.Children.Move(idx, idx - 1);
    }

    private void butOrderDown_Click(object? sender, RoutedEventArgs e)
    {
        if (_selectedNode?.Parent == null) return;
        var parent = _selectedNode.Parent;
        var idx = parent.Children.IndexOf(_selectedNode);
        if (idx >= parent.Children.Count - 1) return;
        parent.Children.Move(idx, idx + 1);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Properties panel
    // ──────────────────────────────────────────────────────────────────────────

    private void UpdatePropertiesPanel()
    {
        var c = _selectedComponent;
        if (c == null)
        {
            pnlPropsBase.IsVisible = false;
            lblNoSelection.IsVisible = true;
            return;
        }

        _loadingProps = true;
        pnlPropsBase.IsVisible = true;
        lblNoSelection.IsVisible = false;

        txtPropName.Text = c.Name;
        numPropX.Value = c.Position.X;
        numPropY.Value = c.Position.Y;
        chkPropVisible.IsChecked = c.Visible;

        secTextureNum.IsVisible = c is Button or Panel;
        secLabelProps.IsVisible = c is Label;
        secCheckBoxProps.IsVisible = c is CheckBox;
        secTextBoxProps.IsVisible = c is TextBox;
        secProgressBarProps.IsVisible = c is ProgressBar;
        secSlotGridProps.IsVisible = c is SlotGrid;
        secPictureProps.IsVisible = c is Picture;

        switch (c)
        {
            case Button btn: numPropTexture.Value = btn.TextureNum; break;
            case Panel pnl: numPropTexture.Value = pnl.TextureNum; break;
            case Label lbl:
                txtPropLblText.Text = lbl.Text;
                cmbPropLblAlignment.SelectedIndex = (int)lbl.Alignment;
                clrPropLabel.Color = Avalonia.Media.Color.FromRgb((byte)(lbl.Color >> 16), (byte)(lbl.Color >> 8), (byte)lbl.Color);
                numPropLblMaxWidth.Value = lbl.MaxWidth;
                break;
            case CheckBox cb:
                txtPropCbText.Text = cb.Text;
                chkPropCbChecked.IsChecked = cb.Checked;
                break;
            case TextBox tb:
                txtPropTbText.Text = tb.Text;
                numPropMaxChars.Value = tb.MaxCharacters;
                numPropTbWidth.Value = tb.Width;
                chkPropPassword.IsChecked = tb.Password;
                break;
            case ProgressBar pb:
                numPropPbSourceY.Value = pb.SourceY;
                numPropPbWidth.Value = pb.Width;
                numPropPbHeight.Value = pb.Height;
                break;
            case SlotGrid sg:
                numPropSgRows.Value = sg.Rows;
                numPropSgColumns.Value = sg.Columns;
                numPropSgSlotSize.Value = sg.SlotSize;
                numPropSgPadding.Value = sg.Padding;
                txtSgSlotCount.Text = sg.SlotCount.ToString();
                break;
            case Picture pic:
                numPropPicWidth.Value = pic.Width;
                numPropPicHeight.Value = pic.Height;
                break;
        }

        _loadingProps = false;
    }

    // ─── Property write-back ──────────────────────────────────────────────────

    private void txtPropName_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent == null) return;
        _selectedComponent.Name = txtPropName.Text ?? string.Empty;
        // INotifyPropertyChanged on TreeItemVM.Header propagates the change to the TreeView automatically
        if (_selectedNode != null)
            _selectedNode.Header = _selectedComponent.ToString() ?? string.Empty;
    }

    private void numPropX_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent == null) return;
        _selectedComponent.Position = new Point((int)(e.NewValue ?? 0), _selectedComponent.Position.Y);
    }

    private void numPropY_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent == null) return;
        _selectedComponent.Position = new Point(_selectedComponent.Position.X, (int)(e.NewValue ?? 0));
    }

    private void chkPropVisible_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_loadingProps || _selectedComponent == null) return;
        _selectedComponent.Visible = chkPropVisible.IsChecked ?? false;
    }

    private void numPropTexture_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent == null) return;
        var v = (byte)(e.NewValue ?? 0);
        if (_selectedComponent is Button btn) btn.TextureNum = v;
        else if (_selectedComponent is Panel pnl) pnl.TextureNum = v;
    }

    private void txtPropLblText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not Label lbl) return;
        lbl.Text = txtPropLblText.Text ?? string.Empty;
    }

    private void cmbPropLblAlignment_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not Label lbl) return;
        lbl.Alignment = (TextAlign)(cmbPropLblAlignment.SelectedIndex >= 0 ? cmbPropLblAlignment.SelectedIndex : 0);
    }

    private void clrPropLabel_ColorChanged(object? sender, Avalonia.Controls.ColorChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not Label lbl) return;
        var c = e.NewColor;
        lbl.Color = (c.R << 16) | (c.G << 8) | c.B;
    }

    private void numPropLblMaxWidth_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not Label lbl) return;
        lbl.MaxWidth = (int)(e.NewValue ?? 0);
    }

    private void txtPropCbText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not CheckBox cb) return;
        cb.Text = txtPropCbText.Text ?? string.Empty;
    }

    private void chkPropCbChecked_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not CheckBox cb) return;
        cb.Checked = chkPropCbChecked.IsChecked ?? false;
    }

    private void txtPropTbText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not TextBox tb) return;
        tb.Text = txtPropTbText.Text ?? string.Empty;
    }

    private void numPropMaxChars_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not TextBox tb) return;
        tb.MaxCharacters = (short)(e.NewValue ?? 0);
    }

    private void numPropTbWidth_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not TextBox tb) return;
        tb.Width = (short)(e.NewValue ?? 0);
    }

    private void chkPropPassword_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not TextBox tb) return;
        tb.Password = chkPropPassword.IsChecked ?? false;
    }

    private void numPropPbSourceY_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not ProgressBar pb) return;
        pb.SourceY = (int)(e.NewValue ?? 0);
    }

    private void numPropPbWidth_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not ProgressBar pb) return;
        pb.Width = (int)(e.NewValue ?? 0);
    }

    private void numPropPbHeight_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not ProgressBar pb) return;
        pb.Height = (int)(e.NewValue ?? 0);
    }

    private void numPropSgRows_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not SlotGrid sg) return;
        sg.Rows = (byte)(e.NewValue ?? 1);
        txtSgSlotCount.Text = sg.SlotCount.ToString();
    }

    private void numPropSgColumns_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not SlotGrid sg) return;
        sg.Columns = (byte)(e.NewValue ?? 1);
        txtSgSlotCount.Text = sg.SlotCount.ToString();
    }

    private void numPropSgSlotSize_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not SlotGrid sg) return;
        sg.SlotSize = (byte)(e.NewValue ?? 32);
    }

    private void numPropSgPadding_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not SlotGrid sg) return;
        sg.Padding = (byte)(e.NewValue ?? 4);
    }

    private void numPropPicWidth_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not Picture pic) return;
        pic.Width = (int)(e.NewValue ?? 0);
    }

    private void numPropPicHeight_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loadingProps || _selectedComponent is not Picture pic) return;
        pic.Height = (int)(e.NewValue ?? 0);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Save / Cancel
    // ──────────────────────────────────────────────────────────────────────────

    private void butSaveAll_Click(object? sender, RoutedEventArgs e)
    {
        // Rebuild Screen.Body / component.Children from the authoritative InterfaceNode tree
        // before serializing, since editor operations mutate the InterfaceNode tree directly.
        foreach (var screenNode in InterfaceData.Instance.Tree.Nodes)
        {
            var screen = (Screen)screenNode.Tag!;
            screen.Body.Clear();
            SyncBodyFromNode(screenNode, screen.Body);
        }

        ToolsRepository.Instance.Write();
        Close();
    }

    private static void SyncBodyFromNode(InterfaceNode node, List<Component> body)
    {
        foreach (var childNode in node.Nodes)
        {
            var comp = (Component)childNode.Tag!;
            comp.Children.Clear();
            SyncBodyFromNode(childNode, comp.Children);
            body.Add(comp);
        }
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
