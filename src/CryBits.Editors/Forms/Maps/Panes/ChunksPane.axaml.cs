using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Interactivity;
using CryBits.Definitions;
using CryBits.Definitions.Maps;
using static CryBits.Editors.Forms.Maps.MapMath;
using DefinitionsTileData = CryBits.Definitions.Maps.TileData;

namespace CryBits.Editors.Forms.Maps.Panes;

internal partial class ChunksPane : UserControl
{
    public ChunksPane()
    {
        InitializeComponent();
        butChunkLeft.Click += (_, _) => AddChunkAt((short)(FloorDiv(ViewportTileX, ChunkSize) - 1),
            (short)FloorDiv(ViewportTileY, ChunkSize));
        butChunkRight.Click += (_, _) => AddChunkAt((short)(FloorDiv(ViewportTileX, ChunkSize) + 1),
            (short)FloorDiv(ViewportTileY, ChunkSize));
        butChunkUp.Click += (_, _) => AddChunkAt((short)FloorDiv(ViewportTileX, ChunkSize),
            (short)(FloorDiv(ViewportTileY, ChunkSize) - 1));
        butChunkDown.Click += (_, _) => AddChunkAt((short)FloorDiv(ViewportTileX, ChunkSize),
            (short)(FloorDiv(ViewportTileY, ChunkSize) + 1));
        butDeleteChunk.Click += OnDeleteChunk;
    }

    public Map? SelectedMap { get; set; }
    public ListBox LstChunks => lstChunks;
    public ZoomBorder? ZoomBorder { get; set; }
    public Func<Map?>? GetSelectedMap { get; set; }

    private int ViewportTileX => (int)(-ZoomBorder!.OffsetX / (Globals.Grid * ZoomBorder.ZoomX));
    private int ViewportTileY => (int)(-ZoomBorder!.OffsetY / (Globals.Grid * ZoomBorder.ZoomY));

    public void RefreshList()
    {
        if (SelectedMap == null) return;
        lstChunks.ItemsSource = null;
        lstChunks.ItemsSource = SelectedMap.Chunks.Keys.OrderBy(c => c.Y).ThenBy(c => c.X).ToList();
    }

    private void AddChunkAt(short cx, short cy)
    {
        var map = GetSelectedMap?.Invoke();
        if (map == null) return;
        var coord = new ChunkCoord(cx, cy);
        if (map.Chunks.ContainsKey(coord)) return;
        var tiles = new DefinitionsTileData[ChunkSize, ChunkSize];
        for (var x = 0; x < ChunkSize; x++)
            for (var y = 0; y < ChunkSize; y++)
                tiles[x, y] = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
        map.Chunks[coord] = new MapChunk(cx, cy, 1, tiles);
        RefreshList();
    }

    private void OnDeleteChunk(object? sender, RoutedEventArgs e)
    {
        var map = GetSelectedMap?.Invoke();
        if (map == null) return;
        var cx = (short)FloorDiv(ViewportTileX, ChunkSize);
        var cy = (short)FloorDiv(ViewportTileY, ChunkSize);
        var coord = new ChunkCoord(cx, cy);
        map.Chunks.Remove(coord);
        RefreshList();
    }
}
