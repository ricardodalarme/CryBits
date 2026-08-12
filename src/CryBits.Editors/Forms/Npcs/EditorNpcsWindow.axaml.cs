using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Editors.Network;
using CryBits.Editors.Utils;

namespace CryBits.Editors.Forms.Npcs;

internal partial class EditorNpcsWindow : Window
{
    private readonly DefinitionCatalog _catalog;
    private readonly PackageSender _sender;
    private NpcEditorViewModel? _viewModel;

    public static void Open(Window owner, DefinitionCatalog catalog, PackageSender sender)
    {
        owner.Hide();
        var window = new EditorNpcsWindow(catalog, sender);
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private static short CurrentTextureIndex { get; set; }

    private Npc? _selected;

    public EditorNpcsWindow(DefinitionCatalog catalog, PackageSender sender)
    {
        _catalog = catalog;
        _sender = sender;
        InitializeComponent();

        foreach (var ms in Enum.GetValues<MovementStyle>())
            cmbMovement.Items.Add(ms.ToString());

        cmbDrop_Item.ItemsSource = _catalog.Items.Values.ToList();
        cmbShop.ItemsSource = _catalog.Shops.Values.ToList();

        RefreshNpcList();
    }

    private void RefreshNpcList()
    {
        var filter = txtFilter.Text ?? string.Empty;
        lstNpcs.ItemsSource = _catalog.Npcs.Values
            .Where(n => n.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (lstNpcs.SelectedItem == null && lstNpcs.ItemCount > 0)
            lstNpcs.SelectedIndex = 0;

        pnlContent.IsVisible = lstNpcs.SelectedItem != null;
    }

    private void txtFilter_TextChanged(object? sender, TextChangedEventArgs e) => RefreshNpcList();

    private void lstNpcs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (lstNpcs.SelectedItem is not Npc npc) return;
        _selected = npc;
        _viewModel = new NpcEditorViewModel(npc, _catalog, _sender);
        DataContext = _viewModel;
        _viewModel.RequestClose += Close;
        _viewModel.RequestRefreshList += RefreshNpcList;

        numTexture.Maximum = Math.Max(0, Textures.Characters.Count - 1);
        CurrentTextureIndex = npc.Texture;
        UpdateTexturePreview(CurrentTextureIndex);

        cmbBehavior.SelectedIndex = (int)npc.Behaviour;
        cmbMovement.SelectedIndex = (int)npc.Movement;
        chkAttackNpc.IsChecked = npc.AttackNpc;
        lstAllies.IsEnabled = npc.AttackNpc;

        pnlShop.IsVisible = npc.Behaviour == Behaviour.ShopKeeper;
        cmbShop.SelectedItem = _catalog.Shops.Get(npc.ShopId);
        if (npc.Behaviour == Behaviour.ShopKeeper && cmbShop.SelectedItem == null && cmbShop.Items.Count > 0)
            cmbShop.SelectedIndex = 0;

        RefreshDropList();
        RefreshAlliesList();

        pnlDrop_Add.IsVisible = false;
        pnlAllie_Add.IsVisible = false;

        pnlContent.IsVisible = true;
    }

    private void numTexture_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        CurrentTextureIndex = (short)(e.NewValue ?? 0);
        UpdateTexturePreview(CurrentTextureIndex);
    }

    private void UpdateTexturePreview(short textureIndex)
    {
        if (textureIndex <= 0 || textureIndex >= Textures.Characters.Count)
        {
            imgTexture.Source = null;
            return;
        }

        imgTexture.Blit(Textures.Characters[textureIndex], cols: 4, rows: 4);
    }

    private void cmbBehavior_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_selected == null) return;
        var behaviour = (Behaviour)cmbBehavior.SelectedIndex;

        if (behaviour == Behaviour.ShopKeeper && _catalog.Shops.Count == 0)
        {
            cmbBehavior.SelectedIndex = (int)_selected.Behaviour;
            return;
        }

        _selected.Behaviour = behaviour;
        pnlShop.IsVisible = behaviour == Behaviour.ShopKeeper;

        if (behaviour != Behaviour.ShopKeeper)
            cmbShop.SelectedIndex = -1;
        else if (_selected.ShopId == Guid.Empty && cmbShop.Items.Count > 0)
            cmbShop.SelectedIndex = 0;
    }

    private void cmbMovement_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _selected?.Movement = (MovementStyle)cmbMovement.SelectedIndex;
    }

    private void cmbShop_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_selected == null) return;
        if (cmbShop.SelectedItem is Shop shop) _selected.ShopId = shop.Id;
    }

    private void RefreshDropList()
    {
        lstDrop.ItemsSource = null;
        lstDrop.ItemsSource = _selected?.Drop;
    }

    private void butDrop_Add_Click(object? sender, RoutedEventArgs e)
    {
        if (_catalog.Items.Count == 0) return;
        numDrop_Amount.Value = 1;
        numDrop_Chance.Value = 100;
        if (cmbDrop_Item.Items.Count > 0) cmbDrop_Item.SelectedIndex = 0;
        pnlDrop_Add.IsVisible = true;
    }

    private void butDrop_Cancel_Click(object? sender, RoutedEventArgs e) => pnlDrop_Add.IsVisible = false;

    private void butDrop_Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || cmbDrop_Item.SelectedItem is not Item item) return;
        _selected.Drop.Add(
            new NpcDrop(item.Id, (short)(numDrop_Amount.Value ?? 1), (byte)(numDrop_Chance.Value ?? 100)));
        pnlDrop_Add.IsVisible = false;
        RefreshDropList();
    }

    private void butDrop_Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || lstDrop.SelectedItem is not NpcDrop drop) return;
        _selected.Drop.Remove(drop);
        RefreshDropList();
    }

    private void RefreshAlliesList()
    {
        lstAllies.ItemsSource = null;
        lstAllies.ItemsSource = _selected?.AllyIds.Select(id => _catalog.Npcs.Get(id)).Where(n => n != null).ToList();
    }

    private void chkAttackNpc_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        _selected.AttackNpc = chkAttackNpc.IsChecked ?? false;
        lstAllies.IsEnabled = _selected.AttackNpc;
        if (!_selected.AttackNpc)
        {
            _selected.AllyIds.Clear();
            RefreshAlliesList();
        }
    }

    private void butAllie_Add_Click(object? sender, RoutedEventArgs e)
    {
        if (!(_selected?.AttackNpc ?? false)) return;
        cmbAllie_Npc.ItemsSource = _catalog.Npcs.Values.ToList();
        if (cmbAllie_Npc.Items.Count > 0) cmbAllie_Npc.SelectedIndex = 0;
        pnlAllie_Add.IsVisible = true;
    }

    private void butAllie_Cancel_Click(object? sender, RoutedEventArgs e) => pnlAllie_Add.IsVisible = false;

    private void butAllie_Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || cmbAllie_Npc.SelectedItem is not Npc allie) return;
        if (!_selected.AllyIds.Contains(allie.Id))
            _selected.AllyIds.Add(allie.Id);
        pnlAllie_Add.IsVisible = false;
        RefreshAlliesList();
    }

    private void butAllie_Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || lstAllies.SelectedItem is not Npc allie) return;
        _selected.AllyIds.Remove(allie.Id);
        RefreshAlliesList();
    }
}
