using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Buffers;
using System.Runtime.InteropServices;

namespace CryBits.Editors;

/// <summary>
/// GPU-readback helpers for the editor. Blits MonoGame <see cref="Texture2D"/>s and
/// <see cref="RenderTarget2D"/>s into Avalonia <see cref="Image"/> controls with zero heap allocations.
/// </summary>
internal unsafe static class EditorBlit
{
    /// <summary>Read a MonoGame texture (or sub-rect) into the target Avalonia Image.</summary>
    public static void Blit(this Image target, Texture2D? texture, Rectangle? sourceRect = null)
    {
        if (target == null || texture == null) return;

        var src = sourceRect ?? new Rectangle(0, 0, texture.Width, texture.Height);
        if (src.Width <= 0 || src.Height <= 0) return;

        var totalPixels = texture.Width * texture.Height;
        var pixels = ArrayPool<Color>.Shared.Rent(totalPixels);
        try
        {
            texture.GetData(pixels, 0, totalPixels);
            WritePixels(target, src, pixels.AsSpan(0, totalPixels), texture.Width);
        }
        finally
        {
            ArrayPool<Color>.Shared.Return(pixels);
        }
    }

    /// <summary>Read the first frame of a multi-frame atlas (cols × rows grid) into the target Image.</summary>
    public static void Blit(this Image target, Texture2D? texture, int cols, int rows)
    {
        if (target == null || texture == null || cols <= 0 || rows <= 0) return;

        var frameW = texture.Width / cols;
        var frameH = texture.Height / rows;
        if (frameW <= 0 || frameH <= 0) return;

        Blit(target, texture, new Rectangle(0, 0, frameW, frameH));
    }

    /// <summary>Read a MonoGame RenderTarget2D into the target Avalonia Image.</summary>
    public static void BlitRenderTarget(this Image target, RenderTarget2D? renderTarget)
    {
        if (target == null || renderTarget == null) return;
        if (renderTarget.Width <= 0 || renderTarget.Height <= 0) return;

        var totalPixels = renderTarget.Width * renderTarget.Height;
        var pixels = ArrayPool<Color>.Shared.Rent(totalPixels);
        try
        {
            renderTarget.GetData(pixels, 0, totalPixels);
            WritePixels(target,
                new Rectangle(0, 0, renderTarget.Width, renderTarget.Height),
                pixels.AsSpan(0, totalPixels),
                renderTarget.Width);
        }
        finally
        {
            ArrayPool<Color>.Shared.Return(pixels);
        }
    }

    private static unsafe void WritePixels(Image target, Rectangle src, ReadOnlySpan<Color> pixels, int srcStride)
    {
        var bitmap = target.Source as WriteableBitmap;
        if (bitmap == null || bitmap.PixelSize.Width != src.Width || bitmap.PixelSize.Height != src.Height)
        {
            bitmap?.Dispose();
            bitmap = new WriteableBitmap(
                new PixelSize(src.Width, src.Height),
                new Vector(96, 96),
                PixelFormat.Rgba8888,
                AlphaFormat.Opaque);
            target.Source = bitmap;
        }

        using var fb = bitmap.Lock();
        const int bytesPerPixel = 4;
        var rowBytesToCopy = src.Width * bytesPerPixel;
        var byteSpan = MemoryMarshal.AsBytes(pixels);

        var destPtr = (byte*)fb.Address;
        fixed (byte* srcPtr = byteSpan)
        {
            if (src.X == 0 && src.Y == 0 && src.Width == srcStride && fb.RowBytes == rowBytesToCopy)
            {
                Buffer.MemoryCopy(srcPtr, destPtr, byteSpan.Length, byteSpan.Length);
            }
            else
            {
                for (var y = 0; y < src.Height; y++)
                {
                    var srcOffset = ((src.Y + y) * srcStride + src.X) * bytesPerPixel;
                    var destOffset = y * fb.RowBytes;
                    Buffer.MemoryCopy(srcPtr + srcOffset, destPtr + destOffset, rowBytesToCopy, rowBytesToCopy);
                }
            }
        }

        target.InvalidateVisual();
    }
}
