using Avalonia.Controls;
using Avalonia.Input;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions;
using SystemPoint = System.Drawing.Point;
using SystemRect = System.Drawing.Rectangle;
using SystemSize = System.Drawing.Size;

namespace CryBits.Editors.Forms.Maps.Panes;

internal partial class TileSheetPane : UserControl
{
    public TileSheetPane() => InitializeComponent();

    public int SelectedTileIndex => cmbTiles.SelectedIndex;
    public bool IsAutoTile => chkAuto.IsChecked == true;
    public int TileScrollX => (int)scrlTileX.Value;
    public int TileScrollY => (int)scrlTileY.Value;

    /// <summary>Current tile selection rectangle in tile coordinates.</summary>
    public SystemRect TileSelectionRect { get; set; } = new(0, 0, 1, 1);

    /// <summary>Mouse position within the tile sheet (pixel coordinates).</summary>
    public SystemPoint TileMousePosition { get; set; }

    public Image ImgTile => imgTile;
    public Border TileViewport => tileViewport;

    /// <summary>Raised when the selected tile index, auto-tile, or scroll changes.</summary>
    public event Action? SelectionChanged;

    public void PopulateTiles()
    {
        for (var i = 1; i < Textures.Tiles.Count; i++)
            cmbTiles.Items.Add(i.ToString());
        if (cmbTiles.Items.Count > 0)
            cmbTiles.SelectedIndex = 0;
    }

    public void SelectTileByIndex(int index)
    {
        if (index >= 0 && index < cmbTiles.Items.Count)
            cmbTiles.SelectedIndex = index;
    }

    public void SetAutoTile(bool auto) => chkAuto.IsChecked = auto;

    // ── Internal event wiring ───────────────────────────────────────

    public void WireHandlers()
    {
        cmbTiles.SelectionChanged += (_, _) => OnTileSelectionChanged();
        chkAuto.IsCheckedChanged += (_, _) => UpdateSelectedSize();
        imgTile.PointerPressed += OnPointerPressed;
        imgTile.PointerMoved += OnPointerMoved;
        tileViewport.LayoutUpdated += (_, _) => UpdateScrollBounds();
    }

    // ── Handlers ────────────────────────────────────────────────────

    private void OnTileSelectionChanged()
    {
        scrlTileX.Value = 0;
        scrlTileY.Value = 0;
        chkAuto.IsChecked = false;
        UpdateScrollBounds();
        TileMousePosition = new SystemPoint(0);
        TileSelectionRect = new SystemRect(0, 0, 1, 1);
        SelectionChanged?.Invoke();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(imgTile).Properties.IsLeftButtonPressed) return;
        if (cmbTiles.SelectedIndex < 0) return;

        var pt = e.GetPosition(imgTile);
        var x = (int)(pt.X + scrlTileX.Value) / Globals.Grid;
        var y = (int)(pt.Y + scrlTileY.Value) / Globals.Grid;
        var tex = Textures.Tiles[cmbTiles.SelectedIndex + 1];
        if ((int)(pt.X + scrlTileX.Value) > tex.ToSize().Width) return;
        if ((int)(pt.Y + scrlTileY.Value) > tex.ToSize().Height) return;

        TileSelectionRect = new SystemRect(new SystemPoint(x, y), TileSelectionRect.Size);
        UpdateSelectedSize();
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (cmbTiles.SelectedIndex < 0) return;

        var pt = e.GetPosition(imgTile);
        var x = (int)(pt.X + scrlTileX.Value) / Globals.Grid;
        var y = (int)(pt.Y + scrlTileY.Value) / Globals.Grid;
        var tex = Textures.Tiles[cmbTiles.SelectedIndex + 1];
        var size = tex.ToSize();

        TileMousePosition = new SystemPoint(
            (x * Globals.Grid) - (int)scrlTileX.Value,
            (y * Globals.Grid) - (int)scrlTileY.Value);

        if (!e.GetCurrentPoint(imgTile).Properties.IsLeftButtonPressed) return;
        if (chkAuto.IsChecked == true) return;

        x = Math.Clamp(x, 0, (size.Width / Globals.Grid) - 1);
        y = Math.Clamp(y, 0, (size.Height / Globals.Grid) - 1);
        TileSelectionRect = new SystemRect(
            TileSelectionRect.Location,
            new SystemSize(x - TileSelectionRect.X + 1, y - TileSelectionRect.Y + 1));
    }

    private void UpdateSelectedSize()
    {
        var size = chkAuto.IsChecked == true
            ? new SystemSize(2, 3)
            : new SystemSize(1, 1);
        TileSelectionRect = new SystemRect(TileSelectionRect.Location, size);
    }

    private void UpdateScrollBounds()
    {
        if (cmbTiles.SelectedIndex < 0) return;
        var texSize = Textures.Tiles[cmbTiles.SelectedIndex + 1].ToSize();
        var visibleW = Math.Max(1, (int)tileViewport.Bounds.Width);
        var visibleH = Math.Max(1, (int)tileViewport.Bounds.Height);
        scrlTileX.Maximum = Math.Max(0, texSize.Width - visibleW);
        scrlTileY.Maximum = Math.Max(0, texSize.Height - visibleH);
    }
}
