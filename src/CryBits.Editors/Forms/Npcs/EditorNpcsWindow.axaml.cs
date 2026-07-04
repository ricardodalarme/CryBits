using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Graphics.Renderers;
using SFML.Graphics;
using SFML.System;

namespace CryBits.Editors.Forms.Npcs;

internal partial class EditorNpcsWindow : Window
{
    private readonly DefinitionCatalog _catalog;
    private NpcEditorViewModel? _viewModel;

    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorNpcsWindow(Program.Catalog);
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    public static short CurrentTextureIndex { get; private set; }

    private Npc? _selected;

    private WriteableBitmap? _previewBitmap;
    private readonly DispatcherTimer? _timer;

    public EditorNpcsWindow(DefinitionCatalog catalog)
    {
        _catalog = catalog;
        InitializeComponent();

        foreach (var ms in Enum.GetValues<MovementStyle>())
            cmbMovement.Items.Add(ms.ToString());

        cmbDrop_Item.ItemsSource = _catalog.Items.Values.ToList();
        cmbShop.ItemsSource = _catalog.Shops.Values.ToList();

        PortraitRenderer.Instance.WinCharacter = new RenderTexture(new Vector2u(80, 80));

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();

        RefreshNpcList();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        PortraitRenderer.Instance.WinCharacter?.Dispose();
        PortraitRenderer.Instance.WinCharacter = null;
        _previewBitmap?.Dispose();
        base.OnClosed(e);
    }

    private void OnRenderTick(object? sender, EventArgs e)
    {
        if (PortraitRenderer.Instance.WinCharacter == null || CurrentTextureIndex <= 0) return;

        PortraitRenderer.Instance.Character();
        SfmlRenderBlit.Blit(PortraitRenderer.Instance.WinCharacter, ref _previewBitmap, imgTexture);
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
        _viewModel = new NpcEditorViewModel(npc, _catalog);
        DataContext = _viewModel;
        _viewModel.RequestClose += () => Close();
        _viewModel.RequestRefreshList += RefreshNpcList;

        numTexture.Maximum = Math.Max(0, Textures.Characters.Count - 1);
        CurrentTextureIndex = npc.Texture;

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
        if (_selected == null) return;
        _selected.Movement = (MovementStyle)cmbMovement.SelectedIndex;
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
        _selected.Drop.Add(new NpcDrop(item.Id, (short)(numDrop_Amount.Value ?? 1), (byte)(numDrop_Chance.Value ?? 100)));
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
