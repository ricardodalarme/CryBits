using System;
using Avalonia.Controls;
using Avalonia.Platform;
using SFML.Graphics;

namespace CryBits.Editors.AvaloniaUI;

/// <summary>
/// An Avalonia NativeControlHost that creates a native child window and
/// binds an SFML RenderWindow to it for in-editor sprite previews.
/// The native handle (and therefore the RenderWindow) is allocated the first
/// time this control is laid out / made visible.
/// </summary>
public sealed class SfmlNativeHost : NativeControlHost
{
    public RenderWindow? SfmlWindow { get; private set; }

    /// <summary>Raised on the Avalonia UI thread when the SFML RenderWindow is ready.</summary>
    public event Action<RenderWindow>? WindowCreated;

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        var handle = base.CreateNativeControlCore(parent);
        // handle.Handle is the native window handle - SFML accepts it directly
        SfmlWindow = new RenderWindow(handle.Handle);
        WindowCreated?.Invoke(SfmlWindow);
        return handle;
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        SfmlWindow?.Close();
        SfmlWindow = null;
        base.DestroyNativeControlCore(control);
    }
}
