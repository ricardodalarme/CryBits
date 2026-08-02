using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Client.Framework.Persistence.Repositories;
using G = CryBits.Definitions.Globals;

namespace CryBits.Editors.Forms.Tiles;

internal sealed partial class TileEditorViewModel : ObservableObject
{
    public event Action? RequestClose;

    [ObservableProperty] private int _tileIndex = 1;

    [ObservableProperty] private int _scrollX;

    [ObservableProperty] private int _scrollY;

    [ObservableProperty] private bool _isAttributeMode = true;

    public string TileLabel => "Tile: " + TileIndex;

    public bool ShowAttributes => IsAttributeMode;

    public int MaxScrollBoundsX { get; private set; }
    public int MaxScrollBoundsY { get; private set; }

    public int MaxTileIndex => Math.Max(1, Textures.Tiles.Count - 1);

    partial void OnTileIndexChanged(int value)
    {
        OnPropertyChanged(nameof(TileLabel));
        ScrollX = 0;
        ScrollY = 0;
        UpdateScrollBounds();
    }

    [RelayCommand]
    private void Save()
    {
        TileRepository.WriteAll();
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Clear()
    {
        if (Textures.Tiles.Count == 0 || TileIndex >= Textures.Tiles.Count) return;
        var tileSize = Textures.Tiles[TileIndex].ToSize();
        Tile.List[TileIndex] = new Tile(tileSize);
    }

    [RelayCommand]
    private void Cancel() => RequestClose?.Invoke();

    private void UpdateScrollBounds()
    {
        if (Textures.Tiles.Count == 0 || TileIndex >= Textures.Tiles.Count)
        {
            MaxScrollBoundsX = 0;
            MaxScrollBoundsY = 0;
            return;
        }

        var tex = Textures.Tiles[TileIndex];
        const int canvasW = 298;
        const int canvasH = 443;
        MaxScrollBoundsX = Math.Max(0, (tex.ToSize().Width / G.Grid) - (canvasW / G.Grid));
        MaxScrollBoundsY = Math.Max(0, (tex.ToSize().Height / G.Grid) - (canvasH / G.Grid));
    }
}
