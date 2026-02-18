using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using SFML.Graphics;

namespace CryBits.Client.Framework.Interfacily;

public static class InterfaceUtils
{
    public static Point MyMouse;

    public static bool Viewable(Component? component)
    {
        // Verifica se a ferramenta está visível
        if (component == null) return true;
        if (!component.Visible) return false;
        return Viewable(component.Parent);
    }

    public static bool IsAbove(Rectangle rectangle)
    {
        // Verficia se o Window.Mouse está sobre o objeto
        if (MyMouse.X >= rectangle.X && MyMouse.X <= rectangle.X + rectangle.Width)
            if (MyMouse.Y >= rectangle.Y && MyMouse.Y <= rectangle.Y + rectangle.Height)
                return true;

        // Se não, retornar um valor nulo
        return false;
    }

    public static short MeasureString(string text)
    {
        // Dados do texto
        var tempText = new Text(Fonts.Default, text) { CharacterSize = 10 };
        return (short)tempText.GetLocalBounds().Width;
    }
}