using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Shops;
using CryBits.Editors.Network;

namespace CryBits.Editors.Forms.Shops;

internal sealed partial class ShopEditorViewModel(Shop model, DefinitionCatalog catalog) : ObservableObject
{
    public Shop Model { get; } = model;

    public event Action? RequestClose;
    public event Action? RequestRefreshList;

    public string Name
    {
        get => Model.Name;
        set
        {
            Model.Name = value;
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    private void CreateNew()
    {
        var shop = new Shop();
        catalog.Shops.Add(shop.Id, shop);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Remove()
    {
        catalog.Shops.Remove(Model.Id);
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
