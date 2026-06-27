using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Definitions.Maps;
using CryBits.Editors.Forms.Maps.Models;
using System.Collections.ObjectModel;

namespace CryBits.Editors.Forms.Maps.Panes;

internal partial class LayersPane : UserControl
{
    public LayersPane()
    {
        InitializeComponent();
        WireEvents();
    }

    public Map? SelectedMap
    {
        get => _selectedMap;
        set
        {
            _selectedMap = value;
            chunksPane.SelectedMap = value;
        }
    }
    private Map? _selectedMap;

    // ── Public API ──────────────────────────────────────────────────

    public Layer PaintLayer { get; private set; } = Layer.Ground;
    public IReadOnlyList<LayerVm> Layers => _layers;
    public ChunksPane ChunksPane => chunksPane;

    public event Action? PaintLayerChanged;
    public event Action? LayerListChanged;

    public void RefreshChunkList() => chunksPane.RefreshList();

    public void InitLayers()
    {
        lstLayers.ItemsSource = _layers;
        _layers.Clear();
        _layers.Add(new LayerVm { Layer = Layer.Ground, Name = "Ground", TypeName = "Ground", Index = 0 });
        _layers.Add(new LayerVm { Layer = Layer.Fringe, Name = "Fringe", TypeName = "Fringe", Index = 1 });
        PaintLayer = Layer.Ground;
        if (lstLayers.SelectedItem == null && _layers.Count > 0)
            lstLayers.SelectedIndex = 0;
    }

    public void RefreshLayerList()
    {
        if (lstLayers.SelectedItem == null && _layers.Count > 0)
            lstLayers.SelectedIndex = 0;
    }

    // ── Internal state ──────────────────────────────────────────────

    private readonly ObservableCollection<LayerVm> _layers = [];

    // ── Event wiring (constructor) ──────────────────────────────────

    private void WireEvents()
    {
        lstLayers.SelectionChanged += OnLayerSelectionChanged;
        butLayers_Add.Click += OnAddLayer;
        butLayers_Edit.Click += OnEditLayer;
        butLayer_Ok.Click += OnLayerOk;
        butLayer_Cancel.Click += (_, _) => pnlLayerEdit.IsVisible = false;
        butLayers_Remove.Click += OnRemoveLayer;
        butLayers_Up.Click += OnLayerUp;
        butLayers_Down.Click += OnLayerDown;
    }

    private void OnLayerSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (lstLayers.SelectedItem is LayerVm lvm)
            PaintLayer = lvm.Layer;
        PaintLayerChanged?.Invoke();
    }

    private void ShowEditPanel(bool isNew)
    {
        pnlLayerEdit.IsVisible = true;
        lblLayerEditTitle.Text = isNew ? "Add layer" : "Edit layer";
        butLayer_Ok.Tag = isNew;
        cmbLayers_Type.Items.Clear();
        foreach (var l in new[] { Layer.Ground, Layer.Fringe })
            cmbLayers_Type.Items.Add(l.ToString());
        cmbLayers_Type.SelectedIndex = 0;
        txtLayer_Name.Text = string.Empty;
    }

    private void OnAddLayer(object? sender, RoutedEventArgs e) => ShowEditPanel(true);

    private void OnEditLayer(object? sender, RoutedEventArgs e)
    {
        if (lstLayers.SelectedItem is not LayerVm sel) return;
        ShowEditPanel(false);
        txtLayer_Name.Text = sel.Name;
        cmbLayers_Type.SelectedItem = sel.Layer.ToString();
    }

    private void OnLayerOk(object? sender, RoutedEventArgs e)
    {
        var name = (txtLayer_Name.Text ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(name)) return;
        var typeStr = cmbLayers_Type.SelectedItem as string ?? Layer.Ground.ToString();
        var layerType = typeStr == Layer.Fringe.ToString() ? Layer.Fringe : Layer.Ground;

        if (butLayer_Ok.Tag is true)
        {
            _layers.Add(new LayerVm { Layer = layerType, Name = name, TypeName = typeStr, Index = _layers.Count });
        }
        else if (lstLayers.SelectedItem is LayerVm sel)
        {
            sel.Name = name;
            sel.TypeName = typeStr;
        }

        RefreshLayerList();
        pnlLayerEdit.IsVisible = false;
        LayerListChanged?.Invoke();
    }

    private void OnRemoveLayer(object? sender, RoutedEventArgs e)
    {
        if (lstLayers.SelectedItem is not LayerVm sel) return;
        if (_layers.Count <= 1) return;
        _layers.Remove(sel);
        for (var i = 0; i < _layers.Count; i++) _layers[i].Index = i;
        RefreshLayerList();
        LayerListChanged?.Invoke();
    }

    private void SwapLayer(int from, int to)
    {
        if (from < 0 || from >= _layers.Count || to < 0 || to >= _layers.Count) return;
        (_layers[from], _layers[to]) = (_layers[to], _layers[from]);
        _layers[from].Index = from;
        _layers[to].Index = to;
        RefreshLayerList();
        lstLayers.SelectedIndex = to;
    }

    private void OnLayerUp(object? sender, RoutedEventArgs e)
    {
        var idx = lstLayers.SelectedIndex;
        SwapLayer(idx, idx - 1);
    }

    private void OnLayerDown(object? sender, RoutedEventArgs e)
    {
        var idx = lstLayers.SelectedIndex;
        SwapLayer(idx, idx + 1);
    }
}
