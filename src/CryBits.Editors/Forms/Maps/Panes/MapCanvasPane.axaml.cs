using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CryBits.Editors.Maps;

internal partial class MapCanvasPane : UserControl
{
    public MapCanvasPane()
    {
        InitializeComponent();
    }

    public Image ImgMap => imgMap;
    public ScrollBar ScrlMapX => scrlMapX;
    public ScrollBar ScrlMapY => scrlMapY;
}
