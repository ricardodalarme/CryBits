using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SFML.Graphics;
using System.Runtime.InteropServices;
using AvaloniaImage = Avalonia.Controls.Image;

namespace CryBits.Editors.AvaloniaUI;

/// <summary>
/// Copies the pixel data from an SFML <see cref="RenderTexture"/> into an
/// Avalonia <see cref="AvaloniaImage"/> control via a <see cref="WriteableBitmap"/>.
/// </summary>
internal static class SfmlRenderBlit
{
    public static void Blit(RenderTexture rt, ref WriteableBitmap? bitmap, AvaloniaImage target)
    {
        using var sfmlImage = rt.Texture.CopyToImage();
        var w = (int)sfmlImage.Size.X;
        var h = (int)sfmlImage.Size.Y;
        var pixels = sfmlImage.Pixels;

        if (bitmap?.PixelSize.Width != w || bitmap?.PixelSize.Height != h)
        {
            bitmap?.Dispose();
            bitmap = new WriteableBitmap(new PixelSize(w, h), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Unpremul);
        }

        using var fb = bitmap.Lock();
        for (var y = 0; y < h; y++)
            Marshal.Copy(pixels, y * w * 4, fb.Address + y * fb.RowBytes, w * 4);

        target.Source = bitmap;
        target.InvalidateVisual();
    }

    public static void BlitTexture(Texture sfmlTexture, ref WriteableBitmap? bitmap, AvaloniaImage target)
    {
        using var img = sfmlTexture.CopyToImage();
        var w = (int)img.Size.X;
        var h = (int)img.Size.Y;
        BlitRegion(img.Pixels, w, h, w, h, ref bitmap, target);
    }

    public static void BlitTexture(Texture sfmlTexture, AvaloniaImage target, int cols, int rows)
    {
        using var img = sfmlTexture.CopyToImage();
        var fullW = (int)img.Size.X;
        var fullH = (int)img.Size.Y;
        var frameW = fullW / cols;
        var frameH = fullH / rows;
        using var bitmap = new WriteableBitmap(
            new PixelSize(frameW, frameH),
            new Vector(96, 96),
            PixelFormat.Rgba8888,
            AlphaFormat.Unpremul);

        using var fb = bitmap.Lock();
        for (var y = 0; y < frameH; y++)
        {
            var srcOffset = y * fullW * 4;
            var dstPtr = fb.Address + y * fb.RowBytes;
            Marshal.Copy(img.Pixels, srcOffset, dstPtr, frameW * 4);
        }

        target.Source = bitmap;
    }

    private static void BlitRegion(byte[] pixels, int fullW, int fullH, int frameW, int frameH,
        ref WriteableBitmap? bitmap, AvaloniaImage target)
    {
        if (bitmap?.PixelSize.Width != frameW || bitmap?.PixelSize.Height != frameH)
        {
            bitmap?.Dispose();
            bitmap = new WriteableBitmap(
                new PixelSize(frameW, frameH),
                new Vector(96, 96),
                PixelFormat.Rgba8888,
                AlphaFormat.Unpremul);
        }

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
