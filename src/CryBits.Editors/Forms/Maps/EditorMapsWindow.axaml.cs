using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.PropertyGrid.Controls;
using Avalonia.Threading;
using CryBits.Client.Framework;
using DefinitionsTileData = CryBits.Definitions.Maps.TileData;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms.Classes;
using CryBits.Editors.Forms.Interface;
using CryBits.Editors.Forms.Items;
using CryBits.Editors.Forms.Maps.Properties;
using CryBits.Editors.Forms.Npcs;
using CryBits.Editors.Forms.Shops;
using CryBits.Editors.Forms.Tiles;
using CryBits.Editors.Network;

using SFML.Graphics;
using SFML.System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AvaloniaScrollEventArgs = Avalonia.Controls.Primitives.ScrollEventArgs;
using SelectionChangedEventArgs = Avalonia.Controls.SelectionChangedEventArgs;
using SystemPoint = System.Drawing.Point;
using SystemRect = System.Drawing.Rectangle;
using SystemSize = System.Drawing.Size;
using TextChangedEventArgs = Avalonia.Controls.TextChangedEventArgs;

namespace CryBits.Editors.Maps;

internal enum MouseButtons { None, Left, Right }

internal sealed class LayerVm : INotifyPropertyChanged
{
    private bool _visible = true;
    public int Index { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TypeName { get; init; } = string.Empty;
    public bool Visible { get => _visible; set { _visible = value; OnPropertyChanged(); } }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
}

internal partial class EditorMapsWindow : Window
{
    private const int Grid = Globals.Grid;
    private const int ChunkSize = 32;

    private readonly DefinitionCatalog _catalog;

    public static void Open()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var window = new EditorMapsWindow(DefinitionCatalog.Instance);
            window.Show();
        });
    }

    public static void Open(Window parent)
    {
        Dispatcher.UIThread.Post(async () =>
        {
            parent.Hide();
            var window = new EditorMapsWindow(DefinitionCatalog.Instance);
            await window.ShowDialog(parent);
            parent.Show();
        });
    }

    public static EditorMapsWindow? Instance { get; private set; }

    private volatile bool _isOpen;
    private volatile bool _showAudio;
    private volatile bool _showVisualization;

    public bool IsOpen => _isOpen;
    public bool ShowAudioSafe => _showAudio;
    public bool ShowVisualizationSafe => _showVisualization;

    public bool ModeNormal => butMNormal.IsChecked == true;
    public bool ModeAttributes => butMAttributes.IsChecked == true;
    public bool ModeNPCs => butMNPCs.IsChecked == true;

    public bool ToolPencil => butPencil.IsChecked == true;
    public bool ToolRectangle => butRectangle.IsChecked == true;
    public bool ToolArea => butArea.IsChecked == true;
    public bool ToolDiscover => butDiscover.IsChecked == true;

    public bool ShowGrid => butGrid.IsChecked == true;
    public bool ShowEdition => butEdition.IsChecked == true;
    public bool ShowVisualization => butVisualization.IsChecked == true;
    public bool ShowAudio => butAudio.IsChecked == true;

    public bool AutoTile => chkAuto.IsChecked == true;

    public int TileSheetIndex => cmbTiles.SelectedIndex;
    public int TileScrollX => (int)scrlTileX.Value;
    public int TileScrollY => (int)scrlTileY.Value;
    public int MapScrollX => (int)scrlMapX.Value;
    public int MapScrollY => (int)scrlMapY.Value;

    public SystemPoint TileMouse { get; private set; }

    public int MapCanvasWidth { get; } = 800;
    public int MapCanvasHeight { get; } = 600;
    public int TileCanvasWidth { get; } = 282;
    public int TileCanvasHeight { get; } = 420;

    public Map? SelectedMap => _selected;

    private Map? _selected;
    private MapProperties? _mapProps;
    private bool _mapPressed;
    private Layer _paintLayer = Layer.Ground;

    private SystemPoint _mapMouse;
    private SystemRect _defTilesSelection = new(0, 0, 1, 1);
    private SystemRect _defMapSelection = new(0, 0, 1, 1);

    private DefinitionsTileData[,]? _clipboardData;
    private SystemRect _clipboardArea;
    private Layer _clipboardLayer;

    // Tile sheet pane
    private ComboBox cmbTiles => tileSheetPane.CmbTiles;
    private CheckBox chkAuto => tileSheetPane.ChkAuto;
    private Avalonia.Controls.Image imgTile => tileSheetPane.ImgTile;
    private ScrollBar scrlTileX => tileSheetPane.ScrlTileX;
    private ScrollBar scrlTileY => tileSheetPane.ScrlTileY;

    // Map canvas pane
    private Avalonia.Controls.Image imgMap => mapCanvasPane.ImgMap;
    private ScrollBar scrlMapX => mapCanvasPane.ScrlMapX;
    private ScrollBar scrlMapY => mapCanvasPane.ScrlMapY;

    // Layers pane (repurposed)
    private DataGrid lstLayers => layersPane.LstLayers;
    private Border grpAttributes => layersPane.GrpAttributes;
    private RadioButton optA_Block => layersPane.OptA_Block;
    private RadioButton optA_Warp => layersPane.OptA_Warp;
    private RadioButton optA_Item => layersPane.OptA_Item;
    private Border grpA_Warp => layersPane.GrpA_Warp;
    private ComboBox cmbA_Warp_Map => layersPane.CmbA_Warp_Map;
    private NumericUpDown numA_Warp_X => layersPane.NumA_Warp_X;
    private NumericUpDown numA_Warp_Y => layersPane.NumA_Warp_Y;
    private Border grpA_Item => layersPane.GrpA_Item;
    private ComboBox cmbA_Item => layersPane.CmbA_Item;
    private NumericUpDown numA_Item_Amount => layersPane.NumA_Item_Amount;
    private Border grpNPCs => layersPane.GrpNPCs;
    private ComboBox cmbNPC => layersPane.CmbNPC;
    private NumericUpDown numNPC_Zone => layersPane.NumNPC_Zone;
    private ListBox lstNPC => layersPane.LstNPC;

    // Map explorer pane
    private TextBox txtFilter => explorerPane.TxtFilter;
    private ListBox lstMaps => explorerPane.LstMaps;

    // Properties pane
    private PropertyGrid prgMapProperties => propertiesPane!.PrgMapProperties;

    // Chunk navigation
    private RadioButton butLayerGround => layersPane.OptA_Block;  // reused UI elements
    private RadioButton butLayerFringe => layersPane.OptA_Warp;

    private TileSheetPane tileSheetPane = null!;
    private MapCanvasPane mapCanvasPane = null!;
    private LayersPane layersPane = null!;
    private MapExplorerPane explorerPane = null!;
    private PropertiesPane propertiesPane = null!;

    private readonly DispatcherTimer? _timer;

    public EditorMapsWindow(DefinitionCatalog catalog)
    {
        _catalog = catalog;
        InitializeComponent();
        Instance = this;

        tileSheetPane = new TileSheetPane();
        layersPane = new LayersPane();
        mapCanvasPane = new MapCanvasPane();
        explorerPane = new MapExplorerPane();
        propertiesPane = new PropertiesPane();
        AssignContentToDockModels();

        MapRenderer.Instance.WinMap = new RenderTexture(new Vector2u((uint)MapCanvasWidth, (uint)MapCanvasHeight));
        MapRenderer.Instance.WinMapTile = new RenderTexture(new Vector2u((uint)TileCanvasWidth, (uint)TileCanvasHeight));

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;

        Loaded += OnLoaded;
    }

    private void AssignContentToDockModels()
    {
        if (DockControl?.Layout is not Dock.Model.Avalonia.Controls.RootDock root) return;
        void Walk(IList<Dock.Model.Core.IDockable>? dockables)
        {
            if (dockables == null) return;
            foreach (var d in dockables)
            {
                switch (d)
                {
                    case Dock.Model.Avalonia.Controls.ToolDock td when td.VisibleDockables?.Count > 0:
                        td.ActiveDockable = td.VisibleDockables[0];
                        foreach (var t in td.VisibleDockables) AssignContent(t);
                        break;
                    case Dock.Model.Avalonia.Controls.DocumentDock dd when dd.VisibleDockables?.Count > 0:
                        dd.ActiveDockable = dd.VisibleDockables[0];
                        foreach (var doc in dd.VisibleDockables) AssignContent(doc);
                        break;
                    case Dock.Model.Avalonia.Controls.ProportionalDock pd:
                        Walk(pd.VisibleDockables);
                        break;
                }
            }
        }
        Walk(root.VisibleDockables);
    }

    private void AssignContent(Dock.Model.Core.IDockable dockable)
    {
        switch (dockable.Id)
        {
            case "TileSheet" when dockable is Dock.Model.Avalonia.Controls.Tool t: t.Content = tileSheetPane; break;
            case "Layers" when dockable is Dock.Model.Avalonia.Controls.Tool t: t.Content = layersPane; break;
            case "MapCanvas" when dockable is Dock.Model.Avalonia.Controls.Document doc: doc.Content = mapCanvasPane; break;
            case "MapExplorer" when dockable is Dock.Model.Avalonia.Controls.Tool t: t.Content = explorerPane; break;
            case "Properties" when dockable is Dock.Model.Avalonia.Controls.Tool t: t.Content = propertiesPane; break;
        }
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        tileSheetPane.CmbTiles.SelectionChanged += cmbTiles_SelectionChanged;
        tileSheetPane.ChkAuto.IsCheckedChanged += chkAuto_IsCheckedChanged;
        tileSheetPane.ImgTile.PointerPressed += imgTile_PointerPressed;
        tileSheetPane.ImgTile.PointerMoved += imgTile_PointerMoved;
        tileSheetPane.ScrlTileY.Scroll += scrlTileY_Scroll;
        tileSheetPane.ScrlTileX.Scroll += scrlTileX_Scroll;

        mapCanvasPane.ImgMap.PointerPressed += imgMap_PointerPressed;
        mapCanvasPane.ImgMap.PointerReleased += imgMap_PointerReleased;
        mapCanvasPane.ImgMap.PointerMoved += imgMap_PointerMoved;
        mapCanvasPane.ScrlMapY.Scroll += scrlMapY_Scroll;
        mapCanvasPane.ScrlMapX.Scroll += scrlMapX_Scroll;

        layersPane.LstLayers.SelectionChanged += lstLayers_SelectionChanged;
        layersPane.ScrlZone.Scroll += scrlZone_Scroll;
        layersPane.ScrlZone_Clear.Click += scrlZone_Clear_Click;
        layersPane.OptA_Warp.IsCheckedChanged += optA_Warp_Changed;
        layersPane.OptA_Item.IsCheckedChanged += optA_Item_Changed;
        layersPane.CmbA_Warp_Map.SelectionChanged += cmbA_Warp_Map_SelectionChanged;
        layersPane.NumA_Warp_X.ValueChanged += numA_Warp_X_ValueChanged;
        layersPane.NumA_Warp_Y.ValueChanged += numA_Warp_Y_ValueChanged;
        layersPane.CmbA_Item.SelectionChanged += cmbA_Item_SelectionChanged;
        layersPane.NumA_Item_Amount.ValueChanged += numA_Item_Amount_ValueChanged;
        layersPane.ButAttributes_Clear.Click += butAttributes_Clear_Click;
        layersPane.ButAttributes_Import.Click += butAttributes_Import_Click;
        layersPane.ButNPC_Add.Click += butNPC_Add_Click;
        layersPane.ButNPC_Remove.Click += butNPC_Remove_Click;
        layersPane.ButNPC_Clear.Click += butNPC_Clear_Click;

        explorerPane.TxtFilter.TextChanged += txtFilter_TextChanged;
        explorerPane.ButNew.Click += butNew_Click;
        explorerPane.ButRemove.Click += butRemove_Click;
        explorerPane.LstMaps.SelectionChanged += lstMaps_SelectionChanged;

        // Wire chunk list selection
        layersPane.LstChunks.SelectionChanged += (_, _) =>
        {
            if (layersPane.LstChunks.SelectedItem is ChunkCoord coord)
            {
                scrlMapX.Value = coord.X * ChunkSize;
                scrlMapY.Value = coord.Y * ChunkSize;
            }
        };

        for (var i = 1; i < Textures.Tiles.Count; i++)
            tileSheetPane.CmbTiles.Items.Add(i.ToString());
        if (tileSheetPane.CmbTiles.Items.Count > 0)
            tileSheetPane.CmbTiles.SelectedIndex = 0;

        layersPane.ScrlZone.Maximum = Globals.MaxZones;
        layersPane.NumNPC_Zone.Maximum = Globals.MaxZones;

        _timer!.Start();
        RefreshMapList();
        _isOpen = true;
        _showAudio = butAudio.IsChecked == true;
        _showVisualization = butVisualization.IsChecked == true;
    }

    protected override void OnClosed(EventArgs e)
    {
        _isOpen = false;
        _timer?.Stop();
        MapRenderer.Instance.WinMap = null;
        MapRenderer.Instance.WinMapTile = null;
        Instance = null;
        base.OnClosed(e);
    }

    private void OnRenderTick(object? sender, EventArgs e)
    {
        if (MapRenderer.Instance.WinMap != null && _selected != null)
        {
            MapRenderer.Instance.EditorMapsMap();
            SfmlRenderBlit.Blit(MapRenderer.Instance.WinMap, ref _mapBitmap, imgMap);
        }
        if (MapRenderer.Instance.WinMapTile != null && ModeNormal)
        {
            MapRenderer.Instance.EditorMapsTile();
            SfmlRenderBlit.Blit(MapRenderer.Instance.WinMapTile, ref _tileBitmap, imgTile);
        }
        UpdateStatusBar();
    }

    // Render bitmaps
    private WriteableBitmap? _mapBitmap;
    private WriteableBitmap? _tileBitmap;

    // ── MAP LIST ───────────────────────────────────────────────────────

    private void RefreshMapList(Guid? keepId = null)
    {
        var filter = txtFilter.Text ?? string.Empty;
        var filtered = _catalog.Maps.Values
            .Where(m => m.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();
        lstMaps.ItemsSource = filtered;
        if (keepId.HasValue)
            lstMaps.SelectedItem = filtered.FirstOrDefault(m => m.Id == keepId.Value);
        if (lstMaps.SelectedItem == null && filtered.Count > 0)
            lstMaps.SelectedIndex = 0;
    }

    private void lstMaps_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (lstMaps.SelectedItem is not Map map) return;
        SelectMap(map);
    }

    private void SelectMap(Map map)
    {
        _selected = map;
        RefreshWarpMapCombo();
        if (_mapProps != null) _mapProps.PropertyChanged -= OnMapPropertyChanged;
        _mapProps = new MapProperties(map);
        _mapProps.PropertyChanged += OnMapPropertyChanged;
        prgMapProperties.DataContext = _mapProps;
        RefreshNpcList();
        MapInstance.Instance.UpdateWeatherType();
        RefreshChunkList();
        UpdateMapBounds();
    }

    private void OnMapPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MapProperties.Name))
            RefreshMapList(_selected?.Id);
    }

    private void RefreshWarpMapCombo()
    {
        cmbA_Warp_Map.Items.Clear();
        foreach (var m in _catalog.Maps.Values) cmbA_Warp_Map.Items.Add(m);
        if (cmbA_Warp_Map.Items.Count > 0) cmbA_Warp_Map.SelectedIndex = 0;
        numA_Warp_X.Maximum = 999;
        numA_Warp_Y.Maximum = 999;
    }

    private void txtFilter_TextChanged(object? sender, TextChangedEventArgs e) => RefreshMapList(_selected?.Id);

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        var map = new Map();
        _catalog.Maps.Add(map.Id, map);
        RefreshMapList(map.Id);
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        if (_catalog.Maps.Count == 1) { MessageBox.Show("It must have at least one map registered."); return; }
        _catalog.Maps.Remove(_selected.Id);
        _selected = null;
        RefreshMapList();
    }

    // ── STATUS BAR ─────────────────────────────────────────────────────

    private void UpdateStatusBar()
    {
        lblFPS.Text = $"FPS: {Program.Fps}";
        var cx = _mapMouse.X / ChunkSize;
        var cy = _mapMouse.Y / ChunkSize;
        var lx = ((_mapMouse.X % ChunkSize) + ChunkSize) % ChunkSize;
        var ly = ((_mapMouse.Y % ChunkSize) + ChunkSize) % ChunkSize;
        lblPosition.Text = $"Chunk: ({cx},{cy}) Tile: ({lx},{ly}) World: ({_mapMouse.X},{_mapMouse.Y})";
    }

    // ── TOOLBAR ────────────────────────────────────────────────────────

    private void butSaveAll_Click(object? sender, RoutedEventArgs e)
    {
        PackageSender.Instance.WriteMaps();
        MessageBox.Show("All maps has been saved");
    }

    private void butReload_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        PackageSender.Instance.RequestMap(_selected);
    }

    private void butPencil_Click(object? sender, RoutedEventArgs e)
    {
        if (butPencil.IsChecked == true) { butRectangle.IsChecked = false; butArea.IsChecked = false; butDiscover.IsChecked = false; }
        else butPencil.IsChecked = true;
        ResetMapSelectionSize();
    }

    private void butRectangle_Click(object? sender, RoutedEventArgs e)
    {
        if (butRectangle.IsChecked == true) { butPencil.IsChecked = false; butArea.IsChecked = false; butDiscover.IsChecked = false; }
        else butRectangle.IsChecked = true;
        ResetMapSelectionSize();
    }

    private void butArea_Click(object? sender, RoutedEventArgs e)
    {
        if (butArea.IsChecked == true) { butPencil.IsChecked = false; butRectangle.IsChecked = false; butDiscover.IsChecked = false; }
        else butArea.IsChecked = true;
        ResetMapSelectionSize();
    }

    private void butDiscover_Click(object? sender, RoutedEventArgs e)
    {
        if (butDiscover.IsChecked == true) { butPencil.IsChecked = false; butRectangle.IsChecked = false; butArea.IsChecked = false; }
        else butDiscover.IsChecked = true;
        ResetMapSelectionSize();
    }

    private void butFill_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        var sel = MapSelection;
        for (var x = sel.X; x < sel.X + sel.Width; x++)
            for (var y = sel.Y; y < sel.Y + sel.Height; y++)
                PaintTile(x, y, MakeSetTile());
    }

    private void butEraser_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        var sel = MapSelection;
        var empty = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
        for (var x = sel.X; x < sel.X + sel.Width; x++)
            for (var y = sel.Y; y < sel.Y + sel.Height; y++)
                PaintTile(x, y, empty);
    }

    private void butCopy_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        var sel = MapSelection;
        if (sel.Width <= 0 || sel.Height <= 0) return;
        var w = Math.Abs(sel.Width);
        var h = Math.Abs(sel.Height);
        var tiles = new DefinitionsTileData[w, h];
        for (var x = 0; x < w; x++)
            for (var y = 0; y < h; y++)
            {
                var wx = sel.X + x;
                var wy = sel.Y + y;
                var coord = TileToChunk(wx, wy);
                var lx = ((wx % ChunkSize) + ChunkSize) % ChunkSize;
                var ly = ((wy % ChunkSize) + ChunkSize) % ChunkSize;
                if (_selected.Chunks.TryGetValue(coord, out var chunk) && chunk.Tiles != null)
                    tiles[x, y] = chunk.Tiles[lx, ly];
                else
                    tiles[x, y] = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
            }
        _clipboardData = tiles;
        _clipboardArea = sel;
        _clipboardLayer = _paintLayer;
    }

    private void butCut_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        butCopy_Click(sender, e);
        var empty = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
        var sel = MapSelection;
        for (var x = sel.X; x < sel.X + sel.Width; x++)
            for (var y = sel.Y; y < sel.Y + sel.Height; y++)
                PaintTile(x, y, empty);
    }

    private void butPaste_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || _clipboardData == null) return;
        var sel = MapSelection;
        for (var x = 0; x < _clipboardData.GetLength(0); x++)
            for (var y = 0; y < _clipboardData.GetLength(1); y++)
                PaintTile(sel.X + x, sel.Y + y, _clipboardData[x, y]);
    }

    private void butEdition_Click(object? sender, RoutedEventArgs e)
    {
        if (butEdition.IsChecked == true) butVisualization.IsChecked = false;
        else butEdition.IsChecked = true;
        Options.Instance.PreMapView = butVisualization.IsChecked == true;
        Client.Framework.Persistence.Repositories.OptionsRepository.Write();
    }

    private void butVisualization_Click(object? sender, RoutedEventArgs e)
    {
        if (butVisualization.IsChecked == true) butEdition.IsChecked = false;
        else butVisualization.IsChecked = true;
        Options.Instance.PreMapView = butVisualization.IsChecked == true;
        Client.Framework.Persistence.Repositories.OptionsRepository.Write();
    }

    private void butGrid_Click(object? sender, RoutedEventArgs e) { }
    private void butAudio_Click(object? sender, RoutedEventArgs e) => _showAudio = butAudio.IsChecked == true;

    // Zoom handlers
    private void butZoom_Normal_Click(object? sender, RoutedEventArgs e)
    { butZoom_Normal.IsChecked = true; butZoom_2x.IsChecked = false; butZoom_4x.IsChecked = false; UpdateMapBounds(); }
    private void butZoom_2x_Click(object? sender, RoutedEventArgs e)
    { butZoom_Normal.IsChecked = false; butZoom_2x.IsChecked = true; butZoom_4x.IsChecked = false; UpdateMapBounds(); }
    private void butZoom_4x_Click(object? sender, RoutedEventArgs e)
    { butZoom_Normal.IsChecked = false; butZoom_2x.IsChecked = false; butZoom_4x.IsChecked = true; UpdateMapBounds(); }

    private void butMNormal_Click(object? sender, RoutedEventArgs e) { ModesExclusive(butMNormal); ResetMapSelectionSize(); }
    private void butMAttributes_Click(object? sender, RoutedEventArgs e) { ModesExclusive(butMAttributes); }
    private void butMZones_Click(object? sender, RoutedEventArgs e) { ModesExclusive(butMZones); }
    private void butMNPCs_Click(object? sender, RoutedEventArgs e)
    {
        ModesExclusive(butMNPCs);
        if (butMNPCs.IsChecked == true)
        {
            cmbNPC.Items.Clear();
            foreach (var npc in _catalog.Npcs.Values) cmbNPC.Items.Add(npc);
            if (cmbNPC.Items.Count > 0) cmbNPC.SelectedIndex = 0;
            numNPC_Zone.Value = 0;
        }
    }

    private void ModesExclusive(ToggleButton pressed)
    {
        foreach (var btn in new[] { butMNormal, butMZones, butMAttributes, butMNPCs })
            btn.IsChecked = btn == pressed ? pressed.IsChecked != true || btn == butMNormal : false;
        if (butMNormal.IsChecked != true)
        {
            butMNormal.IsChecked = false;
            butMZones.IsChecked = butMZones == pressed && pressed.IsChecked == true;
            butMAttributes.IsChecked = butMAttributes == pressed && pressed.IsChecked == true;
            butMNPCs.IsChecked = butMNPCs == pressed && pressed.IsChecked == true;
        }
        layersPane.GrpZones.IsVisible = butMZones.IsChecked == true;
        grpAttributes.IsVisible = butMAttributes.IsChecked == true;
        layersPane.GrpNPCs.IsVisible = butMNPCs.IsChecked == true;
    }

    private void butEditors_Classes_Click(object? sender, RoutedEventArgs e) => EditorClassesWindow.Open(this);
    private void butEditors_Interface_Click(object? sender, RoutedEventArgs e) => EditorInterfaceWindow.Open(this);
    private void butEditors_Items_Click(object? sender, RoutedEventArgs e) => EditorItemsWindow.Open(this);
    private void butEditors_NPCs_Click(object? sender, RoutedEventArgs e) => EditorNpcsWindow.Open(this);
    private void butEditors_Shops_Click(object? sender, RoutedEventArgs e) => EditorShopsWindow.Open(this);
    private void butEditors_Tiles_Click(object? sender, RoutedEventArgs e) => EditorTilesWindow.Open(this);

    // ── CHUNK MANAGEMENT ──────────────────────────────────────────────

    private void RefreshChunkList()
    {
        if (_selected == null) return;
        layersPane.LstChunks.ItemsSource = null;
        layersPane.LstChunks.ItemsSource = _selected.Chunks.Keys.OrderBy(c => c.Y).ThenBy(c => c.X).ToList();
    }

    private void AddChunkAt(short cx, short cy)
    {
        if (_selected == null) return;
        var coord = new ChunkCoord(cx, cy);
        if (_selected.Chunks.ContainsKey(coord)) return;
        var tiles = new DefinitionsTileData[ChunkSize, ChunkSize];
        for (var x = 0; x < ChunkSize; x++)
            for (var y = 0; y < ChunkSize; y++)
                tiles[x, y] = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
        _selected.Chunks[coord] = new MapChunk(cx, cy, 1, tiles);
        RefreshChunkList();
        UpdateMapBounds();
    }

    private void butChunkLeft_Click(object? sender, RoutedEventArgs e)
    {
        var cx = (short)(MapScrollX / ChunkSize - 1);
        var cy = (short)(MapScrollY / ChunkSize);
        AddChunkAt(cx, cy);
    }

    private void butChunkRight_Click(object? sender, RoutedEventArgs e)
    {
        var cx = (short)(MapScrollX / ChunkSize + 1);
        var cy = (short)(MapScrollY / ChunkSize);
        AddChunkAt(cx, cy);
    }

    private void butChunkUp_Click(object? sender, RoutedEventArgs e)
    {
        var cx = (short)(MapScrollX / ChunkSize);
        var cy = (short)(MapScrollY / ChunkSize - 1);
        AddChunkAt(cx, cy);
    }

    private void butChunkDown_Click(object? sender, RoutedEventArgs e)
    {
        var cx = (short)(MapScrollX / ChunkSize);
        var cy = (short)(MapScrollY / ChunkSize + 1);
        AddChunkAt(cx, cy);
    }

    private void butDeleteChunk_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        var cx = (short)(MapScrollX / ChunkSize);
        var cy = (short)(MapScrollY / ChunkSize);
        var coord = new ChunkCoord(cx, cy);
        _selected.Chunks.Remove(coord);
        RefreshChunkList();
        UpdateMapBounds();
    }

    // ── LAYER TOGGLE ──────────────────────────────────────────────────

    private void butLayerGround_Click(object? sender, RoutedEventArgs e) => _paintLayer = Layer.Ground;
    private void butLayerFringe_Click(object? sender, RoutedEventArgs e) => _paintLayer = Layer.Fringe;

    // ── MAP CANVAS ────────────────────────────────────────────────────

    private void imgMap_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_selected == null) return;
        var pt = e.GetPosition(imgMap);
        var left = e.GetCurrentPoint(imgMap).Properties.IsLeftButtonPressed;
        var right = e.GetCurrentPoint(imgMap).Properties.IsRightButtonPressed;
        var btn = left ? MouseButtons.Left : right ? MouseButtons.Right : MouseButtons.None;

        UpdateMapMouse(pt.X, pt.Y);
        var sel = MapSelection;

        if (ModeNormal)
        {
            TileEvents(btn);
            if (ToolArea) _defMapSelection = new SystemRect(_mapMouse, new SystemSize(1, 1));
        }
        else if (ModeAttributes && left)
            SetAttribute();
        else if (ModeAttributes && right)
            ClearAttribute();
        else if (ModeNPCs && left)
            AddNpc(true, (byte)_mapMouse.X, (byte)_mapMouse.Y);
    }

    private void imgMap_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _mapPressed = false;
        if (e.InitialPressMouseButton != MouseButton.Left) return;
        if (_selected == null) return;
        var pt = e.GetPosition(imgMap);
        var sel = MapSelection;

        if (ToolRectangle && (sel.Width > 1 || sel.Height > 1))
        {
            for (var x = sel.X; x < sel.X + sel.Width; x++)
                for (var y = sel.Y; y < sel.Y + sel.Height; y++)
                    PaintTile(x, y, MakeSetTile());
        }
        ResetMapSelectionSize();
    }

    private void imgMap_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_selected == null) return;
        var pt = e.GetPosition(imgMap);
        var left = e.GetCurrentPoint(imgMap).Properties.IsLeftButtonPressed;
        var right = e.GetCurrentPoint(imgMap).Properties.IsRightButtonPressed;
        var btn = left ? MouseButtons.Left : right ? MouseButtons.Right : MouseButtons.None;

        UpdateMapMouse(pt.X, pt.Y);
        if (MapRectangle(pt.X, pt.Y, left)) return;
        if (ToolArea && IsToolEnabled(butArea)) return;

        _defMapSelection.Location = _mapMouse;

        if (ModeNormal)
            TileEvents(btn);
        else if (ModeAttributes && !left)
            return;
        else if (ModeAttributes)
            SetAttribute();
    }

    private void UpdateMapMouse(double px, double py)
    {
        var x = (int)(px / GridZoom) + MapScrollX;
        var y = (int)(py / GridZoom) + MapScrollY;
        _mapMouse = new SystemPoint(x, y);
    }

    private void scrlMapX_Scroll(object? sender, AvaloniaScrollEventArgs e) { }
    private void scrlMapY_Scroll(object? sender, AvaloniaScrollEventArgs e) { }
    private void scrlTileX_Scroll(object? sender, AvaloniaScrollEventArgs e) { }
    private void scrlTileY_Scroll(object? sender, AvaloniaScrollEventArgs e) { }

    // ── TILE PAINTING ─────────────────────────────────────────────────

    private void PaintTile(int worldX, int worldY, DefinitionsTileData tile)
    {
        if (_selected == null) return;
        var coord = TileToChunk(worldX, worldY);
        var lx = ((worldX % ChunkSize) + ChunkSize) % ChunkSize;
        var ly = ((worldY % ChunkSize) + ChunkSize) % ChunkSize;

        if (!_selected.Chunks.TryGetValue(coord, out var chunk) || chunk.Tiles == null)
        {
            var newTiles = new DefinitionsTileData[ChunkSize, ChunkSize];
            for (var x = 0; x < ChunkSize; x++)
                for (var y = 0; y < ChunkSize; y++)
                    newTiles[x, y] = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
            chunk = new MapChunk(coord.X, coord.Y, 1, newTiles);
            _selected.Chunks[coord] = chunk;
        }

        chunk.Tiles?[lx, ly] = tile with { Layer = _paintLayer };
        chunk = chunk.WithNextVersion();
        _selected.Chunks[coord] = chunk;
    }

    private void TileEvents(MouseButtons btn)
    {
        if (_selected == null) return;
        if (btn == MouseButtons.Left)
        {
            if (ToolPencil) PaintTile(_mapMouse.X, _mapMouse.Y, MakeSetTile());
            if (ToolDiscover) TileDiscover();
        }
        else if (btn == MouseButtons.Right)
        {
            if (ToolPencil)
            {
                var empty = new DefinitionsTileData(0, 0, 0, false, new NoAttribute());
                PaintTile(_mapMouse.X, _mapMouse.Y, empty);
            }
        }
    }

    private bool MapRectangle(double px, double py, bool left)
    {
        var x = (int)(px / GridZoom) + MapScrollX;
        var y = (int)(py / GridZoom) + MapScrollY;
        if (!left) return false;
        if (!IsToolEnabled(butRectangle) && !IsToolEnabled(butArea)) return false;
        if (!_mapPressed) _defMapSelection.Size = new SystemSize(1, 1);
        _defMapSelection.Width = x - _defMapSelection.X + 1;
        _defMapSelection.Height = y - _defMapSelection.Y + 1;
        _mapPressed = true;
        return true;
    }

    private void TileDiscover()
    {
        if (_selected == null) return;
        var coord = TileToChunk(_mapMouse.X, _mapMouse.Y);
        var lx = ((_mapMouse.X % ChunkSize) + ChunkSize) % ChunkSize;
        var ly = ((_mapMouse.Y % ChunkSize) + ChunkSize) % ChunkSize;
        if (!_selected.Chunks.TryGetValue(coord, out var chunk) || chunk.Tiles == null) return;
        var data = chunk.Tiles[lx, ly];
        if (data.Texture == 0) return;
        cmbTiles.SelectedIndex = data.Texture - 1;
        chkAuto.IsChecked = data.IsAutoTile;
        _defTilesSelection = new SystemRect(data.SourceX, data.SourceY, 1, 1);
    }

    private DefinitionsTileData MakeSetTile(int x = 0, int y = 0)
    {
        if (x == 0) x = Math.Max(0, TilesSelection.X);
        if (y == 0) y = Math.Max(0, TilesSelection.Y);
        return new DefinitionsTileData(
            (byte)(cmbTiles.SelectedIndex + 1),
            x, y,
            AutoTile,
            new NoAttribute(),
            _paintLayer
        );
    }

    private static ChunkCoord TileToChunk(int tileX, int tileY) =>
        new(
            (short)(tileX >= 0 ? tileX / ChunkSize : (tileX - ChunkSize + 1) / ChunkSize),
            (short)(tileY >= 0 ? tileY / ChunkSize : (tileY - ChunkSize + 1) / ChunkSize));

    // ── TILE SHEET ────────────────────────────────────────────────────

    private void imgTile_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(imgTile).Properties.IsLeftButtonPressed) return;
        var pt = e.GetPosition(imgTile);
        var x = (int)(pt.X + scrlTileX.Value) / Grid;
        var y = (int)(pt.Y + scrlTileY.Value) / Grid;
        if (cmbTiles.SelectedIndex < 0) return;
        var tex = Textures.Tiles[cmbTiles.SelectedIndex + 1];
        if ((int)(pt.X + scrlTileX.Value) > tex.ToSize().Width) return;
        if ((int)(pt.Y + scrlTileY.Value) > tex.ToSize().Height) return;
        _defTilesSelection.Location = new SystemPoint(x, y);
        UpdateTileSelected();
    }

    private void imgTile_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (cmbTiles.SelectedIndex < 0) return;
        var pt = e.GetPosition(imgTile);
        var x = (int)(pt.X + scrlTileX.Value) / Grid;
        var y = (int)(pt.Y + scrlTileY.Value) / Grid;
        var tex = Textures.Tiles[cmbTiles.SelectedIndex + 1];
        var size = tex.ToSize();
        TileMouse = new SystemPoint(x * Grid - (int)scrlTileX.Value, y * Grid - (int)scrlTileY.Value);
        if (!e.GetCurrentPoint(imgTile).Properties.IsLeftButtonPressed) return;
        if (AutoTile) return;
        x = Math.Clamp(x, 0, size.Width / Grid - 1);
        y = Math.Clamp(y, 0, size.Height / Grid - 1);
        _defTilesSelection.Width = x - _defTilesSelection.X + 1;
        _defTilesSelection.Height = y - _defTilesSelection.Y + 1;
    }

    private void cmbTiles_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        scrlTileX.Value = 0; scrlTileY.Value = 0; chkAuto.IsChecked = false;
        UpdateTileBounds(); TileMouse = new SystemPoint(0);
        _defTilesSelection = new SystemRect(0, 0, 1, 1);
        ResetMapSelectionSize();
    }

    private void chkAuto_IsCheckedChanged(object? sender, RoutedEventArgs e) => UpdateTileSelected();

    private void UpdateTileSelected() => _defTilesSelection.Size = AutoTile ? new SystemSize(2, 3) : new SystemSize(1, 1);

    private void UpdateTileBounds()
    {
        if (cmbTiles.SelectedIndex < 0) return;
        var size = Textures.Tiles[cmbTiles.SelectedIndex + 1].ToSize();
        scrlTileX.Maximum = Math.Max(0, size.Width - TileCanvasWidth);
        scrlTileY.Maximum = Math.Max(0, size.Height - TileCanvasHeight);
    }

    // ── ATTRIBUTES ────────────────────────────────────────────────────

    private TileAttributeUnion GetSelectedAttribute()
    {
        if (optA_Block.IsChecked == true) return new BlockedTile();
        if (optA_Warp.IsChecked == true)
        {
            var targetMap = cmbA_Warp_Map.SelectedItem as Map;
            return new WarpTile(
                targetMap?.Id ?? Guid.Empty,
                (int)(numA_Warp_X.Value ?? 0),
                (int)(numA_Warp_Y.Value ?? 0)
            );
        }
        if (optA_Item.IsChecked == true)
        {
            var item = cmbA_Item.SelectedItem as Item;
            return new ItemTile(item?.Id ?? Guid.Empty, (short)(numA_Item_Amount.Value ?? 1));
        }
        return new NoAttribute();
    }

    private void SetAttribute()
    {
        if (_selected == null) return;
        var sel = MapSelection;
        var coord = TileToChunk(sel.X, sel.Y);
        var lx = ((sel.X % ChunkSize) + ChunkSize) % ChunkSize;
        var ly = ((sel.Y % ChunkSize) + ChunkSize) % ChunkSize;
        if (!_selected.Chunks.TryGetValue(coord, out var chunk) || chunk.Tiles == null) return;
        var tile = chunk.Tiles[lx, ly];
        if (tile == null) return;
        chunk.Tiles[lx, ly] = tile with { Attribute = GetSelectedAttribute() };
    }

    private void ClearAttribute()
    {
        if (_selected == null) return;
        var sel = MapSelection;
        var coord = TileToChunk(sel.X, sel.Y);
        var lx = ((sel.X % ChunkSize) + ChunkSize) % ChunkSize;
        var ly = ((sel.Y % ChunkSize) + ChunkSize) % ChunkSize;
        if (!_selected.Chunks.TryGetValue(coord, out var chunk) || chunk.Tiles == null) return;
        var tile = chunk.Tiles[lx, ly];
        if (tile == null) return;
        chunk.Tiles[lx, ly] = tile with { Attribute = new NoAttribute() };
    }

    private void optA_Warp_Changed(object? sender, RoutedEventArgs e)
    {
        grpA_Warp.IsVisible = optA_Warp.IsChecked == true;
        if (optA_Warp.IsChecked == true)
        {
            if (cmbA_Warp_Map.Items.Count > 0) cmbA_Warp_Map.SelectedIndex = 0;
            numA_Warp_X.Value = 0; numA_Warp_Y.Value = 0;
        }
    }

    private void optA_Item_Changed(object? sender, RoutedEventArgs e)
    {
        if (optA_Item.IsChecked == true)
        {
            if (_catalog.Items.Count == 0)
            { MessageBox.Show("It must have at least one item registered to use this attribute."); optA_Block.IsChecked = true; return; }
            cmbA_Item.Items.Clear();
            foreach (var item in _catalog.Items.Values) cmbA_Item.Items.Add(item);
            if (cmbA_Item.Items.Count > 0) cmbA_Item.SelectedIndex = 0;
            numA_Item_Amount.Value = 1;
        }
        grpA_Item.IsVisible = optA_Item.IsChecked == true;
    }

    private void cmbA_Warp_Map_SelectionChanged(object? sender, SelectionChangedEventArgs e) { }
    private void numA_Warp_X_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e) { }
    private void numA_Warp_Y_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e) { }
    private void cmbA_Item_SelectionChanged(object? sender, SelectionChangedEventArgs e) { }
    private void numA_Item_Amount_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e) { }

    private void butAttributes_Clear_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        foreach (var (coord, chunk) in _selected.Chunks)
        {
            if (chunk.Tiles == null) continue;
            for (var x = 0; x < ChunkSize; x++)
                for (var y = 0; y < ChunkSize; y++)
                {
                    var t = chunk.Tiles[x, y];
                    if (t != null) chunk.Tiles[x, y] = t with { Attribute = new NoAttribute() };
                }
        }
    }

    private void butAttributes_Import_Click(object? sender, RoutedEventArgs e)
    {
        // Import from tile sheet metadata — disabled for now
    }

    // ── ZONES (kept for backward compat, maps to SpawnTile) ────────────

    private void scrlZone_Scroll(object? sender, AvaloniaScrollEventArgs e)
    {
        var v = (int)layersPane.ScrlZone.Value;
        layersPane.LblZone.Text = v == 0 ? "Zone: None" : "Zone: " + v;
    }

    private void scrlZone_Clear_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        foreach (var (coord, chunk) in _selected.Chunks)
        {
            if (chunk.Tiles == null) continue;
            for (var x = 0; x < ChunkSize; x++)
                for (var y = 0; y < ChunkSize; y++)
                {
                    var t = chunk.Tiles[x, y];
                    if (t?.Attribute is SpawnTile) chunk.Tiles[x, y] = t with { Attribute = new NoAttribute() };
                }
        }
    }

    // ── NPC LIST ──────────────────────────────────────────────────────

    private void RefreshNpcList()
    {
        if (_selected == null) return;
        lstNPC.ItemsSource = null;
        lstNPC.ItemsSource = _selected.Npc;
    }

    private void AddNpc(bool fixedSpawn = false, int x = 0, int y = 0)
    {
        if (_selected == null || cmbNPC.SelectedItem is not Npc npc) return;
        _selected.Npc.Add(new MapNpc { NpcId = npc.Id, Zone = (byte)(numNPC_Zone.Value ?? 0), Spawn = fixedSpawn, X = x, Y = y });
        RefreshNpcList();
    }

    private void butNPC_Add_Click(object? sender, RoutedEventArgs e) => AddNpc();
    private void butNPC_Remove_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || lstNPC.SelectedIndex < 0) return;
        _selected.Npc.RemoveAt(lstNPC.SelectedIndex);
        RefreshNpcList();
    }
    private void butNPC_Clear_Click(object? sender, RoutedEventArgs e) { _selected?.Npc.Clear(); RefreshNpcList(); }

    // ── LAYERS STUBS (removed, kept for AXAML wiring) ─────────────────

    private void lstLayers_SelectionChanged(object? sender, SelectionChangedEventArgs e) { }
    private void butLayers_Add_Click(object? sender, RoutedEventArgs e) { }
    private void butLayers_Remove_Click(object? sender, RoutedEventArgs e) { }
    private void butLayers_Edit_Click(object? sender, RoutedEventArgs e) { }
    private void butLayers_Up_Click(object? sender, RoutedEventArgs e) { }
    private void butLayers_Down_Click(object? sender, RoutedEventArgs e) { }
    private void butLayer_Ok_Click(object? sender, RoutedEventArgs e) { }
    private void butLayer_Cancel_Click(object? sender, RoutedEventArgs e) { }

    // ── UTILS ─────────────────────────────────────────────────────────

    private static SystemRect SelectionRec(SystemRect t)
    {
        if (t.Width <= 0) { t.X += t.Width - 1; t.Width = (t.Width - 2) * -1; }
        if (t.Height <= 0) { t.Y += t.Height - 1; t.Height = (t.Height - 2) * -1; }
        return t;
    }

    public SystemRect TilesSelection => SelectionRec(_defTilesSelection);

    public SystemRect MapSelection
    {
        get
        {
            if (AutoTile) return new SystemRect(_mapMouse, new SystemSize(1, 1));
            if (ModeNormal && ToolPencil) return new SystemRect(_mapMouse, TilesSelection.Size);
            return SelectionRec(_defMapSelection);
        }
    }

    public SystemRect TileSource => new(TilesSelection.X * Grid, TilesSelection.Y * Grid,
        TilesSelection.Width * Grid, TilesSelection.Height * Grid);

    public byte Zoom()
    {
        if (butZoom_2x.IsChecked == true) return 2;
        if (butZoom_4x.IsChecked == true) return 4;
        return 1;
    }

    public byte GridZoom => (byte)(Grid / Zoom());

    public SystemRect ZoomRect(SystemRect value) => new(value.X / Zoom(), value.Y / Zoom(), value.Width / Zoom(), value.Height / Zoom());
    public SystemPoint ZoomGrid(int x, int y) => new(x * GridZoom, y * GridZoom);

    public bool IsLayerVisible(int index) => (Layer)index == _paintLayer;

    private void UpdateMapBounds()
    {
        if (_selected == null || _selected.Chunks.Count == 0) return;

        var minX = _selected.Chunks.Keys.Min(c => c.X);
        var maxX = _selected.Chunks.Keys.Max(c => c.X);
        var minY = _selected.Chunks.Keys.Min(c => c.Y);
        var maxY = _selected.Chunks.Keys.Max(c => c.Y);

        const int margin = 2;
        scrlMapX.Minimum = (minX - margin) * ChunkSize;
        scrlMapX.Maximum = (maxX + margin + 1) * ChunkSize;
        scrlMapY.Minimum = (minY - margin) * ChunkSize;
        scrlMapY.Maximum = (maxY + margin + 1) * ChunkSize;
    }

    private void ResetMapSelectionSize() => _defMapSelection.Size = new SystemSize(1, 1);
    private static bool IsToolEnabled(ToggleButton btn) => btn.IsEnabled && btn.IsChecked == true;
}
