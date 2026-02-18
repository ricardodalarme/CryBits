using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Entities.Shop;

namespace CryBits.Editors.AvaloniaUI.Forms;

internal partial class EditorShopsWindow : Window
{
    private Shop _selected;
    private bool _addingToSold;

    public EditorShopsWindow()
    {
        InitializeComponent();

        var items = Item.List.Values.ToList();
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
        var filtered = Shop.List.Values
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
            Groups_Visibility();
        }
    }

    private void RefreshSelectedDetails()
    {
        Groups_Visibility();
        if (_selected == null) return;

        txtName.Text = _selected.Name;
        cmbCurrency.SelectedItem = _selected.Currency;

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

    private void txtFilter_TextChanged(object sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        List_Update(_selected?.Id);
    }

    private void butNew_Click(object sender, RoutedEventArgs e)
    {
        var shop = new Shop();
        Shop.List.Add(shop.Id, shop);
        List_Update(shop.Id);
        Groups_Visibility();
    }

    private void butRemove_Click(object sender, RoutedEventArgs e)
    {
        if (_selected == null) return;

        var removeId = _selected.Id;
        Shop.List.Remove(removeId);
        _selected = null;
        List_Update();
        Groups_Visibility();
    }

    private void butSave_Click(object sender, RoutedEventArgs e)
    {
        Send.WriteShops();
        Close();
    }

    private void butCancel_Click(object sender, RoutedEventArgs e)
    {
        Send.RequestShops();
        Close();
    }

    private void txtName_TextChanged(object sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        if (_selected == null) return;

        _selected.Name = txtName.Text ?? string.Empty;
        List_Update(_selected.Id);
    }

    private void cmbCurrency_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_selected == null) return;
        if (cmbCurrency.SelectedItem is Item item)
            _selected.Currency = item;
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
        var data = new ShopItem(item, amount, price);

        if (_addingToSold)
            _selected.Sold.Add(data);
        else
            _selected.Bought.Add(data);

        RefreshShopItems();
        grpAddItem.IsVisible = false;
    }
}
