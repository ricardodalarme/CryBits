using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Editors.Entities;
using CryBits.Editors.Graphics;
using CryBits.Editors.Library.Repositories;
using SFML.Graphics;
using SFML.System;
using Button = CryBits.Client.Framework.Interfacily.Components.Button;
using CheckBox = CryBits.Client.Framework.Interfacily.Components.CheckBox;
using Component = CryBits.Client.Framework.Interfacily.Components.Component;
using Panel = CryBits.Client.Framework.Interfacily.Components.Panel;
using Point = System.Drawing.Point;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Editors.AvaloniaUI.Forms;

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
    // Consumed by Renders.Interface()
    public static byte SelectedWindowIndex { get; private set; }

    private bool _loadingProps;
    private Component? _selectedComponent;
    private TreeItemVM? _selectedNode;
    private TreeItemVM _rootVM = new(); // virtual root for the current window

    private WriteableBitmap? _previewBitmap;
    private DispatcherTimer? _timer;

    public EditorInterfaceWindow()
    {
        InitializeComponent();

        // Populate window combo from tree
        foreach (var node in InterfaceData.Tree.Nodes)
            cmbWindows.Items.Add(node.Text);

        if (cmbWindows.Items.Count > 0)
            cmbWindows.SelectedIndex = 0;

        // Create offscreen SFML render target (933 × 702 to match the WinForms canvas)
        Renders.WinInterfaceRT = new RenderTexture(new Vector2u(933, 702));

        // Start refresh timer
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        Renders.WinInterfaceRT = null;
        base.OnClosed(e);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // SFML render tick → WriteableBitmap
    // ──────────────────────────────────────────────────────────────────────────

    private void OnRenderTick(object? sender, EventArgs e)
    {
        if (Renders.WinInterfaceRT == null) return;
        if (InterfaceData.Tree.Nodes.Count == 0) return;

        Renders.Interface();
        SfmlRenderBlit.Blit(Renders.WinInterfaceRT, ref _previewBitmap, imgPreview);
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
        if (InterfaceData.Tree.Nodes.Count == 0 || SelectedWindowIndex >= InterfaceData.Tree.Nodes.Count) return;

        var sourceRoot = InterfaceData.Tree.Nodes[SelectedWindowIndex];
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
            (int)ToolType.Button => new Button(),
            (int)ToolType.Panel => new Panel(),
            (int)ToolType.CheckBox => new CheckBox(),
            (int)ToolType.TextBox => new TextBox(),
            _ => new Button()
        };
        newComp.Visible = true;

        // Add to canonical InterfaceNode tree
        var winNode = InterfaceData.Tree.Nodes[SelectedWindowIndex];
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
        secCheckBoxProps.IsVisible = c is CheckBox;
        secTextBoxProps.IsVisible = c is TextBox;

        switch (c)
        {
            case Button btn: numPropTexture.Value = btn.TextureNum; break;
            case Panel pnl: numPropTexture.Value = pnl.TextureNum; break;
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

    // ──────────────────────────────────────────────────────────────────────────
    // Save / Cancel
    // ──────────────────────────────────────────────────────────────────────────

    private void butSaveAll_Click(object? sender, RoutedEventArgs e)
    {
        ToolsRepository.Write();
        Close();
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
