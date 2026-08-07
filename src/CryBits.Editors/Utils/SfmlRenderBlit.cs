using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SFML.Graphics;
using System.Runtime.InteropServices;
using AvaloniaImage = Avalonia.Controls.Image;
using SfmlImage = SFML.Graphics.Image;

namespace CryBits.Editors.Utils;

/// <summary>
/// Utility extension methods to blit SFML textures or RenderTextures directly into an Avalonia <see cref="AvaloniaImage"/> control.
/// </summary>
internal static class SfmlRenderBlit
{
    public static void Blit(this AvaloniaImage target, RenderTexture renderTexture)
    {
        if (target == null || renderTexture == null) return;
        using var sfmlImage = renderTexture.Texture.CopyToImage();
        BlitImage(target, sfmlImage, 1, 1);
    }

    public static void Blit(this AvaloniaImage target, Texture texture, int cols = 1, int rows = 1)
    {
        if (target == null || texture == null) return;
        using var sfmlImage = texture.CopyToImage();
        BlitImage(target, sfmlImage, cols, rows);
    }

    private static void BlitImage(AvaloniaImage target, SfmlImage sfmlImage, int cols, int rows)
    {
        var fullW = (int)sfmlImage.Size.X;
        var fullH = (int)sfmlImage.Size.Y;
        if (fullW <= 0 || fullH <= 0) return;

        var frameW = fullW / cols;
        var frameH = fullH / rows;

        var bitmap = target.Source as WriteableBitmap;

        if (bitmap == null || bitmap.PixelSize.Width != frameW || bitmap.PixelSize.Height != frameH)
        {
            bitmap?.Dispose();
            bitmap = new WriteableBitmap(
                new PixelSize(frameW, frameH),
                new Vector(96, 96),
                PixelFormat.Rgba8888,
                AlphaFormat.Unpremul);
            target.Source = bitmap;
        }

        using (var fb = bitmap.Lock())
        {
            var pixels = sfmlImage.Pixels;
            for (var y = 0; y < frameH; y++)
            {
                var srcOffset = y * fullW * 4;
                var dstPtr = fb.Address + (y * fb.RowBytes);
                Marshal.Copy(pixels, srcOffset, dstPtr, frameW * 4);
            }
        }

        target.InvalidateVisual();
    }
}
