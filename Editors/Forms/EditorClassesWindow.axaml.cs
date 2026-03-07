using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Client.Framework.Graphics;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.ViewModels;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using Attribute = CryBits.Enums.Attribute;
using Map = CryBits.Entities.Map.Map;

namespace CryBits.Editors.Forms;

internal partial class EditorClassesWindow : Window
{
    /// <summary>Opens the Classes editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        if (!EditorClassesViewModel.CanOpen())
        {
            MessageBox.Show("It must have at least one map registered before editing classes.");
            return;
        }

        owner.Hide();
        var window = new EditorClassesWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private readonly EditorClassesViewModel _vm = new();
    private bool _addingToMale;

    public EditorClassesWindow()
    {
        DataContext = _vm;
        InitializeComponent();

        // Populate comboboxes from live data
        cmbItems.ItemsSource = Item.List.Values.ToList();
        cmbSpawn_Map.ItemsSource = Map.List.Values.ToList();

        txtFilter.TextChanged += txtFilter_TextChanged;

        RefreshClassList();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
    }

    // ──────────────────────────────────────────────────────────
    // Class list
    // ──────────────────────────────────────────────────────────

    private void RefreshClassList()
    {
        var filtered = _vm.FilteredClasses.ToList();
        lstClasses.ItemsSource = filtered;

        if (filtered.Count > 0 && lstClasses.SelectedItem == null)
            lstClasses.SelectedIndex = 0;

        pnlContent.IsVisible = lstClasses.SelectedItem != null;
    }

    private void RefreshClassListKeepSelection()
    {
        var savedSelected = _vm.Selected;
        _vm.BeginLoad();

        lstClasses.ItemsSource = _vm.FilteredClasses.ToList();
        lstClasses.SelectedItem = savedSelected;
        _vm.EndLoad();
    }

    private void lstClasses_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_vm.IsLoading) return;
        if (lstClasses.SelectedItem is not Class cls) return;
        LoadClass(cls);
        pnlContent.IsVisible = true;
    }

    private void LoadClass(Class cls)
    {
        _vm.BeginLoad();
        _vm.Selected = cls;

        txtName.Text = cls.Name;
        txtDescription.Text = cls.Description;

        numHP.Value = cls.Vital[(byte)Vital.Hp];
        numMP.Value = cls.Vital[(byte)Vital.Mp];

        numStrength.Value = cls.Attribute[(byte)Attribute.Strength];
        numResistance.Value = cls.Attribute[(byte)Attribute.Resistance];
        numIntelligence.Value = cls.Attribute[(byte)Attribute.Intelligence];
        numAgility.Value = cls.Attribute[(byte)Attribute.Agility];
        numVitality.Value = cls.Attribute[(byte)Attribute.Vitality];

        numSpawn_X.Maximum = Map.Width - 1;
        numSpawn_Y.Maximum = Map.Height - 1;
        numSpawn_X.Value = cls.SpawnX;
        numSpawn_Y.Value = cls.SpawnY;

        cmbSpawn_Map.SelectedItem = Map.List.Values.FirstOrDefault(m => m == cls.SpawnMap);
        cmbSpawn_Direction.SelectedIndex = cls.SpawnDirection;

        numTexture.Maximum = Textures.Characters.Count - 1;

        RefreshTextureLists();
        RefreshItemList();
        HideOverlays();

        _vm.EndLoad();
    }

    private void RefreshTextureLists()
    {
        lstMale.ItemsSource = _vm.Selected?.TextureMale?.ToList();
        lstFemale.ItemsSource = _vm.Selected?.TextureFemale?.ToList();
    }

    private void RefreshItemList()
    {
        lstItems.ItemsSource = _vm.Selected?.Item?.ToList();
    }

    private void HideOverlays()
    {
        pnlTexture.IsVisible = true;
        pnlTextureAdd.IsVisible = false;
        pnlDrop.IsVisible = true;
        pnlItemAdd.IsVisible = false;
    }

    private void txtFilter_TextChanged(object? sender, TextChangedEventArgs e)
    {
        _vm.Filter = txtFilter.Text ?? string.Empty;
        RefreshClassList();
    }

    // ──────────────────────────────────────────────────────────
    // New / Remove
    // ──────────────────────────────────────────────────────────

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        var cls = _vm.Add();
        RefreshClassList();
        lstClasses.SelectedItem = Class.List.Values.FirstOrDefault(c => c.Id == cls.Id);
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        if (!_vm.Remove()) return;
        RefreshClassList();
        pnlContent.IsVisible = lstClasses.SelectedItem != null;
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void txtName_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Name = txtName.Text ?? string.Empty;
        RefreshClassListKeepSelection();
    }

    private void txtDescription_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Description = txtDescription.Text ?? string.Empty;
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

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

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void butMTexture_Click(object? sender, RoutedEventArgs e)
    {
        _addingToMale = true;
        numTexture.Value = 1;
        pnlTexture.IsVisible = false;
        pnlTextureAdd.IsVisible = true;
        UpdateTexturePreview((short)(numTexture.Value ?? 1));
    }

    private void butFTexture_Click(object? sender, RoutedEventArgs e)
    {
        _addingToMale = false;
        numTexture.Value = 1;
        pnlTexture.IsVisible = false;
        pnlTextureAdd.IsVisible = true;
        UpdateTexturePreview((short)(numTexture.Value ?? 1));
    }

    private void butMDelete_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || lstMale.SelectedIndex < 0) return;
        _vm.Selected.TextureMale.RemoveAt(lstMale.SelectedIndex);
        RefreshTextureLists();
    }

    private void butFDelete_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || lstFemale.SelectedIndex < 0) return;
        _vm.Selected.TextureFemale.RemoveAt(lstFemale.SelectedIndex);
        RefreshTextureLists();
    }

    private void numTexture_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        _vm.CurrentTextureIndex = (short)(e.NewValue ?? 1);
        UpdateTexturePreview(_vm.CurrentTextureIndex);
    }

    /// <summary>
    /// Converts the first animation frame of an SFML character spritesheet
    /// to an Avalonia WriteableBitmap shown inside imgTexturePreview.
    /// </summary>
    private void UpdateTexturePreview(short textureIndex)
    {
        if (textureIndex <= 0 || textureIndex >= Textures.Characters.Count)
        {
            imgTexturePreview.Source = null;
            return;
        }

        // Spritesheet is 4 cols × 4 rows – preview just the first frame (top-left)
        SfmlRenderBlit.BlitTexture(Textures.Characters[textureIndex], imgTexturePreview, 4, 4);
    }

    private void butTexture_Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null) return;
        var idx = (short)(numTexture.Value ?? 1);

        if (_addingToMale)
            _vm.Selected.TextureMale.Add(idx);
        else
            _vm.Selected.TextureFemale.Add(idx);

        RefreshTextureLists();
        HideOverlays();
    }

    // ──────────────────────────────────────────────────────────
    // Initial items
    // ──────────────────────────────────────────────────────────

    private void butItem_Add_Click(object? sender, RoutedEventArgs e)
    {
        if (Item.List.Count == 0) return;

        cmbItems.SelectedIndex = 0;
        numItem_Amount.Value = 1;
        pnlDrop.IsVisible = false;
        pnlItemAdd.IsVisible = true;
    }

    private void butItem_Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || cmbItems.SelectedItem is not Item item) return;
        _vm.Selected.Item.Add(new ItemSlot(item, (short)(numItem_Amount.Value ?? 1)));
        RefreshItemList();
        HideOverlays();
    }

    private void butItem_Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null || lstItems.SelectedIndex < 0) return;
        _vm.Selected.Item.RemoveAt(lstItems.SelectedIndex);
        RefreshItemList();
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void cmbSpawn_Map_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        if (cmbSpawn_Map.SelectedItem is Map map) _vm.Selected!.SpawnMap = map;
    }

    private void cmbSpawn_Direction_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.SpawnDirection = (byte)cmbSpawn_Direction.SelectedIndex;
    }

    private void numSpawn_X_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.SpawnX = (byte)(e.NewValue ?? 0);
    }

    private void numSpawn_Y_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.SpawnY = (byte)(e.NewValue ?? 0);
    }

    // ──────────────────────────────────────────────────────────
    // Save / Cancel
    // ──────────────────────────────────────────────────────────

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
