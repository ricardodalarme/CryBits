using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Client.Framework.Graphics;
using CryBits.Editors.Graphics;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Editors.AvaloniaUI.Forms;

internal partial class EditorNpcsWindow : Window
{
    // Consumed by Renders.EditorNpc() instead of EditorNpcs.Form.numTexture.Value
    public static short CurrentTextureIndex { get; private set; }

    private Npc? _selected;
    private bool _loading;

    private WriteableBitmap? _previewBitmap;
    private DispatcherTimer? _timer;

    public EditorNpcsWindow()
    {
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
        if (Renders.WinNpcRT == null || CurrentTextureIndex <= 0) return;

        Renders.EditorNpcRT();
        SfmlRenderBlit.Blit(Renders.WinNpcRT, ref _previewBitmap, imgTexture);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // NPC list
    // ──────────────────────────────────────────────────────────────────────────

    private void RefreshNpcList()
    {
        var filter = txtFilter.Text ?? string.Empty;
        lstNpcs.ItemsSource = Npc.List.Values
            .Where(n => n.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (lstNpcs.SelectedItem == null && lstNpcs.ItemCount > 0)
            lstNpcs.SelectedIndex = 0;

        pnlContent.IsVisible = lstNpcs.SelectedItem != null;
    }

    private void RefreshNpcListKeepSelection()
    {
        var saved = _selected;
        _loading = true;
        var filter = txtFilter.Text ?? string.Empty;
        lstNpcs.ItemsSource = Npc.List.Values
            .Where(n => n.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();
        lstNpcs.SelectedItem = saved;
        _loading = false;
    }

    private void txtFilter_TextChanged(object? sender, TextChangedEventArgs e)
    {
        RefreshNpcList();
    }

    private void lstNpcs_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading) return;
        if (lstNpcs.SelectedItem is not Npc npc) return;
        LoadNpc(npc);
        pnlContent.IsVisible = true;
    }

    private void LoadNpc(Npc npc)
    {
        _loading = true;
        _selected = npc;

        txtName.Text = npc.Name;
        txtSayMsg.Text = npc.SayMsg;

        numTexture.Maximum = Math.Max(0, Textures.Characters.Count - 1);
        numTexture.Value = npc.Texture;
        CurrentTextureIndex = npc.Texture;

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

        _loading = false;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // New / Remove
    // ──────────────────────────────────────────────────────────────────────────

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        var npc = new Npc();
        Npc.List.Add(npc.Id, npc);
        RefreshNpcList();
        lstNpcs.SelectedItem = npc;
        pnlContent.IsVisible = true;
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        Npc.List.Remove(_selected.Id);
        _selected = null;
        RefreshNpcList();
        pnlContent.IsVisible = lstNpcs.SelectedItem != null;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Property write-backs
    // ──────────────────────────────────────────────────────────────────────────

    private void txtName_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Name = txtName.Text ?? string.Empty;
        RefreshNpcListKeepSelection();
    }

    private void txtSayMsg_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.SayMsg = txtSayMsg.Text ?? string.Empty;
    }

    private void numTexture_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Texture = (short)(e.NewValue ?? 0);
        CurrentTextureIndex = _selected.Texture;
    }

    private void numRange_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Sight = (byte)(e.NewValue ?? 0);
    }

    private void numSpawn_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.SpawnTime = (byte)(e.NewValue ?? 0);
    }

    private void numExperience_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Experience = (int)(e.NewValue ?? 0);
    }

    private void numHP_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Vital[(byte)Vital.Hp] = (short)(e.NewValue ?? 0);
    }

    private void numMP_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Vital[(byte)Vital.Mp] = (short)(e.NewValue ?? 0);
    }

    private void numStrength_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Attribute[(byte)Attribute.Strength] = (short)(e.NewValue ?? 0);
    }

    private void numResistance_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Attribute[(byte)Attribute.Resistance] = (short)(e.NewValue ?? 0);
    }

    private void numIntelligence_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Attribute[(byte)Attribute.Intelligence] = (short)(e.NewValue ?? 0);
    }

    private void numAgility_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Attribute[(byte)Attribute.Agility] = (short)(e.NewValue ?? 0);
    }

    private void numVitality_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Attribute[(byte)Attribute.Vitality] = (short)(e.NewValue ?? 0);
    }

    private void cmbBehavior_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        var behaviour = (Behaviour)cmbBehavior.SelectedIndex;

        // Validate: ShopKeeper needs at least one shop
        if (behaviour == Behaviour.ShopKeeper && Shop.List.Count == 0)
        {
            cmbBehavior.SelectedIndex = (int)_selected.Behaviour;
            return;
        }

        _selected.Behaviour = behaviour;
        pnlShop.IsVisible = behaviour == Behaviour.ShopKeeper;

        if (behaviour != Behaviour.ShopKeeper)
        {
            cmbShop.SelectedIndex = -1;
        }
        else if (_selected.Shop == null && cmbShop.Items.Count > 0)
        {
            cmbShop.SelectedIndex = 0;
        }
    }

    private void cmbMovement_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Movement = (MovementStyle)cmbMovement.SelectedIndex;
    }

    private void numFlee_Health_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.FleeHealth = (byte)(e.NewValue ?? 0);
    }

    private void cmbShop_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        if (cmbShop.SelectedItem is Shop shop)
            _selected.Shop = shop;
    }

    private void RefreshDropList()
    {
        lstDrop.ItemsSource = null;
        lstDrop.ItemsSource = _selected?.Drop;
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
        if (_selected == null || cmbDrop_Item.SelectedItem is not Item item) return;
        _selected.Drop.Add(new NpcDrop(item, (short)(numDrop_Amount.Value ?? 1), (byte)(numDrop_Chance.Value ?? 100)));
        pnlDrop_Add.IsVisible = false;
        RefreshDropList();
    }

    private void butDrop_Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || lstDrop.SelectedItem is not NpcDrop drop) return;
        _selected.Drop.Remove(drop);
        RefreshDropList();
    }

    // ──────────────────────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────────────────────

    private void RefreshAlliesList()
    {
        lstAllies.ItemsSource = null;
        lstAllies.ItemsSource = _selected?.Allie;
    }

    private void chkAttackNpc_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.AttackNpc = chkAttackNpc.IsChecked ?? false;
        lstAllies.IsEnabled = _selected.AttackNpc;
        if (!_selected.AttackNpc)
        {
            _selected.Allie.Clear();
            RefreshAlliesList();
        }
    }

    private void butAllie_Add_Click(object? sender, RoutedEventArgs e)
    {
        if (!(_selected?.AttackNpc ?? false)) return;
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
        if (_selected == null || cmbAllie_Npc.SelectedItem is not Npc allie) return;
        if (!_selected.Allie.Contains(allie))
            _selected.Allie.Add(allie);
        pnlAllie_Add.IsVisible = false;
        RefreshAlliesList();
    }

    private void butAllie_Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || lstAllies.SelectedItem is not Npc allie) return;
        _selected.Allie.Remove(allie);
        RefreshAlliesList();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Save / Cancel
    // ──────────────────────────────────────────────────────────────────────────

    private void butSave_Click(object? sender, RoutedEventArgs e)
    {
        Send.WriteNpcs();
        Close();
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        Send.RequestNpcs();
        Close();
    }
}
