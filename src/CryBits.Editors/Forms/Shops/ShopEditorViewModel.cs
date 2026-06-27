using CryBits.Definitions.Shops;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryBits.Editors.Forms.Shops;

internal sealed class ShopEditorViewModel(Shop model) : INotifyPropertyChanged
{
    private readonly Shop _model = model;
    public Shop Model => _model;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Notify([CallerMemberName] string? n = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    public string Name
    {
        get => _model.Name;
        set { _model.Name = value; Notify(); }
    }
}
