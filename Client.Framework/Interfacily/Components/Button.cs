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
        // Somente se necessário
        if (!IsAbove(new Rectangle(Position, Textures.Buttons[TextureNum].ToSize()))) return;

        // Altera o estado do botão
        Sound.Play(Sounds.Click);
        ButtonState = ButtonState.Above;

        // Executa o evento
        OnMouseUp?.Invoke();
    }

    public void MouseDown(MouseButtonEventArgs e)
    {
        // Somente se necessário
        if (e.Button == Mouse.Button.Right) return;
        if (!IsAbove(new Rectangle(Position, Textures.Buttons[TextureNum].ToSize()))) return;

        // Altera o estado do botão
        ButtonState = ButtonState.Click;
    }

    public void MouseMoved()
    {
        // Se o mouse não estiver sobre a ferramenta, então não executar o evento
        if (!IsAbove(new Rectangle(Position, Textures.Buttons[TextureNum].ToSize())))
        {
            ButtonState = ButtonState.Normal;
            return;
        }

        // Se o botão já estiver no estado normal, isso não é necessário
        if (ButtonState != ButtonState.Normal) return;

        // Altera o estado do botão
        ButtonState = ButtonState.Above;
        Sound.Play(Sounds.Above);
    }

    public override string ToString() => "[Button] " + Name;
}