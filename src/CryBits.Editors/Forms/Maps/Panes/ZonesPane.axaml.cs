using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using CryBits.Definitions.Maps;

namespace CryBits.Editors.Forms.Maps.Panes;

internal partial class ZonesPane : UserControl
{
    public ZonesPane()
    {
        InitializeComponent();
        scrlZone.Maximum = Definitions.Globals.MaxZones;
        scrlZone.Scroll += (_, _) => UpdateLabel();
        butZonesClear.Click += OnClear;
        UpdateLabel();
    }

    public Map? SelectedMap { get; set; }
    public ScrollBar ScrlZone => scrlZone;

    private void UpdateLabel()
    {
        var v = (int)scrlZone.Value;
        lblZone.Text = v == 0 ? "Zone: None" : "Zone: " + v;
    }

    private void OnClear(object? sender, RoutedEventArgs e)
    {
        if (SelectedMap == null) return;
        foreach (var (_, chunk) in SelectedMap.Chunks)
        {
            if (chunk.Tiles == null) continue;
            for (var x = 0; x < 32; x++)
                for (var y = 0; y < 32; y++)
                {
                    var t = chunk.Tiles[x, y];
                    if (t?.Attribute is SpawnTile) chunk.Tiles[x, y] = t with { Attribute = new NoAttribute() };
                }
        }
    }
}
