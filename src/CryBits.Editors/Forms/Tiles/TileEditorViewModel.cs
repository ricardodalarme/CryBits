using CryBits.Client.Framework.Graphics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using G = CryBits.Definitions.Globals;

namespace CryBits.Editors.Forms.Tiles;

internal sealed class TileEditorViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Notify([CallerMemberName] string? n = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    private int _tileIndex = 1;
    private int _scrollX;
    private int _scrollY;
    private bool _isAttributeMode = true;

    public int TileIndex
    {
        get => _tileIndex;
        set
        {
            if (_tileIndex == value) return;
            _tileIndex = value;
            Notify();
            Notify(nameof(TileLabel));
            ScrollX = 0;
            ScrollY = 0;
            UpdateScrollBounds();
        }
    }

    public string TileLabel => "Tile: " + _tileIndex;

    public int ScrollX
    {
        get => _scrollX;
        set { _scrollX = value; Notify(); }
    }

    public int ScrollY
    {
        get => _scrollY;
        set { _scrollY = value; Notify(); }
    }

    public bool IsAttributeMode
    {
        get => _isAttributeMode;
        set { _isAttributeMode = value; Notify(); Notify(nameof(ShowAttributes)); }
    }

    public bool ShowAttributes => _isAttributeMode;

    public int MaxScrollBoundsX { get; private set; }
    public int MaxScrollBoundsY { get; private set; }

    public int MaxTileIndex => Math.Max(1, Textures.Tiles.Count - 1);

    private void UpdateScrollBounds()
    {
        if (Textures.Tiles.Count == 0 || _tileIndex >= Textures.Tiles.Count)
        {
            MaxScrollBoundsX = 0;
            MaxScrollBoundsY = 0;
            return;
        }

        var tex = Textures.Tiles[_tileIndex];
        const int canvasW = 298;
        const int canvasH = 443;
        MaxScrollBoundsX = Math.Max(0, tex.ToSize().Width / G.Grid - canvasW / G.Grid);
        MaxScrollBoundsY = Math.Max(0, tex.ToSize().Height / G.Grid - canvasH / G.Grid);
    }
}
