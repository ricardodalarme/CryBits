using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Shops;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Network;

namespace CryBits.Editors.Forms.Shops;

internal partial class EditorShopsWindow : Window
{
    private readonly DefinitionCatalog _catalog;
    private ShopEditorViewModel? _viewModel;

    /// <summary>Opens the Shops editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        if (DefinitionCatalog.Instance.Items.Count == 0)
        {
            MessageBox.Show("It must have at least one item registered to open the store editor.");
            return;
        }

        owner.Hide();
        var window = new EditorShopsWindow(DefinitionCatalog.Instance);
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private Shop? _selected;
    private bool _addingToSold;

    public EditorShopsWindow(DefinitionCatalog catalog)
    {
        _catalog = catalog;
        InitializeComponent();

        var items = _catalog.Items.Values.ToList();
        cmbItems.ItemsSource = items;
        cmbCurrency.ItemsSource = items;

        List_Update();
    }

    private void Groups_Visibility()
    {
        pnlContent.IsVisible = _selected != null;
        grpAddItem.IsVisible = false;
    }

    private void List_Update(Guid? keepSelectionId = null)
    {
        var filtered = _catalog.Shops.Values
            .Where(shop => shop.Name.StartsWith(txtFilter.Text ?? string.Empty))
            .ToList();

        lstShops.ItemsSource = filtered;

        if (filtered.Count > 0)
        {
            if (keepSelectionId.HasValue)
                lstShops.SelectedItem = filtered.FirstOrDefault(shop => shop.Id == keepSelectionId.Value);

            if (lstShops.SelectedItem == null)
                lstShops.SelectedIndex = 0;
        }
        else
        {
            _selected = null;
            _viewModel = null;
            DataContext = null;
            Groups_Visibility();
        }
    }

    private void RefreshSelectedDetails()
    {
        Groups_Visibility();
        if (_selected == null) return;

        _viewModel = new ShopEditorViewModel(_selected);
        DataContext = _viewModel;

        cmbCurrency.SelectedItem = _catalog.Items.Get(_selected.CurrencyId);

        RefreshShopItems();
    }

    private void RefreshShopItems()
    {
        if (_selected == null) return;

        var lastSoldIndex = lstSold.SelectedIndex;
        var lastBoughtIndex = lstBought.SelectedIndex;

        lstSold.ItemsSource = _selected.Sold.ToList();
        lstBought.ItemsSource = _selected.Bought.ToList();

        if (lstSold.ItemCount > 0)
            lstSold.SelectedIndex = Math.Clamp(lastSoldIndex, 0, lstSold.ItemCount - 1);

        if (lstBought.ItemCount > 0)
            lstBought.SelectedIndex = Math.Clamp(lastBoughtIndex, 0, lstBought.ItemCount - 1);
    }

    private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selected = lstShops.SelectedItem as Shop;
        RefreshSelectedDetails();
    }

    private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
        List_Update(_selected?.Id);
    }

    private void butNew_Click(object sender, RoutedEventArgs e)
    {
        var shop = new Shop();
        _catalog.Shops.Add(shop.Id, shop);
        List_Update(shop.Id);
        Groups_Visibility();
        if (cmbCurrency.Items.Count > 0)
            cmbCurrency.SelectedIndex = 0;
    }

    private void butRemove_Click(object sender, RoutedEventArgs e)
    {
        if (_selected == null) return;

        var removeId = _selected.Id;
        _catalog.Shops.Remove(removeId);
        _selected = null;
        _viewModel = null;
        DataContext = null;
        List_Update();
        Groups_Visibility();
    }

    private void butSave_Click(object sender, RoutedEventArgs e)
    {
        PackageSender.Instance.WriteShops();
        Close();
    }

    private void butCancel_Click(object sender, RoutedEventArgs e)
    {
        PackageSender.Instance.RequestShops();
        Close();
    }

    private void cmbCurrency_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_selected == null) return;
        if (cmbCurrency.SelectedItem is Item item)
            _selected.CurrencyId = item.Id;
    }

    private void butSold_Add_Click(object sender, RoutedEventArgs e)
    {
        OpenAddItemPanel(true);
    }

    private void butSold_Remove_Click(object sender, RoutedEventArgs e)
    {
        if (_selected == null || lstSold.SelectedIndex < 0) return;

        _selected.Sold.RemoveAt(lstSold.SelectedIndex);
        RefreshShopItems();
    }

    private void butBought_Add_Click(object sender, RoutedEventArgs e)
    {
        OpenAddItemPanel(false);
    }

    private void butBought_Remove_Click(object sender, RoutedEventArgs e)
    {
        if (_selected == null || lstBought.SelectedIndex < 0) return;

        _selected.Bought.RemoveAt(lstBought.SelectedIndex);
        RefreshShopItems();
    }

    private void OpenAddItemPanel(bool toSold)
    {
        if (_selected == null) return;

        _addingToSold = toSold;
        cmbItems.SelectedIndex = 0;
        numAmount.Value = 1;
        numPrice.Value = 0;
        grpAddItem.IsVisible = true;
    }

    private void butConfirm_Click(object sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        if (cmbItems.SelectedItem is not Item item) return;

        var amount = (short)(numAmount.Value ?? 1m);
        var price = (short)(numPrice.Value ?? 0m);
        var data = new ShopItem(item.Id, amount, price);

        if (_addingToSold)
            _selected.Sold.Add(data);
        else
            _selected.Bought.Add(data);

        RefreshShopItems();
        grpAddItem.IsVisible = false;
    }
}
