using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Definitions.Common;
using CryBits.Definitions.Maps;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Graphics.Renderers;
using SFML.Graphics;
using SFML.System;
using static CryBits.Editors.Logic.Utils;
using G = CryBits.Definitions.Globals;
using Point = System.Drawing.Point;

namespace CryBits.Editors.Forms.Tiles;

internal partial class EditorTilesWindow : Window
{
    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorTilesWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private const int CanvasW = 298;
    private const int CanvasH = 443;

    private TileEditorViewModel? _viewModel;
    private WriteableBitmap? _bitmap;
    private readonly DispatcherTimer? _timer;

    public EditorTilesWindow()
    {
        InitializeComponent();

        _viewModel = new TileEditorViewModel();
        DataContext = _viewModel;

        scrlTileX.Value = 0;
        scrlTileY.Value = 0;

        TileRenderer.Instance.WinTile = new RenderTexture(new Vector2u(CanvasW, CanvasH));

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        TileRenderer.Instance.WinTile = null;
        base.OnClosed(e);
    }

    private void OnRenderTick(object? sender, EventArgs e)
    {
        if (TileRenderer.Instance.WinTile == null || _viewModel == null) return;

        TileRenderer.Instance.Tile(_viewModel.TileIndex, _viewModel.ScrollX, _viewModel.ScrollY, _viewModel.IsAttributeMode);
        SfmlRenderBlit.Blit(TileRenderer.Instance.WinTile, ref _bitmap, imgCanvas);
    }

    private void imgCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_viewModel == null) return;
        if (Textures.Tiles.Count == 0 || _viewModel.TileIndex >= Textures.Tiles.Count) return;

        var pt = e.GetPosition(imgCanvas);
        var ex = (int)pt.X;
        var ey = (int)pt.Y;

        var position = new Point((ex + _viewModel.ScrollX * G.Grid) / G.Grid, (ey + _viewModel.ScrollY * G.Grid) / G.Grid);
        var tileDif = new Point(ex - ex / G.Grid * G.Grid, ey - ey / G.Grid * G.Grid);

        var tileRef = Tile.List[_viewModel.TileIndex];
        if (position.X > tileRef.Data.GetUpperBound(0)) return;
        if (position.Y > tileRef.Data.GetUpperBound(1)) return;

        var isLeft = e.GetCurrentPoint(imgCanvas).Properties.IsLeftButtonPressed;

        if (_viewModel.IsAttributeMode)
        {
            if (isLeft)
                tileRef.Data[position.X, position.Y].Attribute = (byte)TileAttribute.Block;
            else
                tileRef.Data[position.X, position.Y].Attribute = 0;
        }
        else
        {
            for (byte i = 0; i < (byte)Direction.Count; i++)
            {
                var bp = Block_Position(i);
                if (tileDif.X >= bp.X && tileDif.X <= bp.X + 8)
                    if (tileDif.Y >= bp.Y && tileDif.Y <= bp.Y + 8)
                        if (tileRef.Data[position.X, position.Y].Attribute != (byte)TileAttribute.Block)
                            tileRef.Data[position.X, position.Y].Block[i] = !tileRef.Data[position.X, position.Y].Block[i];
            }
        }
    }

    private void butSave_Click(object? sender, RoutedEventArgs e)
    {
        TileRepository.WriteAll();
        Close();
    }

    private void butClear_Click(object? sender, RoutedEventArgs e)
    {
        if (_viewModel == null) return;
        if (Textures.Tiles.Count == 0 || _viewModel.TileIndex >= Textures.Tiles.Count) return;
        var tileSize = Textures.Tiles[_viewModel.TileIndex].ToSize();
        Tile.List[_viewModel.TileIndex] = new Tile(tileSize);
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
