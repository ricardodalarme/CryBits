using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Client.Framework.Graphics;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.ViewModels;
using CryBits.Entities;
using CryBits.Enums;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Editors.Forms;

internal partial class EditorItemsWindow : Window
{
    /// <summary>Opens the Items editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorItemsWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private readonly EditorItemsViewModel _vm = new();

    public EditorItemsWindow()
    {
        DataContext = _vm;
        InitializeComponent();

        // Set texture upper bound
        numTexture.Maximum = Textures.Items.Count - 1;

        // Populate Rarity combo
        for (byte i = 0; i < (byte)Rarity.Count; i++)
            cmbRarity.Items.Add((Rarity)i);

        // Populate BindOn combo
        for (byte i = 0; i < (byte)BindOn.Count; i++)
            cmbBind.Items.Add((BindOn)i);

        // Populate class requirement combo
        cmbReq_Class.Items.Add("None");
        foreach (var cls in Class.List.Values)
            cmbReq_Class.Items.Add(cls);

        RefreshItemList();
    }

    // ──────────────────────────────────────────────────────────
    // List management
    // ──────────────────────────────────────────────────────────

    private void RefreshItemList()
    {
        var filtered = _vm.FilteredItems.ToList();
        lstItems.ItemsSource = filtered;

        if (filtered.Count > 0 && lstItems.SelectedItem == null)
            lstItems.SelectedIndex = 0;

        pnlRight.IsVisible = lstItems.SelectedItem != null;
    }

    private void RefreshItemListKeepSelection()
    {
        var saved = _vm.Selected;
        _vm.BeginLoad();

        lstItems.ItemsSource = _vm.FilteredItems.ToList();
        lstItems.SelectedItem = saved;
        _vm.EndLoad();
    }

    private void txtFilter_TextChanged(object? sender, TextChangedEventArgs e)
    {
        _vm.Filter = txtFilter.Text ?? string.Empty;
        RefreshItemList();
    }

    private void lstItems_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_vm.IsLoading) return;
        if (lstItems.SelectedItem is not Item item) return;
        LoadItem(item);
        pnlRight.IsVisible = true;
    }

    private void LoadItem(Item item)
    {
        _vm.BeginLoad();
        _vm.Selected = item;

        txtName.Text = item.Name;
        txtDescription.Text = item.Description;
        numTexture.Value = item.Texture;
        cmbType.SelectedIndex = (byte)item.Type;
        chkStackable.IsChecked = item.Stackable;
        cmbBind.SelectedIndex = (byte)item.Bind;
        cmbRarity.SelectedIndex = (byte)item.Rarity;
        numReq_Level.Value = item.ReqLevel;
        cmbReq_Class.SelectedIndex = item.ReqClass != null ? cmbReq_Class.Items.IndexOf(item.ReqClass) : 0;
        numPotion_Experience.Value = item.PotionExperience;
        numPotion_HP.Value = item.PotionVital[(byte)Vital.Hp];
        numPotion_MP.Value = item.PotionVital[(byte)Vital.Mp];
        cmbEquipment_Type.SelectedIndex = item.EquipType;
        numEquip_Strength.Value = item.EquipAttribute[(byte)Attribute.Strength];
        numEquip_Resistance.Value = item.EquipAttribute[(byte)Attribute.Resistance];
        numEquip_Intelligence.Value = item.EquipAttribute[(byte)Attribute.Intelligence];
        numEquip_Agility.Value = item.EquipAttribute[(byte)Attribute.Agility];
        numEquip_Vitality.Value = item.EquipAttribute[(byte)Attribute.Vitality];
        numWeapon_Damage.Value = item.WeaponDamage;

        UpdateTypePanels((byte)item.Type);
        UpdateWeaponDamageVisibility(item.EquipType);

        _vm.CurrentTextureIndex = item.Texture;
        UpdateTexturePreview(item.Texture);

        _vm.EndLoad();
    }

    // ──────────────────────────────────────────────────────────
    // New / Remove
    // ──────────────────────────────────────────────────────────

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        var item = _vm.Add();
        RefreshItemList();
        lstItems.SelectedItem = Item.List.Values.FirstOrDefault(i => i.Id == item.Id);
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        _vm.Remove();
        RefreshItemList();
        pnlRight.IsVisible = lstItems.SelectedItem != null;
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void txtName_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Name = txtName.Text ?? string.Empty;
        RefreshItemListKeepSelection();
    }

    private void txtDescription_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Description = txtDescription.Text ?? string.Empty;
    }

    private void cmbType_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Type = (ItemType)cmbType.SelectedIndex;
        UpdateTypePanels((byte)cmbType.SelectedIndex);
    }

    private void UpdateTypePanels(byte typeIndex)
    {
        pnlPotion.IsVisible = typeIndex == (byte)ItemType.Potion;
        pnlEquipment.IsVisible = typeIndex == (byte)ItemType.Equipment;
    }

    private void numTexture_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        _vm.CurrentTextureIndex = (short)(e.NewValue ?? 0);
        if (!_vm.CanEdit) return;
        _vm.Selected!.Texture = _vm.CurrentTextureIndex;
        UpdateTexturePreview(_vm.CurrentTextureIndex);
    }

    private void cmbRarity_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Rarity = (Rarity)cmbRarity.SelectedIndex;
    }

    private void cmbBind_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Bind = (BindOn)cmbBind.SelectedIndex;
    }

    private void chkStackable_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.Stackable = chkStackable.IsChecked ?? false;
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void numReq_Level_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.ReqLevel = (short)(e.NewValue ?? 0);
    }

    private void cmbReq_Class_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.ReqClass = cmbReq_Class.SelectedIndex == 0
            ? null
            : cmbReq_Class.SelectedItem as Class;
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void numPotion_HP_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.PotionVital[(byte)Vital.Hp] = (short)(e.NewValue ?? 0);
    }

    private void numPotion_MP_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.PotionVital[(byte)Vital.Mp] = (short)(e.NewValue ?? 0);
    }

    private void numPotion_Experience_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.PotionExperience = (int)(e.NewValue ?? 0);
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void cmbEquipment_Type_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.EquipType = (byte)cmbEquipment_Type.SelectedIndex;
        UpdateWeaponDamageVisibility(_vm.Selected!.EquipType);
    }

    private void UpdateWeaponDamageVisibility(byte equipTypeIndex)
    {
        pnlWeaponDamage.IsVisible = equipTypeIndex == (byte)Equipment.Weapon;
    }

    private void numEquip_Strength_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.EquipAttribute[(byte)Attribute.Strength] = (short)(e.NewValue ?? 0);
    }

    private void numEquip_Resistance_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.EquipAttribute[(byte)Attribute.Resistance] = (short)(e.NewValue ?? 0);
    }

    private void numEquip_Intelligence_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.EquipAttribute[(byte)Attribute.Intelligence] = (short)(e.NewValue ?? 0);
    }

    private void numEquip_Agility_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.EquipAttribute[(byte)Attribute.Agility] = (short)(e.NewValue ?? 0);
    }

    private void numEquip_Vitality_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.EquipAttribute[(byte)Attribute.Vitality] = (short)(e.NewValue ?? 0);
    }

    private void numWeapon_Damage_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEdit) return;
        _vm.Selected!.WeaponDamage = (short)(e.NewValue ?? 0);
    }

    // ──────────────────────────────────────────────────────────
    // Texture preview
    // ──────────────────────────────────────────────────────────

    /// <summary>
    /// Converts the full SFML item texture to an Avalonia WriteableBitmap
    /// shown inside imgTexturePreview (no frame splitting, items are single icons).
    /// </summary>
    private void UpdateTexturePreview(short textureIndex)
    {
        if (textureIndex <= 0 || textureIndex >= Textures.Items.Count)
        {
            imgTexturePreview.Source = null;
            return;
        }

        SfmlRenderBlit.BlitTexture(Textures.Items[textureIndex], imgTexturePreview);
    }

    // ──────────────────────────────────────────────────────────
    // Save / Cancel
    // ──────────────────────────────────────────────────────────

    private void butSaveAll_Click(object? sender, RoutedEventArgs e)
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
