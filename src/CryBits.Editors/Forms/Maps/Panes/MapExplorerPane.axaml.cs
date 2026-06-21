using Avalonia.Controls;

namespace CryBits.Editors.Maps;

internal partial class MapExplorerPane : UserControl
{
    public MapExplorerPane()
    {
        InitializeComponent();
    }

    public TextBox TxtFilter => txtFilter;
    public Button ButNew => butNew;
    public Button ButRemove => butRemove;
    public ListBox LstMaps => lstMaps;
}
