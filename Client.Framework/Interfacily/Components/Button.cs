using System.Drawing;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Client.Framework.Interfacily.Interfaces;
using SFML.Window;
using static CryBits.Client.Framework.Interfacily.InterfaceUtils;

namespace CryBits.Client.Framework.Interfacily.Components;

public class Button : Component, IMouseMoved, IMouseUp, IMouseDown
{
    public byte TextureNum { get; set; }
    public ButtonState ButtonState { get; set; }

    public event Action? OnMouseUp;

    public void MouseUp()
    {
        var size = Textures.Buttons[TextureNum].ToSize();
        if (!IsAbove(new Rectangle(Position.X, Position.Y, size.X, size.Y))) return;

        AudioManager.Instance.PlaySound(Sounds.Click);
        ButtonState = ButtonState.Above;

        OnMouseUp?.Invoke();
    }

    public void MouseDown(MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Right) return;
        var size = Textures.Buttons[TextureNum].ToSize();
        if (!IsAbove(new Rectangle(Position.X, Position.Y, size.X, size.Y))) return;

        ButtonState = ButtonState.Click;
    }

    public void MouseMoved()
    {
        var size = Textures.Buttons[TextureNum].ToSize();
        if (!IsAbove(new Rectangle(Position.X, Position.Y, size.X, size.Y)))
        {
            ButtonState = ButtonState.Normal;
            return;
        }

        if (ButtonState != ButtonState.Normal) return;

        ButtonState = ButtonState.Above;
        AudioManager.Instance.PlaySound(Sounds.Above);
    }

    public override string ToString() => "[Button] " + Name;
}
