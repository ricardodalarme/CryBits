using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Graphics;
using CryBits.Editors.ViewModels;
using CryBits.Entities;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Editors.Forms;

internal partial class EditorNpcsWindow : Window
{
    /// <summary>Opens the NPCs editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorNpcsWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private readonly EditorNpcsViewModel _vm = new();

    private WriteableBitmap? _previewBitmap;
    private DispatcherTimer? _timer;

    public EditorNpcsWindow()
    {
        DataContext = _vm;
        InitializeComponent();

        // Populate behaviour/movement combos
        foreach (var ms in Enum.GetValues<MovementStyle>())
            cmbMovement.Items.Add(ms.ToString());

        // Populate drop-item and shop combos from live data
        cmbDrop_Item.ItemsSource = Item.List.Values.ToList();
        cmbShop.ItemsSource = Shop.List.Values.ToList();

        // SFML offscreen render for the texture preview
        Renders.WinNpcRT = new RenderTexture(new Vector2u(80, 80));

        // Timer: ~30 fps preview
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();

        RefreshNpcList();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        Renders.WinNpcRT = null;
        base.OnClosed(e);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // SFML render tick → WriteableBitmap texture preview
    // ──────────────────────────────────────────────────────────────────────────

    private void OnRenderTick(object? sender, EventArgs e)
    {
        if (Renders.WinNpcRT == null || EditorNpcsViewModel.CurrentTextureIndex <= 0) return;

        Renders.EditorNpcRT();
        SfmlRenderBlit.Blit(Renders.WinNpcRT, ref _previewBitmap, imgTexture);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // NPC list
    // ──────────────────────────────────────────────────────────────────────────

    private void RefreshNpcList()
    {
        lstNpcs.ItemsSource = _vm.FilteredNpcs.ToList();

        if (lstNpcs.SelectedItem == null && lstNpcs.ItemCount > 0)
            lstNpcs.SelectedIndex = 0;

        pnlContent.IsVisible = lstNpcs.SelectedItem != null;
    }

    private void RefreshNpcListKeepSelection()
    {
        var saved = _vm.Selected;
        _vm.BeginLoad();
        lstNpcs.ItemsSource = _vm.FilteredNpcs.ToList();
        lstNpcs.SelectedItem = saved;
        _vm.EndLoad();
    }

    private void txtFilter_TextChanged(object? sender, TextChangedEventArgs e)
    {
        _vm.Filter = txtFilter.Text ?? string.Empty;
        RefreshNpcList();
    }

    private void lstNpcs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_vm.IsLoading) return;
        if (lstNpcs.SelectedItem is not Npc npc) return;
        LoadNpc(npc);
        pnlContent.IsVisible = true;
    }

    private void LoadNpc(Npc npc)
    {
        _vm.BeginLoad();
        _vm.Selected = npc;

        txtName.Text = npc.Name;
        txtSayMsg.Text = npc.SayMsg;

        numTexture.Maximum = Math.Max(0, CryBits.Client.Framework.Graphics.Textures.Characters.Count - 1);
        numTexture.Value = npc.Texture;
        EditorNpcsViewModel.CurrentTextureIndex = npc.Texture;

        numRange.Value = npc.Sight;
        numSpawn.Value = npc.SpawnTime;
        numExperience.Value = npc.Experience;

        numHP.Value = npc.Vital[(byte)Vital.Hp];
        numMP.Value = npc.Vital[(byte)Vital.Mp];
        numStrength.Value = npc.Attribute[(byte)Attribute.Strength];
        numResistance.Value = npc.Attribute[(byte)Attribute.Resistance];
        numIntelligence.Value = npc.Attribute[(byte)Attribute.Intelligence];
        numAgility.Value = npc.Attribute[(byte)Attribute.Agility];
        numVitality.Value = npc.Attribute[(byte)Attribute.Vitality];

        cmbBehavior.SelectedIndex = (int)npc.Behaviour;
        cmbMovement.SelectedIndex = (int)npc.Movement;
        numFlee_Health.Value = npc.FleeHealth;
        chkAttackNpc.IsChecked = npc.AttackNpc;
        lstAllies.IsEnabled = npc.AttackNpc;

        // Shop visibility
        pnlShop.IsVisible = npc.Behaviour == Behaviour.ShopKeeper;
        cmbShop.SelectedItem = npc.Shop;

        // Drop / Allies lists
        RefreshDropList();
        RefreshAlliesList();

        pnlDrop_Add.IsVisible = false;
        pnlAllie_Add.IsVisible = false;

        _vm.EndLoad();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // New / Remove
    // ──────────────────────────────────────────────────────────────────────────

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        var npc = _vm.Add();
        RefreshNpcList();
        lstNpcs.SelectedItem = npc;
        pnlContent.IsVisible = true;
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        _vm.Remove();
        RefreshNpcList();
        pnlContent.IsVisible = lstNpcs.SelectedItem != null;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Property write-backs
    // ──────────────────────────────────────────────────────────────────────────

    private void txtName_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Name = txtName.Text ?? string.Empty;
        RefreshNpcListKeepSelection();
    }

    private void txtSayMsg_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.SayMsg = txtSayMsg.Text ?? string.Empty;
    }

    private void numTexture_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Texture = (short)(e.NewValue ?? 0);
        EditorNpcsViewModel.CurrentTextureIndex = _vm.Selected.Texture;
    }

    private void numRange_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Sight = (byte)(e.NewValue ?? 0);
    }

    private void numSpawn_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.SpawnTime = (byte)(e.NewValue ?? 0);
    }

    private void numExperience_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Experience = (int)(e.NewValue ?? 0);
    }

    private void numHP_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Vital[(byte)Vital.Hp] = (short)(e.NewValue ?? 0);
    }

    private void numMP_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Vital[(byte)Vital.Mp] = (short)(e.NewValue ?? 0);
    }

    private void numStrength_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Attribute[(byte)Attribute.Strength] = (short)(e.NewValue ?? 0);
    }

    private void numResistance_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Attribute[(byte)Attribute.Resistance] = (short)(e.NewValue ?? 0);
    }

    private void numIntelligence_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Attribute[(byte)Attribute.Intelligence] = (short)(e.NewValue ?? 0);
    }

    private void numAgility_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Attribute[(byte)Attribute.Agility] = (short)(e.NewValue ?? 0);
    }

    private void numVitality_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Attribute[(byte)Attribute.Vitality] = (short)(e.NewValue ?? 0);
    }

    private void cmbBehavior_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        var behaviour = (Behaviour)cmbBehavior.SelectedIndex;

        // Validate: ShopKeeper needs at least one shop
        if (behaviour == Behaviour.ShopKeeper && !EditorNpcsViewModel.CanAssignShopKeeper())
        {
            cmbBehavior.SelectedIndex = (int)_vm.Selected!.Behaviour;
            return;
        }

        _vm.Selected!.Behaviour = behaviour;
        pnlShop.IsVisible = behaviour == Behaviour.ShopKeeper;

        if (behaviour != Behaviour.ShopKeeper)
        {
            cmbShop.SelectedIndex = -1;
        }
        else if (_vm.Selected.Shop == null && cmbShop.Items.Count > 0)
        {
            cmbShop.SelectedIndex = 0;
        }
    }

    private void cmbMovement_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Movement = (MovementStyle)cmbMovement.SelectedIndex;
    }

    private void numFlee_Health_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.FleeHealth = (byte)(e.NewValue ?? 0);
    }

    private void cmbShop_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        if (cmbShop.SelectedItem is Shop shop)
            _vm.Selected!.Shop = shop;
    }

    private void RefreshDropList()
    {
        lstDrop.ItemsSource = null;
        lstDrop.ItemsSource = _vm.Selected?.Drop;
    }

    private void butDrop_Add_Click(object? sender, RoutedEventArgs e)
    {
        if (Item.List.Count == 0) return;
        numDrop_Amount.Value = 1;
        numDrop_Chance.Value = 100;
        if (cmbDrop_Item.Items.Count > 0) cmbDrop_Item.SelectedIndex = 0;
        pnlDrop_Add.IsVisible = true;
    }

    private void butDrop_Cancel_Click(object? sender, RoutedEventArgs e)
    {
        pnlDrop_Add.IsVisible = false;
    }

    private void butDrop_Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || cmbDrop_Item.SelectedItem is not Item item) return;
        _vm.Selected.Drop.Add(new NpcDrop(item, (short)(numDrop_Amount.Value ?? 1), (byte)(numDrop_Chance.Value ?? 100)));
        pnlDrop_Add.IsVisible = false;
        RefreshDropList();
    }

    private void butDrop_Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || lstDrop.SelectedItem is not NpcDrop drop) return;
        _vm.Selected.Drop.Remove(drop);
        RefreshDropList();
    }

    // ──────────────────────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────────────────────

    private void RefreshAlliesList()
    {
        lstAllies.ItemsSource = null;
        lstAllies.ItemsSource = _vm.Selected?.Allie;
    }

    private void chkAttackNpc_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.AttackNpc = chkAttackNpc.IsChecked ?? false;
        lstAllies.IsEnabled = _vm.Selected.AttackNpc;
        if (!_vm.Selected.AttackNpc)
        {
            _vm.Selected.Allie.Clear();
            RefreshAlliesList();
        }
    }

    private void butAllie_Add_Click(object? sender, RoutedEventArgs e)
    {
        if (!(_vm.Selected?.AttackNpc ?? false)) return;
        cmbAllie_Npc.ItemsSource = Npc.List.Values.ToList();
        if (cmbAllie_Npc.Items.Count > 0) cmbAllie_Npc.SelectedIndex = 0;
        pnlAllie_Add.IsVisible = true;
    }

    private void butAllie_Cancel_Click(object? sender, RoutedEventArgs e)
    {
        pnlAllie_Add.IsVisible = false;
    }

    private void butAllie_Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || cmbAllie_Npc.SelectedItem is not Npc allie) return;
        if (!_vm.Selected.Allie.Contains(allie))
            _vm.Selected.Allie.Add(allie);
        pnlAllie_Add.IsVisible = false;
        RefreshAlliesList();
    }

    private void butAllie_Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || lstAllies.SelectedItem is not Npc allie) return;
        _vm.Selected.Allie.Remove(allie);
        RefreshAlliesList();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Save / Cancel
    // ──────────────────────────────────────────────────────────────────────────

    private void butSave_Click(object? sender, RoutedEventArgs e)
    {
        _vm.SaveAll();
        Close();
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        _vm.Cancel();
        Close();
    }
}
