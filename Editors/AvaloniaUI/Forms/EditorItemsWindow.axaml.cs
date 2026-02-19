using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Client.Framework.Graphics;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Enums;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Editors.AvaloniaUI.Forms;

internal partial class EditorItemsWindow : Window
{
    // Consumed by Renders.cs EditorItem() instead of EditorItems.Form.numTexture.Value
    public static short CurrentTextureIndex { get; private set; } = 0;

    private Item? _selected;
    private bool _loading;

    public EditorItemsWindow()
    {
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
        var filter = txtFilter.Text ?? string.Empty;
        var filtered = Item.List.Values
            .Where(i => i.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        lstItems.ItemsSource = filtered;

        if (filtered.Count > 0 && lstItems.SelectedItem == null)
            lstItems.SelectedIndex = 0;

        pnlRight.IsVisible = lstItems.SelectedItem != null;
    }

    private void RefreshItemListKeepSelection()
    {
        var saved = _selected;
        _loading = true;

        var filter = txtFilter.Text ?? string.Empty;
        lstItems.ItemsSource = Item.List.Values
            .Where(i => i.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        lstItems.SelectedItem = saved;
        _loading = false;
    }

    private void txtFilter_TextChanged(object? sender, TextChangedEventArgs e)
    {
        RefreshItemList();
    }

    private void lstItems_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading) return;
        if (lstItems.SelectedItem is not Item item) return;
        LoadItem(item);
        pnlRight.IsVisible = true;
    }

    private void LoadItem(Item item)
    {
        _loading = true;
        _selected = item;

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

        CurrentTextureIndex = item.Texture;
        UpdateTexturePreview(item.Texture);

        _loading = false;
    }

    // ──────────────────────────────────────────────────────────
    // New / Remove
    // ──────────────────────────────────────────────────────────

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        var item = new Item();
        Item.List.Add(item.Id, item);

        RefreshItemList();
        lstItems.SelectedItem = Item.List.Values.FirstOrDefault(i => i.Id == item.Id);
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        Item.List.Remove(_selected.Id);
        _selected = null;
        RefreshItemList();
        pnlRight.IsVisible = lstItems.SelectedItem != null;
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void txtName_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Name = txtName.Text ?? string.Empty;
        RefreshItemListKeepSelection();
    }

    private void txtDescription_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Description = txtDescription.Text ?? string.Empty;
    }

    private void cmbType_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Type = (ItemType)cmbType.SelectedIndex;
        UpdateTypePanels((byte)cmbType.SelectedIndex);
    }

    private void UpdateTypePanels(byte typeIndex)
    {
        pnlPotion.IsVisible = typeIndex == (byte)ItemType.Potion;
        pnlEquipment.IsVisible = typeIndex == (byte)ItemType.Equipment;
    }

    private void numTexture_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        CurrentTextureIndex = (short)(e.NewValue ?? 0);
        if (_loading || _selected == null) return;
        _selected.Texture = CurrentTextureIndex;
        UpdateTexturePreview(CurrentTextureIndex);
    }

    private void cmbRarity_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Rarity = (Rarity)cmbRarity.SelectedIndex;
    }

    private void cmbBind_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Bind = (BindOn)cmbBind.SelectedIndex;
    }

    private void chkStackable_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Stackable = chkStackable.IsChecked ?? false;
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void numReq_Level_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.ReqLevel = (short)(e.NewValue ?? 0);
    }

    private void cmbReq_Class_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.ReqClass = cmbReq_Class.SelectedIndex == 0
            ? null
            : cmbReq_Class.SelectedItem as Class;
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void numPotion_HP_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.PotionVital[(byte)Vital.Hp] = (short)(e.NewValue ?? 0);
    }

    private void numPotion_MP_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.PotionVital[(byte)Vital.Mp] = (short)(e.NewValue ?? 0);
    }

    private void numPotion_Experience_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.PotionExperience = (int)(e.NewValue ?? 0);
    }

    // ──────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────

    private void cmbEquipment_Type_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.EquipType = (byte)cmbEquipment_Type.SelectedIndex;
        UpdateWeaponDamageVisibility(_selected.EquipType);
    }

    private void UpdateWeaponDamageVisibility(byte equipTypeIndex)
    {
        pnlWeaponDamage.IsVisible = equipTypeIndex == (byte)Equipment.Weapon;
    }

    private void numEquip_Strength_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.EquipAttribute[(byte)Attribute.Strength] = (short)(e.NewValue ?? 0);
    }

    private void numEquip_Resistance_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.EquipAttribute[(byte)Attribute.Resistance] = (short)(e.NewValue ?? 0);
    }

    private void numEquip_Intelligence_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.EquipAttribute[(byte)Attribute.Intelligence] = (short)(e.NewValue ?? 0);
    }

    private void numEquip_Agility_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.EquipAttribute[(byte)Attribute.Agility] = (short)(e.NewValue ?? 0);
    }

    private void numEquip_Vitality_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.EquipAttribute[(byte)Attribute.Vitality] = (short)(e.NewValue ?? 0);
    }

    private void numWeapon_Damage_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.WeaponDamage = (short)(e.NewValue ?? 0);
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
        Send.WriteItems();
        Close();
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        Send.RequestItems();
        Close();
    }
}
