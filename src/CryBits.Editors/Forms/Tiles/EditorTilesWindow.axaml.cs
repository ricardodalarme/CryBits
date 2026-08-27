using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Definitions.Common;
using CryBits.Definitions.Maps;
using CryBits.Editors.Graphics;
using CryBits.Editors.Graphics.Renderers;
using static CryBits.Editors.Logic.Utils;
using G = CryBits.Definitions.Globals;
using SysPoint = System.Drawing.Point;

namespace CryBits.Editors.Forms.Tiles;

internal partial class EditorTilesWindow : Window
{
    public static void Open(Window owner, TileRenderer tileRenderer)
    {
        owner.Hide();
        var window = new EditorTilesWindow(tileRenderer);
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private const int CanvasW = 298;
    private const int CanvasH = 443;

    private readonly TileRenderer _tileRenderer;
    private readonly TileEditorViewModel _viewModel;
    private readonly DispatcherTimer _timer;

    public EditorTilesWindow(TileRenderer tileRenderer)
    {
        _tileRenderer = tileRenderer;
        InitializeComponent();

        _viewModel = new TileEditorViewModel();
        DataContext = _viewModel;
        _viewModel.RequestClose += Close;

        scrlTileX.Value = 0;
        scrlTileY.Value = 0;

        _tileRenderer.WinTile = new Microsoft.Xna.Framework.Graphics.RenderTarget2D(
            EditorGraphics.Device, CanvasW, CanvasH);

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer.Stop();
        _tileRenderer.WinTile?.Dispose();
        _tileRenderer.WinTile = null;
        base.OnClosed(e);
    }

    private void OnRenderTick(object? sender, EventArgs e)
    {
        EditorGraphics.Tick();
        if (_tileRenderer.WinTile == null || _viewModel == null) return;

        EditorGraphics.Device.SetRenderTarget(_tileRenderer.WinTile);
        EditorGraphics.Device.Clear(Microsoft.Xna.Framework.Color.Black);
        EditorGraphics.SpriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred, Microsoft.Xna.Framework.Graphics.BlendState.NonPremultiplied, Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp, null, null);
        _tileRenderer.Tile(_viewModel.TileIndex, _viewModel.ScrollX, _viewModel.ScrollY, _viewModel.IsAttributeMode);
        EditorGraphics.SpriteBatch.End();
        EditorGraphics.Device.SetRenderTarget(null);

        imgCanvas.BlitRenderTarget(_tileRenderer.WinTile);
    }

    private void imgCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Textures.Tiles.Count == 0 || _viewModel.TileIndex >= Textures.Tiles.Count) return;

        var pt = e.GetPosition(imgCanvas);
        var ex = (int)pt.X;
        var ey = (int)pt.Y;

        var position = new SysPoint((ex + (_viewModel.ScrollX * G.Grid)) / G.Grid,
            (ey + (_viewModel.ScrollY * G.Grid)) / G.Grid);
        var tileDif = new SysPoint(ex - (ex / G.Grid * G.Grid), ey - (ey / G.Grid * G.Grid));

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
                            tileRef.Data[position.X, position.Y].Block[i] =
                                !tileRef.Data[position.X, position.Y].Block[i];
            }
        }
    }
}
