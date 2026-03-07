using System;
using System.Collections.Generic;
using System.Linq;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Entities.Shop;

namespace CryBits.Editors.ViewModels;

internal sealed class EditorShopsViewModel : ViewModelBase
{
    private string _filter = string.Empty;
    private Shop? _selected;

    public string Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
                OnPropertyChanged(nameof(FilteredShops));
        }
    }

    public Shop? Selected
    {
        get => _selected;
        set
        {
            if (SetProperty(ref _selected, value))
                OnPropertyChanged(nameof(HasSelection));
        }
    }

    public bool HasSelection => _selected != null;

    public IReadOnlyList<Shop> FilteredShops =>
        Shop.List.Values
            .Where(s => s.Name.StartsWith(_filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

    /// <summary>Validates that at least one item exists before opening the editor.</summary>
    public static bool CanOpen() => Item.List.Count > 0;

    public Shop Add()
    {
        var shop = new Shop();
        Shop.List.Add(shop.Id, shop);
        OnPropertyChanged(nameof(FilteredShops));
        return shop;
    }

    public void Remove()
    {
        if (_selected == null) return;
        Shop.List.Remove(_selected.Id);
        _selected = null;
        OnPropertyChanged(nameof(Selected));
        OnPropertyChanged(nameof(HasSelection));
        OnPropertyChanged(nameof(FilteredShops));
    }

    public void RefreshFilteredShops() => OnPropertyChanged(nameof(FilteredShops));

    public void SaveAll() => PackageSender.WriteShops();

    public void Cancel() => PackageSender.RequestShops();
}
