using CommunityToolkit.Mvvm.ComponentModel;
using CryBits.Definitions.Maps;

namespace CryBits.Editors.Forms.Maps.Models;

internal sealed partial class LayerVm : ObservableObject
{
    [ObservableProperty] private bool _visible = true;

    public Layer Layer { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public int Index { get; set; }
}
