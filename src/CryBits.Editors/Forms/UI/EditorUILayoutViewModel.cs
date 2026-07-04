using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Client.Framework.Persistence.Dtos;
using CryBits.Client.Framework.Persistence.Repositories;
using System.Collections.ObjectModel;

namespace CryBits.Editors.Forms.UI;

internal sealed partial class EditorUILayoutViewModel : ObservableObject
{
    private UILayout? _currentLayout;
    private EntityNode? _selectedNode;
    private string _configPath = string.Empty;

    public event Action? RequestRefresh;
    public event Action? RequestOpenTheme;

    public ObservableCollection<string> ScreenNames { get; } = [];

    public UILayout? CurrentLayout
    {
        get => _currentLayout;
        set { _currentLayout = value; OnPropertyChanged(); }
    }

    public EntityNode? SelectedNode
    {
        get => _selectedNode;
        set { _selectedNode = value; OnPropertyChanged(); }
    }

    [ObservableProperty]
    private string? _selectedScreen;

    public void Load(string themeDir)
    {
        var path = Path.Combine(themeDir, "Layout.json");
        if (!File.Exists(path)) return;
        _configPath = path;
        CurrentLayout = InterfaceRepository.Load(path);
        SelectedNode = null;

        ScreenNames.Clear();
        if (_currentLayout == null) return;
        foreach (var screen in _currentLayout.Screens)
            ScreenNames.Add(screen.Name);

        if (ScreenNames.Count > 0)
            SelectedScreen = ScreenNames[0];
    }

    partial void OnSelectedScreenChanged(string? value)
    {
        if (value != null)
            RequestRefresh?.Invoke();
    }

    public void AddElement(Element el)
    {
        if (_currentLayout == null) return;
        _currentLayout.Screens.FirstOrDefault()?.Children.Add(el);
        SelectedNode = null;
        RequestRefresh?.Invoke();
    }

    [RelayCommand]
    private void Remove()
    {
        if (_selectedNode?.ConfigElement == null || _currentLayout == null) return;
        var screen = _currentLayout.Screens.FirstOrDefault();
        if (screen == null) return;

        RemoveRecursive(screen.Children, _selectedNode.ConfigElement);
        SelectedNode = null;
        RequestRefresh?.Invoke();
    }

    [RelayCommand]
    private void MoveUp() => Reorder(-1);

    [RelayCommand]
    private void MoveDown() => Reorder(1);

    private void Reorder(int dir)
    {
        if (_selectedNode?.ConfigElement == null || _currentLayout == null) return;
        var screen = _currentLayout.Screens.FirstOrDefault();
        if (screen == null) return;

        if (ReorderInList(screen.Children, _selectedNode.ConfigElement, dir))
            RequestRefresh?.Invoke();
    }

    [RelayCommand]
    private void EditTheme() => RequestOpenTheme?.Invoke();

    [RelayCommand]
    private void Save()
    {
        if (_currentLayout == null || string.IsNullOrEmpty(_configPath)) return;
        InterfaceRepository.Save(_configPath, _currentLayout);
    }

    private static bool RemoveRecursive(List<Element> list, Element target)
    {
        if (list.Remove(target)) return true;
        return list.Any(el => RemoveRecursive(el.Children, target));
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

        return list.Any(el => ReorderInList(el.Children, target, dir));
    }
}
