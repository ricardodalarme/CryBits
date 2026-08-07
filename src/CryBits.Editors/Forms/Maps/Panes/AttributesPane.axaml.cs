using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Editors.Utils;
using static CryBits.Editors.Forms.Maps.MapMath;
using SystemRect = System.Drawing.Rectangle;

namespace CryBits.Editors.Forms.Maps.Panes;

internal partial class AttributesPane : UserControl
{
    public AttributesPane()
    {
        InitializeComponent();
        optA_Warp.IsCheckedChanged += OnWarpToggled;
        optA_Item.IsCheckedChanged += OnItemToggled;
        butAttributes_Clear.Click += OnClearAttributes;
        butAttributes_Import.Click += (_, _) => { };
    }

    public Map? SelectedMap { get; set; }
    public DefinitionCatalog? Catalog { get; set; }

    public void RefreshWarpMapCombo()
    {
        cmbA_Warp_Map.Items.Clear();
        if (Catalog == null) return;
        foreach (var m in Catalog.Maps.Values) cmbA_Warp_Map.Items.Add(m);
        if (cmbA_Warp_Map.Items.Count > 0) cmbA_Warp_Map.SelectedIndex = 0;
        numA_Warp_X.Maximum = 999;
        numA_Warp_Y.Maximum = 999;
    }

    private TileAttributeUnion GetSelectedAttribute()
    {
        if (optA_Block.IsChecked == true) return new BlockedTile();
        if (optA_Warp.IsChecked == true)
        {
            var targetMap = cmbA_Warp_Map.SelectedItem as Map;
            return new WarpTile(
                targetMap?.Id ?? Guid.Empty,
                (int)(numA_Warp_X.Value ?? 0),
                (int)(numA_Warp_Y.Value ?? 0));
        }

        if (optA_Item.IsChecked == true)
        {
            var item = cmbA_Item.SelectedItem as Item;
            return new ItemTile(item?.Id ?? Guid.Empty, (short)(numA_Item_Amount.Value ?? 1));
        }

        return new NoAttribute();
    }

    public void SetAttributeAt(SystemRect mapSelection)
    {
        if (SelectedMap == null) return;
        var coord = TileToChunk(mapSelection.X, mapSelection.Y);
        var lx = ((mapSelection.X % ChunkSize) + ChunkSize) % ChunkSize;
        var ly = ((mapSelection.Y % ChunkSize) + ChunkSize) % ChunkSize;
        if (!SelectedMap.Chunks.TryGetValue(coord, out var chunk) || chunk.Tiles == null) return;
        var tile = chunk.Tiles[lx, ly];
        if (tile == null) return;
        chunk.Tiles[lx, ly] = tile with { Attribute = GetSelectedAttribute() };
    }

    public void ClearAttributeAt(SystemRect mapSelection)
    {
        if (SelectedMap == null) return;
        var coord = TileToChunk(mapSelection.X, mapSelection.Y);
        var lx = ((mapSelection.X % ChunkSize) + ChunkSize) % ChunkSize;
        var ly = ((mapSelection.Y % ChunkSize) + ChunkSize) % ChunkSize;
        if (!SelectedMap.Chunks.TryGetValue(coord, out var chunk) || chunk.Tiles == null) return;
        var tile = chunk.Tiles[lx, ly];
        if (tile == null) return;
        chunk.Tiles[lx, ly] = tile with { Attribute = new NoAttribute() };
    }

    private void OnWarpToggled(object? sender, RoutedEventArgs e)
    {
        grpA_Warp.IsVisible = optA_Warp.IsChecked == true;
        if (optA_Warp.IsChecked == true)
        {
            if (cmbA_Warp_Map.Items.Count > 0) cmbA_Warp_Map.SelectedIndex = 0;
            numA_Warp_X.Value = 0;
            numA_Warp_Y.Value = 0;
        }
    }

    private void OnItemToggled(object? sender, RoutedEventArgs e)
    {
        if (optA_Item.IsChecked == true)
        {
            if (Catalog == null || Catalog.Items.Count == 0)
            {
                MessageBox.Show("It must have at least one item registered to use this attribute.");
                optA_Block.IsChecked = true;
                return;
            }

            cmbA_Item.Items.Clear();
            foreach (var item in Catalog.Items.Values) cmbA_Item.Items.Add(item);
            if (cmbA_Item.Items.Count > 0) cmbA_Item.SelectedIndex = 0;
            numA_Item_Amount.Value = 1;
        }

        grpA_Item.IsVisible = optA_Item.IsChecked == true;
    }

    private void OnClearAttributes(object? sender, RoutedEventArgs e)
    {
        if (SelectedMap == null) return;
        foreach (var (_, chunk) in SelectedMap.Chunks)
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
}
