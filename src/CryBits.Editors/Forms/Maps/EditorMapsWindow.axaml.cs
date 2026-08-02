using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Definitions;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms.Maps.Models;
using CryBits.Editors.Forms.Maps.Panes;
using CryBits.Editors.Graphics.Renderers;
using SFML.Graphics;
using SFML.System;
using System.ComponentModel;
using DefinitionsTileData = CryBits.Definitions.Maps.TileData;
using SystemPoint = System.Drawing.Point;
using SystemRect = System.Drawing.Rectangle;

namespace CryBits.Editors.Forms.Maps;

internal partial class EditorMapsWindow : Window
{
    private readonly DefinitionCatalog _catalog;

    public static void Open()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var window = new EditorMapsWindow(Program.Catalog);
            window.Show();
        });
    }

    public static void Open(Window parent)
    {
        Dispatcher.UIThread.Post(async () =>
        {
            parent.Hide();
            var window = new EditorMapsWindow(Program.Catalog);
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

    // Forwarded to toolbar pane
    public bool ModeNormal => toolbarPane.ModeNormal;
    public bool ModeAttributes => toolbarPane.ModeAttributes;
    public bool ModeNPCs => toolbarPane.ModeNPCs;
    public bool ToolPencil => toolbarPane.ToolPencil;
    public bool ToolRectangle => toolbarPane.ToolRectangle;
    public bool ShowGrid => toolbarPane.ShowGrid;

    // Forwarded to map canvas pane
    public SystemRect MapSelection => mapCanvasPane.MapSelection;
    public SystemRect TileSource => mapCanvasPane.TileSource;
    public int MapScrollX => mapCanvasPane.ViewportTileX;
    public int MapScrollY => mapCanvasPane.ViewportTileY;

    public int MapCanvasWidth
    {
        get
        {
            var w = (int)mapCanvasPane.ZoomBorder.Bounds.Width;
            return w > 0 ? w : 800;
        }
    }

    public int MapCanvasHeight
    {
        get
        {
            var h = (int)mapCanvasPane.ZoomBorder.Bounds.Height;
            return h > 0 ? h : 600;
        }
    }

    // Delegated to tile sheet pane
    public bool AutoTile => tileSheetPane.IsAutoTile;
    public int TileSheetIndex => tileSheetPane.SelectedTileIndex;
    public int TileScrollX => tileSheetPane.TileScrollX;
    public int TileScrollY => tileSheetPane.TileScrollY;
    public SystemPoint TileMouse => tileSheetPane.TileMousePosition;

    private int TileCanvasWidth => Math.Max(1, (int)tileSheetPane.TileViewport.Bounds.Width);
    private int TileCanvasHeight => Math.Max(1, (int)tileSheetPane.TileViewport.Bounds.Height);

    public Map? SelectedMap { get; private set; }

    private MapProperties? _mapProps;
    private Layer _paintLayer = Layer.Ground;
    private DefinitionsTileData[,]? _clipboardData;

    // Pane instances
    private readonly TileSheetPane tileSheetPane;
    private readonly LayersPane layersPane;
    private readonly AttributesPane attributesPane;
    private readonly NpcPane npcPane;
    private readonly ZonesPane zonesPane;
    private readonly MapCanvasPane mapCanvasPane;
    private readonly MapExplorerPane explorerPane;
    private readonly PropertiesPane propertiesPane;
    private readonly Grid _leftPanelRoot;
    private readonly Grid _normalView;

    private readonly DispatcherTimer? _timer;

    public EditorMapsWindow(DefinitionCatalog catalog)
    {
        _catalog = catalog;
        InitializeComponent();
        Instance = this;

        tileSheetPane = new TileSheetPane();
        layersPane = new LayersPane();
        attributesPane = new AttributesPane();
        npcPane = new NpcPane();
        zonesPane = new ZonesPane();
        mapCanvasPane = new MapCanvasPane();
        explorerPane = new MapExplorerPane { Catalog = _catalog };
        propertiesPane = new PropertiesPane();

        // Build the left panel content
        _normalView = new Grid();
        _normalView.RowDefinitions.Add(new RowDefinition(new GridLength(3, GridUnitType.Star)));
        _normalView.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        _normalView.RowDefinitions.Add(new RowDefinition(new GridLength(2, GridUnitType.Star)));
        _normalView.Children.Add(tileSheetPane);
        Grid.SetRow(tileSheetPane, 0);
        var splitter = new GridSplitter { Height = 4, ResizeDirection = GridResizeDirection.Rows };
        _normalView.Children.Add(splitter);
        Grid.SetRow(splitter, 1);
        _normalView.Children.Add(layersPane);
        Grid.SetRow(layersPane, 2);

        _leftPanelRoot = new Grid();
        _leftPanelRoot.Children.Add(_normalView);
        _leftPanelRoot.Children.Add(attributesPane);
        _leftPanelRoot.Children.Add(npcPane);
        _leftPanelRoot.Children.Add(zonesPane);
        attributesPane.IsVisible = false;
        npcPane.IsVisible = false;
        zonesPane.IsVisible = false;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;

        Loaded += OnLoaded;
    }

    public void SetMode(string mode)
    {
        _normalView.IsVisible = false;
        attributesPane.IsVisible = false;
        npcPane.IsVisible = false;
        zonesPane.IsVisible = false;

        switch (mode)
        {
            case "Normal" or "Tile placement":
                _normalView.IsVisible = true;
                break;
            case "Attributes":
                attributesPane.IsVisible = true;
                break;
            case "NPCs":
                npcPane.IsVisible = true;
                break;
            case "Zones":
                zonesPane.IsVisible = true;
                break;
        }
    }

    private void AssignContentToDockModels()
    {
        if (DockControl?.Layout is not Dock.Model.Avalonia.Controls.RootDock root) return;

        void Walk(IList<Dock.Model.Core.IDockable>? dockables)
        {
            if (dockables == null) return;
            foreach (var d in dockables)
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

        Walk(root.VisibleDockables);
    }

    private void AssignContent(Dock.Model.Core.IDockable dockable)
    {
        switch (dockable.Id)
        {
            case "LeftPanel" when dockable is Dock.Model.Avalonia.Controls.Tool t: t.Content = _leftPanelRoot; break;
            case "MapCanvas"
                when dockable is Dock.Model.Avalonia.Controls.Document doc:
                doc.Content = mapCanvasPane; break;
            case "MapExplorer" when dockable is Dock.Model.Avalonia.Controls.Tool t: t.Content = explorerPane; break;
            case "Properties" when dockable is Dock.Model.Avalonia.Controls.Tool t: t.Content = propertiesPane; break;
        }
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // Wire pane internal handlers
        tileSheetPane.WireHandlers();
        // WireHandlers moved into pane constructors
        explorerPane.WireHandlers();
        mapCanvasPane.WireHandlers();

        // Inject dependencies
        layersPane.ChunksPane.ZoomBorder = mapCanvasPane.ZoomBorder;
        layersPane.ChunksPane.GetSelectedMap = () => SelectedMap;

        // Set canvas dependencies
        mapCanvasPane.Deps = new CanvasDeps
        {
            GetSelectedMap = () => SelectedMap,
            GetPaintLayer = () => _paintLayer,
            ToolbarPane = toolbarPane,
            LayersPane = layersPane,
            TileSheetPane = tileSheetPane,
            AttributesPane = attributesPane,
            NpcPane = npcPane,
            RefreshChunkList = () => layersPane.RefreshChunkList()
        };

        // Wire toolbar pane with dependencies
        toolbarPane.Attach(new ToolbarDeps
        {
            CanvasPane = mapCanvasPane,
            LayersPane = layersPane,
            TileSheetPane = tileSheetPane,
            GetSelectedMap = () => SelectedMap,
            GetMapSelection = () => mapCanvasPane.MapSelection,
            MakeSetTile = () => mapCanvasPane.MakeSetTileForTool(),
            GetClipboard = () => _clipboardData,
            SetClipboard = data => _clipboardData = data,
            PaintTile = (x, y, t) => mapCanvasPane.PaintTileForTool(x, y, t),
            ResetMapSelectionSize = () => mapCanvasPane.ResetMapSelectionSize(),
            SetShowAudio = v => _showAudio = v,
            ParentWindow = this,
            SetLeftPanelMode = mode => SetMode(mode),
            PopulateNpcCombo = () => npcPane.PopulateCombo()
        });

        // Wire cross-pane events
        tileSheetPane.SelectionChanged += () => mapCanvasPane.ResetMapSelectionSize();
        layersPane.PaintLayerChanged += () => _paintLayer = layersPane.PaintLayer;

        // Wire chunk list selection → ZoomBorder
        layersPane.ChunksPane.LstChunks.SelectionChanged += (_, _) =>
        {
            if (layersPane.ChunksPane.LstChunks.SelectedItem is ChunkCoord coord)
                mapCanvasPane.ZoomBorder.CenterOn(new Avalonia.Point(coord.X * MapMath.ChunkSize * Globals.Grid,
                    coord.Y * MapMath.ChunkSize * Globals.Grid));
        };

        // Wire explorer pane events
        explorerPane.MapSelected += SelectMap;

        // Populate tile combos
        tileSheetPane.PopulateTiles();

        // Assign content to Dock model after layout has been initialized
        AssignContentToDockModels();

        MapRenderer.Instance.WinMap = new RenderTexture(new Vector2u((uint)MapCanvasWidth, (uint)MapCanvasHeight));
        MapRenderer.Instance.WinMapTile =
            new RenderTexture(new Vector2u((uint)TileCanvasWidth, (uint)TileCanvasHeight));

        _timer!.Start();
        explorerPane.RefreshList();
        _isOpen = true;
        _showAudio = toolbarPane.ShowAudio;
        _showVisualization = toolbarPane.ShowVisualization;
    }

    protected override void OnClosed(EventArgs e)
    {
        _isOpen = false;
        _timer?.Stop();
        MapRenderer.Instance.WinMap?.Dispose();
        MapRenderer.Instance.WinMap = null;
        MapRenderer.Instance.WinMapTile?.Dispose();
        MapRenderer.Instance.WinMapTile = null;
        _mapBitmap?.Dispose();
        _tileBitmap?.Dispose();
        Instance = null;
        base.OnClosed(e);
    }

    private void OnRenderTick(object? sender, EventArgs e)
    {
        var vw = (int)mapCanvasPane.ZoomBorder.Bounds.Width;
        var vh = (int)mapCanvasPane.ZoomBorder.Bounds.Height;
        if (vw > 0 && vh > 0)
        {
            var winMap = MapRenderer.Instance.WinMap;
            if (winMap == null || winMap.Size.X != vw || winMap.Size.Y != vh)
            {
                winMap?.Dispose();
                MapRenderer.Instance.WinMap = new RenderTexture(new Vector2u((uint)vw, (uint)vh));
            }
        }

        if (MapRenderer.Instance.WinMap != null && SelectedMap != null)
        {
            MapRenderer.Instance.EditorMapsMap();
            SfmlRenderBlit.Blit(MapRenderer.Instance.WinMap, ref _mapBitmap, mapCanvasPane.ImgMap);
        }

        if (ModeNormal)
        {
            var tw = (uint)TileCanvasWidth;
            var th = (uint)TileCanvasHeight;
            var tileMap = MapRenderer.Instance.WinMapTile;
            if (tileMap == null || tileMap.Size.X != tw || tileMap.Size.Y != th)
            {
                tileMap?.Dispose();
                tileMap = new RenderTexture(new Vector2u(tw, th));
                MapRenderer.Instance.WinMapTile = tileMap;
            }

            MapRenderer.Instance.EditorMapsTile();
            SfmlRenderBlit.Blit(tileMap, ref _tileBitmap, tileSheetPane.ImgTile);
        }

        UpdateStatusBar();
    }

    private WriteableBitmap? _mapBitmap;
    private WriteableBitmap? _tileBitmap;

    // ── MAP SELECTION ─────────────────────────────────────────────────

    private void SelectMap(Map map)
    {
        SelectedMap = map;
        layersPane.SelectedMap = map;
        zonesPane.SelectedMap = map;
        attributesPane.SelectedMap = map;
        npcPane.SelectedMap = map;
        layersPane.InitLayers();
        attributesPane.RefreshWarpMapCombo();

        if (_mapProps != null) _mapProps.PropertyChanged -= OnMapPropertyChanged;
        _mapProps = new MapProperties(map);
        _mapProps.PropertyChanged += OnMapPropertyChanged;
        propertiesPane.PrgMapProperties.DataContext = _mapProps;

        npcPane.RefreshList();
        MapInstance.Instance.UpdateWeatherType();
        layersPane.RefreshChunkList();
    }

    private void OnMapPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MapProperties.Name))
            explorerPane.RefreshList(SelectedMap?.Id);
    }

    // ── STATUS BAR ─────────────────────────────────────────────────────

    private void UpdateStatusBar()
    {
        lblFPS.Text = $"FPS: {Program.Fps}";
        var m = mapCanvasPane.MapMouse;
        var cx = m.X / MapMath.ChunkSize;
        var cy = m.Y / MapMath.ChunkSize;
        var lx = ((m.X % MapMath.ChunkSize) + MapMath.ChunkSize) % MapMath.ChunkSize;
        var ly = ((m.Y % MapMath.ChunkSize) + MapMath.ChunkSize) % MapMath.ChunkSize;
        lblPosition.Text = $"Chunk: ({cx},{cy}) Tile: ({lx},{ly}) World: ({m.X},{m.Y})";
    }

    // ── UTILS ─────────────────────────────────────────────────────────

    public bool IsLayerVisible(Layer layer)
    {
        foreach (var lvm in layersPane.Layers)
            if (lvm.Layer == layer)
                return lvm.Visible;
        return true;
    }
}
