using System.Drawing;
using CryBits.Client.Framework.Interfacily.Components;

namespace CryBits.Client.Framework.Interfacily;

public static class InterfaceUtils
{
    public static Point MyMouse;

    public static bool Viewable(Component? component)
    {
        // Return true if component and all parents are visible (null is considered visible).
        if (component == null) return true;
        if (!component.Visible) return false;
        return Viewable(component.Parent);
    }

    public static bool IsAbove(Rectangle rectangle)
    {
        // Check whether the stored mouse position lies inside the rectangle.
        if (MyMouse.X >= rectangle.X && MyMouse.X <= rectangle.X + rectangle.Width)
            if (MyMouse.Y >= rectangle.Y && MyMouse.Y <= rectangle.Y + rectangle.Height)
                return true;

        // Otherwise return false.
        return false;
    }
}
