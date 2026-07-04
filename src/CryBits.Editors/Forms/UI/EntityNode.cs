using CommunityToolkit.Mvvm.ComponentModel;
using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using System.Collections.ObjectModel;

namespace CryBits.Editors.Forms.UI;

internal sealed partial class EntityNode : ObservableObject
{
    [ObservableProperty]
    private string _header = string.Empty;

    public Entity? Entity { get; set; }
    public Element? ConfigElement { get; set; }
    public ObservableCollection<EntityNode> Children { get; } = [];
    public override string ToString() => Header;
}
