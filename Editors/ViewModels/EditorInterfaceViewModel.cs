using System;
using System.Collections.ObjectModel;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Editors.Entities;
using CryBits.Editors.Library.Repositories;
using Button = CryBits.Client.Framework.Interfacily.Components.Button;
using CheckBox = CryBits.Client.Framework.Interfacily.Components.CheckBox;
using Component = CryBits.Client.Framework.Interfacily.Components.Component;
using Label = CryBits.Client.Framework.Interfacily.Components.Label;
using Panel = CryBits.Client.Framework.Interfacily.Components.Panel;
using Picture = CryBits.Client.Framework.Interfacily.Components.Picture;
using ProgressBar = CryBits.Client.Framework.Interfacily.Components.ProgressBar;
using SlotGrid = CryBits.Client.Framework.Interfacily.Components.SlotGrid;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Editors.ViewModels;

internal sealed class EditorInterfaceViewModel : ViewModelBase
{
    private bool _loadingProps;
    private Component? _selectedComponent;
    private TreeItemVM? _selectedNode;
    private TreeItemVM _rootVM = new();

    /// <summary>
    /// Currently selected window index. Consumed by <c>Renders.Interface()</c>.
    /// Accessed exclusively on the UI thread.
    /// </summary>
    public static byte SelectedWindowIndex { get; private set; }

    public bool LoadingProps => _loadingProps;

    public Component? SelectedComponent => _selectedComponent;

    public TreeItemVM? SelectedNode => _selectedNode;

    public ObservableCollection<TreeItemVM> RootChildren => _rootVM.Children;

    public void SetSelectedWindowIndex(int index)
    {
        SelectedWindowIndex = (byte)Math.Max(0, index);
        _selectedNode = null;
        _selectedComponent = null;
        RebuildTree();
        OnPropertyChanged(nameof(SelectedComponent));
        OnPropertyChanged(nameof(SelectedNode));
    }

    private static TreeItemVM BuildVM(InterfaceNode node, TreeItemVM? parent)
    {
        var vm = new TreeItemVM
        {
            Header = node.Text,
            Tag = node.Tag as Component,
            SourceNode = node,
            Parent = parent
        };
        foreach (var child in node.Nodes)
            vm.Children.Add(BuildVM(child, vm));
        return vm;
    }

    public void RebuildTree()
    {
        if (InterfaceData.Tree.Nodes.Count == 0 || SelectedWindowIndex >= InterfaceData.Tree.Nodes.Count) return;

        var sourceRoot = InterfaceData.Tree.Nodes[SelectedWindowIndex];
        _rootVM = BuildVM(sourceRoot, null);
        OnPropertyChanged(nameof(RootChildren));
    }

    public void SelectNode(TreeItemVM? node)
    {
        _selectedNode = node;
        _selectedComponent = node?.Tag;
        OnPropertyChanged(nameof(SelectedNode));
        OnPropertyChanged(nameof(SelectedComponent));
    }

    public TreeItemVM? AddComponent(int toolTypeIndex)
    {
        Component newComp = toolTypeIndex switch
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

        var winNode = InterfaceData.Tree.Nodes[SelectedWindowIndex];
        var newTreeNode = new InterfaceNode(newComp.ToString()) { Tag = newComp };
        winNode.Nodes.Add(newTreeNode);

        var newVM = new TreeItemVM
        {
            Header = newComp.ToString(),
            Tag = newComp,
            SourceNode = newTreeNode,
            Parent = _rootVM
        };
        _rootVM.Children.Add(newVM);

        return newVM;
    }

    public void RemoveSelected()
    {
        if (_selectedNode?.Parent == null) return;
        _selectedNode.SourceNode?.Parent?.Nodes.Remove(_selectedNode.SourceNode);
        _selectedNode.Parent.Children.Remove(_selectedNode);
        _selectedNode = null;
        _selectedComponent = null;
        OnPropertyChanged(nameof(SelectedNode));
        OnPropertyChanged(nameof(SelectedComponent));
    }

    public void PinSelected()
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

    public void UnpinSelected()
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

    public void MoveSelectedUp()
    {
        if (_selectedNode?.Parent == null) return;
        var parent = _selectedNode.Parent;
        var idx = parent.Children.IndexOf(_selectedNode);
        if (idx <= 0) return;
        parent.Children.Move(idx, idx - 1);
    }

    public void MoveSelectedDown()
    {
        if (_selectedNode?.Parent == null) return;
        var parent = _selectedNode.Parent;
        var idx = parent.Children.IndexOf(_selectedNode);
        if (idx >= parent.Children.Count - 1) return;
        parent.Children.Move(idx, idx + 1);
    }

    public void BeginLoadProps() => _loadingProps = true;
    public void EndLoadProps() => _loadingProps = false;

    public bool CanEditProps => !_loadingProps && _selectedComponent != null;

    public void UpdateSelectedNodeHeader()
    {
        if (_selectedNode != null && _selectedComponent != null)
            _selectedNode.Header = _selectedComponent.ToString() ?? string.Empty;
    }

    public static void Save() => ToolsRepository.Write();
}
