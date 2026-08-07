using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using CryBits.Client.Framework;
using CryBits.Definitions.Maps;
using CryBits.Editors.Forms.Classes;
using CryBits.Editors.Forms.Items;
using CryBits.Editors.Forms.Npcs;
using CryBits.Editors.Forms.Shops;
using CryBits.Editors.Forms.Tiles;
using CryBits.Editors.Forms.UI;
using CryBits.Editors.Network;
using CryBits.Editors.Utils;
using DefinitionsTileData = CryBits.Definitions.Maps.TileData;
using SystemRect = System.Drawing.Rectangle;

namespace CryBits.Editors.Forms.Maps.Panes;

// ── External dependencies supplied by EditorMapsWindow ─────────────

internal sealed record ToolbarDeps
{
    public required MapCanvasPane CanvasPane { get; init; }
    public required LayersPane LayersPane { get; init; }
    public required TileSheetPane TileSheetPane { get; init; }
    public required Func<Map?> GetSelectedMap { get; init; }
    public required Func<SystemRect> GetMapSelection { get; init; }
    public required Func<DefinitionsTileData> MakeSetTile { get; init; }
    public required Func<DefinitionsTileData[,]?> GetClipboard { get; init; }
    public required Action<DefinitionsTileData[,]> SetClipboard { get; init; }
    public required Action<int, int, DefinitionsTileData> PaintTile { get; init; }
    public required Action ResetMapSelectionSize { get; init; }
    public required Action<bool> SetShowAudio { get; init; }
    public required Action<string> SetLeftPanelMode { get; init; }
    public required Action PopulateNpcCombo { get; init; }
    public required Window? ParentWindow { get; init; }
}

// ── Toolbar Pane ───────────────────────────────────────────────────

internal partial class ToolbarPane : UserControl
{
    public ToolbarPane() => InitializeComponent();

    private ToolbarDeps? _deps;

    public void Attach(ToolbarDeps deps)
    {
        _deps = deps;
        WireHandlers();
    }

    // ── Event wiring ────────────────────────────────────────────────

    private void WireHandlers()
    {
        butSaveAll.Click += OnSaveAll;
        butReload.Click += OnReload;

        butMNormal.Click += OnModeNormal;
        butMAttributes.Click += OnModeAttributes;
        butMZones.Click += OnModeZones;
        butMNPCs.Click += OnModeNpcs;

        butCut.Click += OnCut;
        butCopy.Click += OnCopy;
        butPaste.Click += OnPaste;

        butPencil.Click += OnPencil;
        butRectangle.Click += OnRectangle;
        butArea.Click += OnArea;
        butDiscover.Click += OnDiscover;

        butFill.Click += OnFill;
        butEraser.Click += OnEraser;

        butZoomReset.Click += (_, _) => _deps?.CanvasPane.ZoomBorder.ResetMatrix();
        butZoomFit.Click += (_, _) => _deps?.CanvasPane.ZoomBorder.AutoFit();

        butVisualization.Click += OnVisualization;
        butEdition.Click += OnEdition;
        butAudio.Click += OnAudio;

        butEditors_Classes.Click += (_, _) => EditorClassesWindow.Open(_deps!.ParentWindow!);
        butEditors_Interface.Click += (_, _) => EditorUILayoutWindow.Open(_deps!.ParentWindow!);
        butEditors_Items.Click += (_, _) => EditorItemsWindow.Open(_deps!.ParentWindow!);
        butEditors_NPCs.Click += (_, _) => EditorNpcsWindow.Open(_deps!.ParentWindow!);
        butEditors_Shops.Click += (_, _) => EditorShopsWindow.Open(_deps!.ParentWindow!);
        butEditors_Tiles.Click += (_, _) => EditorTilesWindow.Open(_deps!.ParentWindow!);
    }

    // ── Mode / tool helpers ─────────────────────────────────────────

    public bool ModeNormal => butMNormal.IsChecked == true;
    public bool ModeAttributes => butMAttributes.IsChecked == true;
    public bool ModeNPCs => butMNPCs.IsChecked == true;

    public bool ToolPencil => butPencil.IsChecked == true;
    public bool ToolRectangle => butRectangle.IsChecked == true;
    public bool ToolArea => butArea.IsChecked == true;
    public bool ToolDiscover => butDiscover.IsChecked == true;

    public ToggleButton ToolRectangleButton => butRectangle;
    public ToggleButton ToolAreaButton => butArea;

    public bool ShowGrid => butGrid.IsChecked == true;
    public bool ShowAudio => butAudio.IsChecked == true;
    public bool ShowVisualization => butVisualization.IsChecked == true;

    // ── File handlers ───────────────────────────────────────────────

    private void OnSaveAll(object? sender, RoutedEventArgs e)
    {
        PackageSender.Instance!.WriteMaps();
        MessageBox.Show("All maps has been saved");
    }

    private void OnReload(object? sender, RoutedEventArgs e)
    {
        var map = _deps?.GetSelectedMap();
        if (map == null) return;
        PackageSender.Instance!.RequestMap(map);
    }

    // ── Mode handlers ───────────────────────────────────────────────

    private void OnModeNormal(object? sender, RoutedEventArgs e)
    {
        ModesExclusive(butMNormal);
        _deps?.ResetMapSelectionSize();
    }

    private void OnModeAttributes(object? sender, RoutedEventArgs e) { ModesExclusive(butMAttributes); }
    private void OnModeZones(object? sender, RoutedEventArgs e) { ModesExclusive(butMZones); }

    private void OnModeNpcs(object? sender, RoutedEventArgs e)
    {
        ModesExclusive(butMNPCs);
        if (butMNPCs.IsChecked == true)
            _deps?.PopulateNpcCombo();
    }

    private void ModesExclusive(ToggleButton pressed)
    {
        foreach (var btn in new[] { butMNormal, butMZones, butMAttributes, butMNPCs })
            btn.IsChecked = btn == pressed;

        if (_deps != null)
        {
            if (butMZones.IsChecked == true) _deps.SetLeftPanelMode("Zones");
            else if (butMAttributes.IsChecked == true) _deps.SetLeftPanelMode("Attributes");
            else if (butMNPCs.IsChecked == true) _deps.SetLeftPanelMode("NPCs");
            else _deps.SetLeftPanelMode("Normal");
        }
    }

    // ── Draw tool handlers ──────────────────────────────────────────

    private void OnPencil(object? sender, RoutedEventArgs e)
    {
        if (butPencil.IsChecked == true)
        {
            butRectangle.IsChecked = false;
            butArea.IsChecked = false;
            butDiscover.IsChecked = false;
        }
        else
        {
            butPencil.IsChecked = true;
        }

        _deps?.ResetMapSelectionSize();
    }

    private void OnRectangle(object? sender, RoutedEventArgs e)
    {
        if (butRectangle.IsChecked == true)
        {
            butPencil.IsChecked = false;
            butArea.IsChecked = false;
            butDiscover.IsChecked = false;
        }
        else
        {
            butRectangle.IsChecked = true;
        }

        _deps?.ResetMapSelectionSize();
    }

    private void OnArea(object? sender, RoutedEventArgs e)
    {
        if (butArea.IsChecked == true)
        {
            butPencil.IsChecked = false;
            butRectangle.IsChecked = false;
            butDiscover.IsChecked = false;
        }
        else
        {
            butArea.IsChecked = true;
        }

        _deps?.ResetMapSelectionSize();
    }

    private void OnDiscover(object? sender, RoutedEventArgs e)
    {
        if (butDiscover.IsChecked == true)
        {
            butPencil.IsChecked = false;
            butRectangle.IsChecked = false;
            butArea.IsChecked = false;
        }
        else
        {
            butDiscover.IsChecked = true;
        }

        _deps?.ResetMapSelectionSize();
    }

    // ── Fill / Eraser ───────────────────────────────────────────────

    private void OnFill(object? sender, RoutedEventArgs e)
    {
        if (_deps == null) return;
        var map = _deps.GetSelectedMap();
        if (map == null) return;
        var sel = _deps.GetMapSelection();
        for (var x = sel.X; x < sel.X + sel.Width; x++)
            for (var y = sel.Y; y < sel.Y + sel.Height; y++)
                _deps.PaintTile(x, y, _deps.MakeSetTile());
    }

    private void OnEraser(object? sender, RoutedEventArgs e)
    {
        if (_deps == null) return;
        var map = _deps.GetSelectedMap();
        if (map == null) return;
        var sel = _deps.GetMapSelection();
        var empty = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
        for (var x = sel.X; x < sel.X + sel.Width; x++)
            for (var y = sel.Y; y < sel.Y + sel.Height; y++)
                _deps.PaintTile(x, y, empty);
    }

    // ── Clipboard ───────────────────────────────────────────────────

    private void OnCopy(object? sender, RoutedEventArgs e)
    {
        if (_deps == null) return;
        var map = _deps.GetSelectedMap();
        if (map == null) return;
        var sel = _deps.GetMapSelection();
        if (sel.Width <= 0 || sel.Height <= 0) return;
        var w = Math.Abs(sel.Width);
        var h = Math.Abs(sel.Height);
        var tiles = new DefinitionsTileData[w, h];
        for (var x = 0; x < w; x++)
            for (var y = 0; y < h; y++)
            {
                var wx = sel.X + x;
                var wy = sel.Y + y;
                var coord = MapMath.TileToChunk(wx, wy);
                var lx = ((wx % MapMath.ChunkSize) + MapMath.ChunkSize) % MapMath.ChunkSize;
                var ly = ((wy % MapMath.ChunkSize) + MapMath.ChunkSize) % MapMath.ChunkSize;
                if (map.Chunks.TryGetValue(coord, out var chunk) && chunk.Tiles != null)
                    tiles[x, y] = chunk.Tiles[lx, ly];
                else
                    tiles[x, y] = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
            }

        _deps.SetClipboard(tiles);
    }

    private void OnCut(object? sender, RoutedEventArgs e)
    {
        if (_deps == null) return;
        var map = _deps.GetSelectedMap();
        if (map == null) return;
        OnCopy(sender, e);
        var sel = _deps.GetMapSelection();
        var empty = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
        for (var x = sel.X; x < sel.X + sel.Width; x++)
            for (var y = sel.Y; y < sel.Y + sel.Height; y++)
                _deps.PaintTile(x, y, empty);
    }

    private void OnPaste(object? sender, RoutedEventArgs e)
    {
        if (_deps == null) return;
        var map = _deps.GetSelectedMap();
        var data = _deps.GetClipboard();
        if (map == null || data == null) return;
        var sel = _deps.GetMapSelection();
        for (var x = 0; x < data.GetLength(0); x++)
            for (var y = 0; y < data.GetLength(1); y++)
                _deps.PaintTile(sel.X + x, sel.Y + y, data[x, y]);
    }

    // ── View / Audio ────────────────────────────────────────────────

    private void OnEdition(object? sender, RoutedEventArgs e)
    {
        if (butEdition.IsChecked == true) butVisualization.IsChecked = false;
        else butEdition.IsChecked = true;
        Options.Instance.PreMapView = butVisualization.IsChecked == true;
        Client.Framework.Persistence.Repositories.OptionsRepository.Write();
    }

    private void OnVisualization(object? sender, RoutedEventArgs e)
    {
        if (butVisualization.IsChecked == true) butEdition.IsChecked = false;
        else butVisualization.IsChecked = true;
        Options.Instance.PreMapView = butVisualization.IsChecked == true;
        Client.Framework.Persistence.Repositories.OptionsRepository.Write();
    }

    private void OnAudio(object? sender, RoutedEventArgs e) =>
        _deps?.SetShowAudio(butAudio.IsChecked == true);

    public bool IsToolEnabled(ToggleButton btn) => btn.IsEnabled && btn.IsChecked == true;
}
