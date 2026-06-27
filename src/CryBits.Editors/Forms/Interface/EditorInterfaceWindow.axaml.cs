using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Entities;
using CryBits.Editors.Graphics.Renderers;
using SFML.Graphics;
using SFML.System;
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

namespace CryBits.Editors.Forms.Interface;

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

    private Component? _selectedComponent;
    private ComponentEditorViewModel? _componentViewModel;
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
        _componentViewModel = null;
        DataContext = null;
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
        var newTreeNode = new InterfaceNode(newComp.ToString() ?? string.Empty) { Tag = newComp };
        winNode.Nodes.Add(newTreeNode);

        // Add to VM tree
        var newVM = new TreeItemVM
        { Header = newComp.ToString() ?? string.Empty, Tag = newComp, SourceNode = newTreeNode, Parent = _rootVM };
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
        _componentViewModel = null;
        DataContext = null;
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

        pnlPropsBase.IsVisible = true;
        lblNoSelection.IsVisible = false;

        _componentViewModel = new ComponentEditorViewModel(c);
        DataContext = _componentViewModel;

        // Update Header in tree when Name changes
        if (_selectedNode != null)
            _selectedNode.Header = c.ToString() ?? string.Empty;

        // ColorPicker needs manual handling (Avalonia.Media.Color ↔ int conversion)
        if (c is Label lbl)
        {
            clrPropLabel.Color = Avalonia.Media.Color.FromRgb(
                (byte)(lbl.Color >> 16), (byte)(lbl.Color >> 8), (byte)lbl.Color);
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Remaining property handlers (can't be bound)
    // ──────────────────────────────────────────────────────────────────────────

    private void clrPropLabel_ColorChanged(object? sender, Avalonia.Controls.ColorChangedEventArgs e)
    {
        if (_selectedComponent is not Label lbl) return;
        var c = e.NewColor;
        lbl.Color = (c.R << 16) | (c.G << 8) | c.B;
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
