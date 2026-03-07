using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Client.Framework.Graphics;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Graphics;
using CryBits.Editors.ViewModels;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using static CryBits.Editors.Logic.Utils;
using G = CryBits.Globals;
using Point = System.Drawing.Point;

namespace CryBits.Editors.Forms;

internal partial class EditorTilesWindow : Window
{
    /// <summary>Opens the Tiles editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorTilesWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    // Canvas dimensions (matching the WinForms picTile size)
    private const int CanvasW = 298;
    private const int CanvasH = 443;

    private readonly EditorTilesViewModel _vm = new();

    private WriteableBitmap? _bitmap;
    private DispatcherTimer? _timer;

    public EditorTilesWindow()
    {
        DataContext = _vm;
        InitializeComponent();

        // Set tile scroll limits
        scrlTile.Maximum = Math.Max(1, Textures.Tiles.Count - 1);
        scrlTile.Value = 1;
        EditorTilesViewModel.ScrollTile = 1;
        UpdateScrollBounds();

        // SFML offscreen canvas
        Renders.WinTileRT = new RenderTexture(new Vector2u(CanvasW, CanvasH));

        // 30 fps refresh timer
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        Renders.WinTileRT = null;
        base.OnClosed(e);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // SFML render → WriteableBitmap
    // ──────────────────────────────────────────────────────────────────────────

    private void OnRenderTick(object? sender, EventArgs e)
    {
        if (Renders.WinTileRT == null) return;

        Renders.EditorTileRT();
        SfmlRenderBlit.Blit(Renders.WinTileRT, ref _bitmap, imgCanvas);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Tile scroll (selects the tile sheet)
    // ──────────────────────────────────────────────────────────────────────────

    private void scrlTile_Scroll(object? sender, ScrollEventArgs e)
    {
        EditorTilesViewModel.ScrollTile = (int)scrlTile.Value;
        lblTile.Text = "Tile: " + EditorTilesViewModel.ScrollTile;
        scrlTileX.Value = 0;
        scrlTileY.Value = 0;
        EditorTilesViewModel.ScrollX = 0;
        EditorTilesViewModel.ScrollY = 0;
        UpdateScrollBounds();
    }

    private void UpdateScrollBounds()
    {
        if (Textures.Tiles.Count == 0 || EditorTilesViewModel.ScrollTile >= Textures.Tiles.Count) return;

        var tex = Textures.Tiles[EditorTilesViewModel.ScrollTile];
        var maxX = tex.ToSize().Width / G.Grid - CanvasW / G.Grid;
        var maxY = tex.ToSize().Height / G.Grid - CanvasH / G.Grid;

        scrlTileX.Maximum = Math.Max(0, maxX);
        scrlTileY.Maximum = Math.Max(0, maxY);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // X / Y canvas scroll
    // ──────────────────────────────────────────────────────────────────────────

    private void scrlXY_Scroll(object? sender, ScrollEventArgs e)
    {
        EditorTilesViewModel.ScrollX = (int)scrlTileX.Value;
        EditorTilesViewModel.ScrollY = (int)scrlTileY.Value;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Mode toggle (Attributes / Dir. Block)
    // ──────────────────────────────────────────────────────────────────────────

    private void optMode_Changed(object? sender, RoutedEventArgs e)
    {
        EditorTilesViewModel.ModeAttributes = optAttributes.IsChecked ?? true;
        pnlAttributes.IsVisible = EditorTilesViewModel.ModeAttributes;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Canvas click → paint attribute / dir-block
    // ──────────────────────────────────────────────────────────────────────────

    private void imgCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Textures.Tiles.Count == 0 || EditorTilesViewModel.ScrollTile >= Textures.Tiles.Count) return;

        var pt = e.GetPosition(imgCanvas);
        var ex = (int)pt.X;
        var ey = (int)pt.Y;

        var position = new Point((ex + EditorTilesViewModel.ScrollX * G.Grid) / G.Grid, (ey + EditorTilesViewModel.ScrollY * G.Grid) / G.Grid);
        var tileDif = new Point(ex - ex / G.Grid * G.Grid, ey - ey / G.Grid * G.Grid);

        var tileRef = Tile.List[EditorTilesViewModel.ScrollTile];
        if (position.X > tileRef.Data.GetUpperBound(0)) return;
        if (position.Y > tileRef.Data.GetUpperBound(1)) return;

        var isLeft = e.GetCurrentPoint(imgCanvas).Properties.IsLeftButtonPressed;

        if (EditorTilesViewModel.ModeAttributes)
        {
            // Only Block attribute exists currently
            if (isLeft)
                tileRef.Data[position.X, position.Y].Attribute = (byte)TileAttribute.Block;
            else
                tileRef.Data[position.X, position.Y].Attribute = 0;
        }
        else
        {
            // Dir. block mode
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

    // ──────────────────────────────────────────────────────────────────────────
    // Save / Clear / Cancel
    // ──────────────────────────────────────────────────────────────────────────

    private void butSave_Click(object? sender, RoutedEventArgs e)
    {
        _vm.Save();
        Close();
    }

    private void butClear_Click(object? sender, RoutedEventArgs e)
    {
        if (Textures.Tiles.Count == 0 || EditorTilesViewModel.ScrollTile >= Textures.Tiles.Count) return;
        var tileSize = Textures.Tiles[EditorTilesViewModel.ScrollTile].ToSize();
        Tile.List[EditorTilesViewModel.ScrollTile] = new Tile(tileSize);
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
