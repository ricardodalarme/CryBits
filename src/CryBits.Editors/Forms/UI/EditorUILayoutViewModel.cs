using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Client.Framework.Persistence.Repositories;
using Myra.Graphics2D.UI;
using System.Collections.ObjectModel;

namespace CryBits.Editors.Forms.UI;

internal sealed partial class EditorUILayoutViewModel : ObservableObject
{
    private Project? _currentProject;
    private WidgetNode? _selectedNode;
    private string _themeDir = string.Empty;

    public event Action? RequestRefresh;

    public ObservableCollection<string> ScreenNames { get; } = [];

    public Project? CurrentProject
    {
        get => _currentProject;
        set
        {
            _currentProject = value;
            OnPropertyChanged();
        }
    }

    public WidgetNode? SelectedNode
    {
        get => _selectedNode;
        set
        {
            _selectedNode = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty] private string? _selectedScreen;

    public void Load(string themeDir)
    {
        _themeDir = themeDir;
        SelectedNode = null;

        ScreenNames.Clear();
        ScreenNames.Add("Menu");
        ScreenNames.Add("Game");

        if (ScreenNames.Count > 0)
            SelectedScreen = ScreenNames[0];
    }

    partial void OnSelectedScreenChanged(string? value)
    {
        if (value != null)
        {
            var path = Path.Combine(_themeDir, $"{value}.xmmp");
            if (File.Exists(path))
            {
                CurrentProject = MmlRepository.Load(path);
            }
            else
            {
                CurrentProject = null;
            }
            RequestRefresh?.Invoke();
        }
    }

    [RelayCommand]
    private void Remove()
    {
        if (_selectedNode?.Widget == null || _currentProject?.Root is not Container container) return;
        container.Widgets.Remove(_selectedNode.Widget);
        SelectedNode = null;
        RequestRefresh?.Invoke();
    }

    [RelayCommand]
    private void MoveUp()
    {
        if (_selectedNode?.Widget == null || _currentProject?.Root is not Container container) return;
        var idx = container.Widgets.IndexOf(_selectedNode.Widget);
        if (idx > 0)
        {
            container.Widgets.RemoveAt(idx);
            container.Widgets.Insert(idx - 1, _selectedNode.Widget);
            RequestRefresh?.Invoke();
        }
    }

    [RelayCommand]
    private void MoveDown()
    {
        if (_selectedNode?.Widget == null || _currentProject?.Root is not Container container) return;
        var idx = container.Widgets.IndexOf(_selectedNode.Widget);
        if (idx >= 0 && idx < container.Widgets.Count - 1)
        {
            container.Widgets.RemoveAt(idx);
            container.Widgets.Insert(idx + 1, _selectedNode.Widget);
            RequestRefresh?.Invoke();
        }
    }

    [RelayCommand]
    private void Save()
    {
        if (SelectedScreen != null && CurrentProject != null)
        {
            var path = Path.Combine(_themeDir, $"{SelectedScreen}.xmmp");
            MmlRepository.Save(path, CurrentProject);
        }
    }
}
