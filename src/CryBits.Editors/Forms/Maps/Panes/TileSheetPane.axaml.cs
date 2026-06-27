using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CryBits.Editors.Maps;

internal partial class TileSheetPane : UserControl
{
    public TileSheetPane()
    {
        InitializeComponent();
    }

    public ComboBox CmbTiles => cmbTiles;
    public CheckBox ChkAuto => chkAuto;
    public Image ImgTile => imgTile;
    public ScrollBar ScrlTileX => scrlTileX;
    public ScrollBar ScrlTileY => scrlTileY;
}
