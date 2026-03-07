using System;
using System.Collections.Generic;
using System.Linq;
using CryBits.Editors.Network;
using CryBits.Entities;
using Map = CryBits.Entities.Map.Map;

namespace CryBits.Editors.ViewModels;

internal sealed class EditorClassesViewModel : ViewModelBase
{
    private string _filter = string.Empty;
    private Class? _selected;
    private bool _loading;

    public string Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
                OnPropertyChanged(nameof(FilteredClasses));
        }
    }

    public Class? Selected
    {
        get => _selected;
        set
        {
            if (SetProperty(ref _selected, value))
                OnPropertyChanged(nameof(HasSelection));
        }
    }

    public bool HasSelection => _selected != null;

    public bool IsLoading => _loading;

    /// <summary>Current texture index, used by the texture preview.</summary>
    public short CurrentTextureIndex { get; set; } = 1;

    public IReadOnlyList<Class> FilteredClasses =>
        Class.List.Values
            .Where(c => c.Name.StartsWith(_filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public void BeginLoad() => _loading = true;
    public void EndLoad() => _loading = false;

    /// <summary>Returns true when edits should be applied (not loading and a class is selected).</summary>
    public bool CanEdit => !_loading && _selected != null;

    public Class Add()
    {
        var cls = new Class();
        Class.List.Add(cls.Id, cls);
        OnPropertyChanged(nameof(FilteredClasses));
        return cls;
    }

    public bool Remove()
    {
        if (_selected == null || Class.List.Count <= 1) return false;
        Class.List.Remove(_selected.Id);
        _selected = null;
        OnPropertyChanged(nameof(Selected));
        OnPropertyChanged(nameof(HasSelection));
        OnPropertyChanged(nameof(FilteredClasses));
        return true;
    }

    /// <summary>Validates that at least one map exists before opening the editor.</summary>
    public static bool CanOpen() => Map.List.Count > 0;

    public void SaveAll() => PackageSender.WriteClasses();

    public void Cancel() => PackageSender.RequestClasses();
}
