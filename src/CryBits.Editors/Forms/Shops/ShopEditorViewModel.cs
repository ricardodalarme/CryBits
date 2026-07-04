using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Shops;
using CryBits.Editors.Network;

namespace CryBits.Editors.Forms.Shops;

internal sealed partial class ShopEditorViewModel(Shop model, DefinitionCatalog catalog) : ObservableObject
{
    private readonly Shop _model = model;
    private readonly DefinitionCatalog _catalog = catalog;

    public Shop Model => _model;
    public event Action? RequestClose;
    public event Action? RequestRefreshList;

    public string Name
    {
        get => _model.Name;
        set { _model.Name = value; OnPropertyChanged(); }
    }

    [RelayCommand]
    private void CreateNew()
    {
        var shop = new Shop();
        _catalog.Shops.Add(shop.Id, shop);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Remove()
    {
        _catalog.Shops.Remove(_model.Id);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Save()
    {
        PackageSender.Instance!.WriteShops();
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        PackageSender.Instance!.RequestShops();
        RequestClose?.Invoke();
    }
}
