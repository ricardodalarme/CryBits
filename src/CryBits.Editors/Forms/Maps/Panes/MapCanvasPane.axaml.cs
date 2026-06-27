using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;

namespace CryBits.Editors.Maps;

internal partial class MapCanvasPane : UserControl
{
    public MapCanvasPane()
    {
        InitializeComponent();
    }

    public Image ImgMap => imgMap;
    public ZoomBorder ZoomBorder => zoomBorder;
}
