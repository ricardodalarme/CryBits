using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.ViewModels;
using CryBits.Entities;
using CryBits.Entities.Shop;

namespace CryBits.Editors.Forms;

internal partial class EditorShopsWindow : Window
{
    /// <summary>Opens the Shops editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        if (!EditorShopsViewModel.CanOpen())
        {
            MessageBox.Show("It must have at least one item registered to open the store editor.");
            return;
        }

        owner.Hide();
        var window = new EditorShopsWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private readonly EditorShopsViewModel _vm = new();
    private bool _addingToSold;

    public EditorShopsWindow()
    {
        DataContext = _vm;
        InitializeComponent();

        var items = Item.List.Values.ToList();
        cmbItems.ItemsSource = items;
        cmbCurrency.ItemsSource = items;

        List_Update();
    }

    private void Groups_Visibility()
    {
        pnlContent.IsVisible = _vm.HasSelection;
        grpAddItem.IsVisible = false;
    }

    private void List_Update(Guid? keepSelectionId = null)
    {
        var filtered = _vm.FilteredShops.ToList();
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
            _vm.Selected = null;
            Groups_Visibility();
        }
    }

    private void RefreshSelectedDetails()
    {
        Groups_Visibility();
        if (_vm.Selected == null) return;

        txtName.Text = _vm.Selected.Name;
        cmbCurrency.SelectedItem = _vm.Selected.Currency;

        RefreshShopItems();
    }

    private void RefreshShopItems()
    {
        if (_vm.Selected == null) return;

        var lastSoldIndex = lstSold.SelectedIndex;
        var lastBoughtIndex = lstBought.SelectedIndex;

        lstSold.ItemsSource = _vm.Selected.Sold.ToList();
        lstBought.ItemsSource = _vm.Selected.Bought.ToList();

        if (lstSold.ItemCount > 0)
            lstSold.SelectedIndex = Math.Clamp(lastSoldIndex, 0, lstSold.ItemCount - 1);

        if (lstBought.ItemCount > 0)
            lstBought.SelectedIndex = Math.Clamp(lastBoughtIndex, 0, lstBought.ItemCount - 1);
    }

    private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _vm.Selected = lstShops.SelectedItem as Shop;
        RefreshSelectedDetails();
    }

    private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vm.Filter = txtFilter.Text ?? string.Empty;
        List_Update(_vm.Selected?.Id);
    }

    private void butNew_Click(object sender, RoutedEventArgs e)
    {
        var shop = _vm.Add();
        List_Update(shop.Id);
        Groups_Visibility();
    }

    private void butRemove_Click(object sender, RoutedEventArgs e)
    {
        var removeId = _vm.Selected?.Id;
        _vm.Remove();
        List_Update();
        Groups_Visibility();
    }

    private void butSave_Click(object sender, RoutedEventArgs e)
    {
        _vm.SaveAll();
        Close();
    }

    private void butCancel_Click(object sender, RoutedEventArgs e)
    {
        _vm.Cancel();
        Close();
    }

    private void txtName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_vm.Selected == null) return;

        _vm.Selected.Name = txtName.Text ?? string.Empty;
        List_Update(_vm.Selected.Id);
    }

    private void cmbCurrency_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_vm.Selected == null) return;
        if (cmbCurrency.SelectedItem is Item item)
            _vm.Selected.Currency = item;
    }

    private void butSold_Add_Click(object sender, RoutedEventArgs e)
    {
        OpenAddItemPanel(true);
    }

    private void butSold_Remove_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || lstSold.SelectedIndex < 0) return;

        _vm.Selected.Sold.RemoveAt(lstSold.SelectedIndex);
        RefreshShopItems();
    }

    private void butBought_Add_Click(object sender, RoutedEventArgs e)
    {
        OpenAddItemPanel(false);
    }

    private void butBought_Remove_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || lstBought.SelectedIndex < 0) return;

        _vm.Selected.Bought.RemoveAt(lstBought.SelectedIndex);
        RefreshShopItems();
    }

    private void OpenAddItemPanel(bool toSold)
    {
        if (_vm.Selected == null) return;

        _addingToSold = toSold;
        cmbItems.SelectedIndex = 0;
        numAmount.Value = 1;
        numPrice.Value = 0;
        grpAddItem.IsVisible = true;
    }

    private void butConfirm_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null) return;
        if (cmbItems.SelectedItem is not Item item) return;

        var amount = (short)(numAmount.Value ?? 1m);
        var price = (short)(numPrice.Value ?? 0m);
        var data = new ShopItem(item, amount, price);

        if (_addingToSold)
            _vm.Selected.Sold.Add(data);
        else
            _vm.Selected.Bought.Add(data);

        RefreshShopItems();
        grpAddItem.IsVisible = false;
    }
}
