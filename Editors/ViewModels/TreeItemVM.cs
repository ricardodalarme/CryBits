using System.Collections.ObjectModel;
using System.ComponentModel;
using CryBits.Editors.Entities;
using Component = CryBits.Client.Framework.Interfacily.Components.Component;

namespace CryBits.Editors.ViewModels;

internal sealed class TreeItemVM : INotifyPropertyChanged
{
    private string _header = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Header
    {
        get => _header;
        set
        {
            _header = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Header)));
        }
    }

    public Component? Tag { get; set; }
    public InterfaceNode? SourceNode { get; set; }
    public TreeItemVM? Parent { get; set; }
    public ObservableCollection<TreeItemVM> Children { get; } = [];
    public override string ToString() => _header;
}
