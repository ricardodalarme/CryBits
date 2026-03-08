using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Interfaces;
using SFML.Window;
using static CryBits.Client.Framework.Interfacily.InterfaceUtils;

namespace CryBits.Client.Framework.Interfacily.Components;

public class Panel : Component, IMouseUp, IMouseDown, IMouseDoubleClick
{
    public byte TextureNum { get; set; }

    public event Action? OnMouseUp;
    public event Action<MouseButtonEventArgs>? OnMouseDown;
    public event Action<MouseButtonEventArgs>? OnMouseDoubleClick;

    public void MouseUp()
    {
        if (TextureNum <= 0 || TextureNum > Textures.Panels.Count) return;
        var size = Textures.Panels[TextureNum].ToSize();
        if (!IsAbove(new Rectangle(Position.X, Position.Y, size.X, size.Y))) return;

        OnMouseUp?.Invoke();
    }

    public void MouseDown(MouseButtonEventArgs e)
    {
        if (TextureNum <= 0 || TextureNum > Textures.Panels.Count) return;
        var size = Textures.Panels[TextureNum].ToSize();
        if (!IsAbove(new Rectangle(Position.X, Position.Y, size.X, size.Y))) return;

        OnMouseDown?.Invoke(e);
    }

    public void MouseDoubleClick(MouseButtonEventArgs e)
    {
        if (TextureNum <= 0 || TextureNum > Textures.Panels.Count) return;
        var size = Textures.Panels[TextureNum].ToSize();
        if (!IsAbove(new Rectangle(Position.X, Position.Y, size.X, size.Y))) return;

        OnMouseDoubleClick?.Invoke(e);
    }

    public override string ToString() => "[Panel] " + Name;
}
