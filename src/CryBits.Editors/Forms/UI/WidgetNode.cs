using CommunityToolkit.Mvvm.ComponentModel;
using Myra.Graphics2D.UI;
using System.Collections.ObjectModel;

namespace CryBits.Editors.Forms.UI;

internal sealed partial class WidgetNode : ObservableObject
{
    [ObservableProperty] private string _header = string.Empty;

    public Widget? Widget { get; set; }
    public ObservableCollection<WidgetNode> Children { get; } = [];
    public override string ToString() => Header;
}
