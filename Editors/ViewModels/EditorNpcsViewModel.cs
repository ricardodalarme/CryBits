using System;
using System.Collections.Generic;
using System.Linq;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;

namespace CryBits.Editors.ViewModels;

internal sealed class EditorNpcsViewModel : ViewModelBase
{
    private string _filter = string.Empty;
    private Npc? _selected;
    private bool _loading;

    public string Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
                OnPropertyChanged(nameof(FilteredNpcs));
        }
    }

    public Npc? Selected
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

    /// <summary>
    /// Current texture index consumed by <c>Renders.EditorNpcRT()</c>.
    /// Accessed exclusively on the UI thread.
    /// </summary>
    public static short CurrentTextureIndex { get; set; }

    public IReadOnlyList<Npc> FilteredNpcs =>
        Npc.List.Values
            .Where(n => n.Name.StartsWith(_filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public void BeginLoad() => _loading = true;
    public void EndLoad() => _loading = false;

    /// <summary>Returns true when edits should be applied (not loading and an NPC is selected).</summary>
    public bool CanEdit => !_loading && _selected != null;

    public Npc Add()
    {
        var npc = new Npc();
        Npc.List.Add(npc.Id, npc);
        OnPropertyChanged(nameof(FilteredNpcs));
        return npc;
    }

    public void Remove()
    {
        if (_selected == null) return;
        Npc.List.Remove(_selected.Id);
        _selected = null;
        OnPropertyChanged(nameof(Selected));
        OnPropertyChanged(nameof(HasSelection));
        OnPropertyChanged(nameof(FilteredNpcs));
    }

    /// <summary>Validates the ShopKeeper behaviour change: requires at least one shop.</summary>
    public static bool CanAssignShopKeeper() => Shop.List.Count > 0;

    public void SaveAll() => PackageSender.WriteNpcs();

    public void Cancel() => PackageSender.RequestNpcs();
}
