using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using CryBits.Definitions;
using CryBits.Definitions.Maps;
using DefinitionsTileData = CryBits.Definitions.Maps.TileData;
using SystemPoint = System.Drawing.Point;
using SystemRect = System.Drawing.Rectangle;
using SystemSize = System.Drawing.Size;

namespace CryBits.Editors.Forms.Maps.Panes;

internal enum MouseButtons { None, Left, Right }

internal sealed record CanvasDeps
{
    public required Func<Map?> GetSelectedMap { get; init; }
    public required Func<Layer> GetPaintLayer { get; init; }
    public required ToolbarPane ToolbarPane { get; init; }
    public required LayersPane LayersPane { get; init; }
    public required TileSheetPane TileSheetPane { get; init; }
    public required AttributesPane AttributesPane { get; init; }
    public required NpcPane NpcPane { get; init; }
    public Action? RefreshChunkList { get; init; }
}

internal partial class MapCanvasPane : UserControl
{
    public MapCanvasPane() => InitializeComponent();

    // ── Public API ──────────────────────────────────────────────────

    public CanvasDeps? Deps { get; set; }
    public ZoomBorder ZoomBorder => zoomBorder;
    public Image ImgMap => imgMap;

    public SystemPoint MapMouse => _mapMouse;
    public SystemRect MapSelection => ComputeMapSelection();
    public SystemRect TileSource
    {
        get
        {
            var ts = TilesSelection;
            return new SystemRect(ts.X * Globals.Grid, ts.Y * Globals.Grid, ts.Width * Globals.Grid, ts.Height * Globals.Grid);
        }
    }

    public int ViewportTileX => (int)(-ZoomBorder.OffsetX / (Globals.Grid * ZoomBorder.ZoomX));
    public int ViewportTileY => (int)(-ZoomBorder.OffsetY / (Globals.Grid * ZoomBorder.ZoomY));

    public void ResetMapSelectionSize() => _defMapSelection.Size = new SystemSize(1, 1);

    // Public wrappers for toolbar callbacks
    public DefinitionsTileData MakeSetTileForTool() => MakeSetTile();
    public void PaintTileForTool(int x, int y, DefinitionsTileData t) => PaintTile(x, y, t);

    // ── Internal state ──────────────────────────────────────────────

    private SystemPoint _mapMouse;
    private SystemRect _defMapSelection = new(0, 0, 1, 1);
    private bool _mapPressed;

    // ── Event wiring ────────────────────────────────────────────────

    public void WireHandlers()
    {
        imgMap.PointerPressed += OnPointerPressed;
        imgMap.PointerReleased += OnPointerReleased;
        imgMap.PointerMoved += OnPointerMoved;
    }

    // ── Pointer handlers ────────────────────────────────────────────

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var map = Deps?.GetSelectedMap();
        if (map == null) return;

        var pt = e.GetPosition(imgMap);
        var left = e.GetCurrentPoint(imgMap).Properties.IsLeftButtonPressed;
        var right = e.GetCurrentPoint(imgMap).Properties.IsRightButtonPressed;
        var btn = left ? MouseButtons.Left : right ? MouseButtons.Right : MouseButtons.None;

        UpdateMapMouse(pt.X, pt.Y);

        if (Deps!.ToolbarPane.ModeNormal)
        {
            TileEvents(btn);
            if (Deps.ToolbarPane.ToolArea)
                _defMapSelection = new SystemRect(_mapMouse, new SystemSize(1, 1));
        }
        else if (Deps.ToolbarPane.ModeAttributes && left)
        {
            Deps.AttributesPane.SetAttributeAt(MapSelection);
        }
        else if (Deps.ToolbarPane.ModeAttributes && right)
        {
            Deps.AttributesPane.ClearAttributeAt(MapSelection);
        }
        else if (Deps.ToolbarPane.ModeNPCs && left)
        {
            Deps.NpcPane.AddNpcAt((byte)_mapMouse.X, (byte)_mapMouse.Y);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _mapPressed = false;
        if (e.InitialPressMouseButton != MouseButton.Left) return;
        var map = Deps?.GetSelectedMap();
        if (map == null) return;

        var sel = MapSelection;
        if (Deps!.ToolbarPane.ToolRectangle && (sel.Width > 1 || sel.Height > 1))
            for (var x = sel.X; x < sel.X + sel.Width; x++)
                for (var y = sel.Y; y < sel.Y + sel.Height; y++)
                    PaintTile(x, y, MakeSetTile());
        ResetMapSelectionSize();
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var map = Deps?.GetSelectedMap();
        if (map == null) return;

        var pt = e.GetPosition(imgMap);
        var left = e.GetCurrentPoint(imgMap).Properties.IsLeftButtonPressed;
        var right = e.GetCurrentPoint(imgMap).Properties.IsRightButtonPressed;
        var btn = left ? MouseButtons.Left : right ? MouseButtons.Right : MouseButtons.None;

        UpdateMapMouse(pt.X, pt.Y);
        if (MapRectangle(pt.X, pt.Y, left)) return;
        if (Deps!.ToolbarPane.ToolArea && Deps.ToolbarPane.IsToolEnabled(Deps.ToolbarPane.ToolAreaButton)) return;

        _defMapSelection.Location = _mapMouse;

        if (Deps.ToolbarPane.ModeNormal)
            TileEvents(btn);
        else if (!Deps.ToolbarPane.ModeAttributes)
            return;
        else if (Deps.ToolbarPane.ModeAttributes)
            Deps.AttributesPane.SetAttributeAt(MapSelection);
    }

    private void UpdateMapMouse(double px, double py)
    {
        var x = (int)(px / Globals.Grid) + ViewportTileX;
        var y = (int)(py / Globals.Grid) + ViewportTileY;
        _mapMouse = new SystemPoint(x, y);
    }

    // ── Tile painting ───────────────────────────────────────────────

    private void PaintTile(int worldX, int worldY, DefinitionsTileData tile)
    {
        var map = Deps?.GetSelectedMap();
        if (map == null) return;
        var coord = MapMath.TileToChunk(worldX, worldY);
        var lx = ((worldX % MapMath.ChunkSize) + MapMath.ChunkSize) % MapMath.ChunkSize;
        var ly = ((worldY % MapMath.ChunkSize) + MapMath.ChunkSize) % MapMath.ChunkSize;

        if (!map.Chunks.TryGetValue(coord, out var chunk) || chunk.Tiles == null)
        {
            var newTiles = new DefinitionsTileData[MapMath.ChunkSize, MapMath.ChunkSize];
            for (var x = 0; x < MapMath.ChunkSize; x++)
                for (var y = 0; y < MapMath.ChunkSize; y++)
                    newTiles[x, y] = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
            chunk = new MapChunk(coord.X, coord.Y, 1, newTiles);
            map.Chunks[coord] = chunk;
            Deps?.RefreshChunkList?.Invoke();
        }

        chunk.Tiles?[lx, ly] = tile with { Layer = Deps!.GetPaintLayer() };
        chunk = chunk.WithNextVersion();
        map.Chunks[coord] = chunk;
    }

    private void TileEvents(MouseButtons btn)
    {
        var map = Deps?.GetSelectedMap();
        if (map == null) return;
        if (btn == MouseButtons.Left)
        {
            if (Deps!.ToolbarPane.ToolPencil) PaintTile(_mapMouse.X, _mapMouse.Y, MakeSetTile());
            if (Deps.ToolbarPane.ToolDiscover) TileDiscover();
        }
        else if (btn == MouseButtons.Right)
        {
            if (Deps!.ToolbarPane.ToolPencil)
            {
                var empty = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
                PaintTile(_mapMouse.X, _mapMouse.Y, empty);
            }
        }
    }

    private bool MapRectangle(double px, double py, bool left)
    {
        var x = (int)(px / Globals.Grid) + ViewportTileX;
        var y = (int)(py / Globals.Grid) + ViewportTileY;
        if (!left) return false;
        if (!Deps!.ToolbarPane.IsToolEnabled(Deps.ToolbarPane.ToolRectangleButton)
            && !Deps.ToolbarPane.IsToolEnabled(Deps.ToolbarPane.ToolAreaButton))
            return false;
        if (!_mapPressed) _defMapSelection.Size = new SystemSize(1, 1);
        _defMapSelection.Width = x - _defMapSelection.X + 1;
        _defMapSelection.Height = y - _defMapSelection.Y + 1;
        _mapPressed = true;
        return true;
    }

    private void TileDiscover()
    {
        var map = Deps?.GetSelectedMap();
        if (map == null) return;
        var coord = MapMath.TileToChunk(_mapMouse.X, _mapMouse.Y);
        var lx = ((_mapMouse.X % MapMath.ChunkSize) + MapMath.ChunkSize) % MapMath.ChunkSize;
        var ly = ((_mapMouse.Y % MapMath.ChunkSize) + MapMath.ChunkSize) % MapMath.ChunkSize;
        if (!map.Chunks.TryGetValue(coord, out var chunk) || chunk.Tiles == null) return;
        var data = chunk.Tiles[lx, ly];
        if (data.Texture == 0) return;
        Deps!.TileSheetPane.SelectTileByIndex(data.Texture - 1);
        Deps.TileSheetPane.SetAutoTile(data.IsAutoTile);
        Deps.TileSheetPane.TileSelectionRect = new SystemRect(data.SourceX, data.SourceY, 1, 1);
    }

    private DefinitionsTileData MakeSetTile(int x = 0, int y = 0)
    {
        var ts = TilesSelection;
        if (x == 0) x = Math.Max(0, ts.X);
        if (y == 0) y = Math.Max(0, ts.Y);
        return new DefinitionsTileData(
            (byte)(Deps!.TileSheetPane.SelectedTileIndex + 1),
            x, y,
            Deps.TileSheetPane.IsAutoTile,
            new NoAttribute(),
            Deps.GetPaintLayer()
        );
    }

    // ── Selection computation ───────────────────────────────────────

    private static SystemRect SelectionRec(SystemRect t)
    {
        if (t.Width <= 0) { t.X += t.Width - 1; t.Width = (t.Width - 2) * -1; }
        if (t.Height <= 0) { t.Y += t.Height - 1; t.Height = (t.Height - 2) * -1; }
        return t;
    }

    private SystemRect TilesSelection => SelectionRec(Deps!.TileSheetPane.TileSelectionRect);

    private SystemRect ComputeMapSelection()
    {
        if (Deps!.TileSheetPane.IsAutoTile)
            return new SystemRect(_mapMouse, new SystemSize(1, 1));
        if (Deps.ToolbarPane.ModeNormal && Deps.ToolbarPane.ToolPencil)
            return new SystemRect(_mapMouse, TilesSelection.Size);
        return SelectionRec(_defMapSelection);
    }
}
