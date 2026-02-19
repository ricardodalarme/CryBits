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
        if (!IsAbove(new Rectangle(Position, Textures.Panels[TextureNum].ToSize()))) return;

        OnMouseUp?.Invoke();
    }

    public void MouseDown(MouseButtonEventArgs e)
    {
        if (!IsAbove(new Rectangle(Position, Textures.Panels[TextureNum].ToSize()))) return;

        OnMouseDown?.Invoke(e);
    }

    public void MouseDoubleClick(MouseButtonEventArgs e)
    {
        if (!IsAbove(new Rectangle(Position, Textures.Panels[TextureNum].ToSize()))) return;

        OnMouseDoubleClick?.Invoke(e);
    }

    public override string ToString() => "[Panel] " + Name;
}
