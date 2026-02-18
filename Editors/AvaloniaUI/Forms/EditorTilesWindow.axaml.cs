using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Library;
using CryBits.Editors.Graphics;
using CryBits.Enums;
using SFML.Graphics;
using static CryBits.Editors.Logic.Utils;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using G = CryBits.Globals;

namespace CryBits.Editors.AvaloniaUI.Forms;

internal partial class EditorTilesWindow : Window
{
    // Canvas dimensions (matching the WinForms picTile size)
    private const int CanvasW = 298;
    private const int CanvasH = 443;

    // How the render is read back by Renders.EditorTileRT()
    public static int ScrollTile { get; private set; } = 1;
    public static int ScrollX { get; private set; }
    public static int ScrollY { get; private set; }
    public static bool ModeAttributes { get; private set; } = true;

    private WriteableBitmap? _bitmap;
    private DispatcherTimer? _timer;

    public EditorTilesWindow()
    {
        InitializeComponent();

        // Set tile scroll limits
        scrlTile.Maximum = Math.Max(1, Textures.Tiles.Count - 1);
        scrlTile.Value = 1;
        ScrollTile = 1;
        UpdateScrollBounds();

        // SFML offscreen canvas
        Renders.WinTileRT = new RenderTexture((uint)CanvasW, (uint)CanvasH);

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
        ScrollTile = (int)scrlTile.Value;
        lblTile.Text = "Tile: " + ScrollTile;
        scrlTileX.Value = 0;
        scrlTileY.Value = 0;
        ScrollX = 0;
        ScrollY = 0;
        UpdateScrollBounds();
    }

    private void UpdateScrollBounds()
    {
        if (Textures.Tiles.Count == 0 || ScrollTile >= Textures.Tiles.Count) return;

        var tex = Textures.Tiles[ScrollTile];
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
        ScrollX = (int)scrlTileX.Value;
        ScrollY = (int)scrlTileY.Value;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Mode toggle (Attributes / Dir. Block)
    // ──────────────────────────────────────────────────────────────────────────

    private void optMode_Changed(object? sender, RoutedEventArgs e)
    {
        ModeAttributes = optAttributes.IsChecked ?? true;
        pnlAttributes.IsVisible = ModeAttributes;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Canvas click → paint attribute / dir-block
    // ──────────────────────────────────────────────────────────────────────────

    private void imgCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Textures.Tiles.Count == 0 || ScrollTile >= Textures.Tiles.Count) return;

        var pt = e.GetPosition(imgCanvas);
        var ex = (int)pt.X;
        var ey = (int)pt.Y;

        var position = new Point((ex + ScrollX * G.Grid) / G.Grid, (ey + ScrollY * G.Grid) / G.Grid);
        var tileDif = new Point(ex - ex / G.Grid * G.Grid, ey - ey / G.Grid * G.Grid);

        var tileRef = Tile.List[ScrollTile];
        if (position.X > tileRef.Data.GetUpperBound(0)) return;
        if (position.Y > tileRef.Data.GetUpperBound(1)) return;

        var isLeft = e.GetCurrentPoint(imgCanvas).Properties.IsLeftButtonPressed;

        if (ModeAttributes)
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
        Write.Tiles();
        Close();
    }

    private void butClear_Click(object? sender, RoutedEventArgs e)
    {
        if (Textures.Tiles.Count == 0 || ScrollTile >= Textures.Tiles.Count) return;
        var tileSize = Textures.Tiles[ScrollTile].ToSize();
        Tile.List[ScrollTile] = new Tile(tileSize);
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
