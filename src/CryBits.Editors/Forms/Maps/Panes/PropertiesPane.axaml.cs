using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;

namespace CryBits.Editors.Forms.Maps.Panes;

internal partial class PropertiesPane : UserControl
{
    public PropertiesPane()
    {
        InitializeComponent();
    }

    public PropertyGrid PrgMapProperties => prgMapProperties;
}
