using System;
using System.Collections.Generic;
using System.Linq;
using CryBits.Editors.Network;
using CryBits.Entities;

namespace CryBits.Editors.ViewModels;

internal sealed class EditorItemsViewModel : ViewModelBase
{
    private string _filter = string.Empty;
    private Item? _selected;
    private bool _loading;

    public string Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
                OnPropertyChanged(nameof(FilteredItems));
        }
    }

    public Item? Selected
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
    public short CurrentTextureIndex { get; set; }

    public IReadOnlyList<Item> FilteredItems =>
        Item.List.Values
            .Where(i => i.Name.StartsWith(_filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public void BeginLoad() => _loading = true;
    public void EndLoad() => _loading = false;

    /// <summary>Returns true when edits should be applied (not loading and an item is selected).</summary>
    public bool CanEdit => !_loading && _selected != null;

    public Item Add()
    {
        var item = new Item();
        Item.List.Add(item.Id, item);
        OnPropertyChanged(nameof(FilteredItems));
        return item;
    }

    public void Remove()
    {
        if (_selected == null) return;
        Item.List.Remove(_selected.Id);
        _selected = null;
        OnPropertyChanged(nameof(Selected));
        OnPropertyChanged(nameof(HasSelection));
        OnPropertyChanged(nameof(FilteredItems));
    }

    public void SaveAll() => PackageSender.WriteItems();

    public void Cancel() => PackageSender.RequestItems();
}
