using Myra.Graphics2D.UI;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal static class WidgetPropertiesFactory
{
    public static WidgetProperties Create(Widget widget)
    {
        return new WidgetProperties(widget);
    }
}
