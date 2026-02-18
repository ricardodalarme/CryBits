using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SFML.Graphics;
using AvaloniaImage = Avalonia.Controls.Image;

namespace CryBits.Editors.AvaloniaUI;

/// <summary>
/// Shared helper: copies the pixel data from an SFML <see cref="RenderTexture"/>
/// into an Avalonia <see cref="AvaloniaImage"/> control via a <see cref="WriteableBitmap"/>.
/// Call this once per render-tick from any window that hosts an SFML preview.
/// </summary>
internal static class SfmlRenderBlit
{
    /// <summary>
    /// Blits <paramref name="rt"/> into <paramref name="target"/>.
    /// The <paramref name="bitmap"/> ref is reused if its size has not changed.
    /// </summary>
    public static void Blit(RenderTexture rt, ref WriteableBitmap? bitmap, AvaloniaImage target)
    {
        var sfmlImage = rt.Texture.CopyToImage();
        var w = (int)sfmlImage.Size.X;
        var h = (int)sfmlImage.Size.Y;
        var pixels = sfmlImage.Pixels;

        if (bitmap == null || bitmap.PixelSize.Width != w || bitmap.PixelSize.Height != h)
            bitmap = new WriteableBitmap(new PixelSize(w, h), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Unpremul);

        using var fb = bitmap.Lock();
        for (var y = 0; y < h; y++)
            Marshal.Copy(pixels, y * w * 4, fb.Address + y * fb.RowBytes, w * 4);

        target.Source = bitmap;
        target.InvalidateVisual();
    }

    /// <summary>
    /// Converts a full SFML <see cref="SFML.Graphics.Texture"/> to a fresh
    /// <see cref="WriteableBitmap"/> and assigns it to <paramref name="target"/>.
    /// Use this for textures that are displayed whole (e.g. item icons).
    /// </summary>
    public static void BlitTexture(Texture sfmlTexture, AvaloniaImage target)
    {
        var img = sfmlTexture.CopyToImage();
        var w = (int)img.Size.X;
        var h = (int)img.Size.Y;
        BlitRegion(img.Pixels, w, h, w, h, target);
    }

    /// <summary>
    /// Crops the first cell from a uniform sprite-sheet and assigns it to
    /// <paramref name="target"/>. Use this for character sprite-sheets where
    /// the sheet is divided into <paramref name="cols"/> columns and
    /// <paramref name="rows"/> rows and only the top-left frame is needed.
    /// </summary>
    public static void BlitTexture(Texture sfmlTexture, AvaloniaImage target, int cols, int rows)
    {
        var img = sfmlTexture.CopyToImage();
        var fullW = (int)img.Size.X;
        var fullH = (int)img.Size.Y;
        BlitRegion(img.Pixels, fullW, fullH, fullW / cols, fullH / rows, target);
    }

    // Shared inner blit: copies frameW×frameH pixels from the top-left of a
    // fullW-stride RGBA buffer into a new WriteableBitmap on target.
    private static void BlitRegion(byte[] pixels, int fullW, int fullH, int frameW, int frameH, AvaloniaImage target)
    {
        var bitmap = new WriteableBitmap(
            new PixelSize(frameW, frameH),
            new Vector(96, 96),
            PixelFormat.Rgba8888,
            AlphaFormat.Unpremul);

        using var fb = bitmap.Lock();
        for (var y = 0; y < frameH; y++)
        {
            var srcOffset = y * fullW * 4;
            var dstPtr = fb.Address + y * fb.RowBytes;
            Marshal.Copy(pixels, srcOffset, dstPtr, frameW * 4);
        }

        target.Source = bitmap;
    }
}
