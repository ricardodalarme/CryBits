using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Editors.AvaloniaUI;
using TextChangedEventArgs = Avalonia.Controls.TextChangedEventArgs;

namespace CryBits.Editors.Forms.Maps.Panes;

internal partial class MapExplorerPane : UserControl
{
    public MapExplorerPane() => InitializeComponent();

    // ── Dependencies (set by EditorMapsWindow) ──────────────────────

    public DefinitionCatalog? Catalog { get; set; }

    // ── Public API ──────────────────────────────────────────────────

    /// <summary>Raised when the user selects a different map in the list.</summary>
    public event Action<Map>? MapSelected;

    /// <summary>Re-filters the list, optionally preserving a selection by ID.</summary>
    public void RefreshList(Guid? keepSelectionId = null)
    {
        var filter = txtFilter.Text ?? string.Empty;
        var filtered = Catalog?.Maps.Values
            .Where(m => m.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList() ?? [];

        lstMaps.ItemsSource = filtered;

        if (keepSelectionId.HasValue)
            lstMaps.SelectedItem = filtered.FirstOrDefault(m => m.Id == keepSelectionId.Value);
        if (lstMaps.SelectedItem == null && filtered.Count > 0)
            lstMaps.SelectedIndex = 0;
    }

    /// <summary>Syncs the list selection to match the given map (without firing MapSelected).</summary>
    public void SelectMap(Map map)
    {
        lstMaps.SelectedItem = Catalog?.Maps.Values.FirstOrDefault(m => m.Id == map.Id);
    }

    // ── Internal state ──────────────────────────────────────────────

    private Map? _selected;

    // ── Event wiring ────────────────────────────────────────────────

    public void WireHandlers()
    {
        lstMaps.SelectionChanged += OnSelectionChanged;
        txtFilter.TextChanged += OnFilterChanged;
        butNew.Click += OnNew;
        butRemove.Click += OnRemove;
    }

    // ── Handlers ────────────────────────────────────────────────────

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (lstMaps.SelectedItem is not Map map) return;
        _selected = map;
        MapSelected?.Invoke(map);
    }

    private void OnFilterChanged(object? sender, TextChangedEventArgs e) => RefreshList(_selected?.Id);

    private void OnNew(object? sender, RoutedEventArgs e)
    {
        var map = new Map();
        Catalog?.Maps.Add(map.Id, map);
        RefreshList(map.Id);
    }

    private void OnRemove(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        if (Catalog == null) return;
        if (Catalog.Maps.Count == 1)
        {
            MessageBox.Show("It must have at least one map registered.");
            return;
        }

        Catalog.Maps.Remove(_selected.Id);
        _selected = null;
        RefreshList();
    }
}
