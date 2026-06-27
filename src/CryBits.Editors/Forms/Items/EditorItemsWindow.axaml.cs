using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Client.Framework.Graphics;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Network;

namespace CryBits.Editors.Forms.Items;

internal partial class EditorItemsWindow : Window
{
    private readonly DefinitionCatalog _catalog;
    private ItemEditorViewModel? _viewModel;

    /// <summary>Opens the Items editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorItemsWindow(DefinitionCatalog.Instance);
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    // Consumed by Renders.cs EditorItem() instead of EditorItems.Form.numTexture.Value
    public static short CurrentTextureIndex { get; private set; } = 0;

    private Item? _selected;

    public EditorItemsWindow(DefinitionCatalog catalog)
    {
        _catalog = catalog;
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
        foreach (var cls in _catalog.Classes.Values)
            cmbReq_Class.Items.Add(cls);

        RefreshItemList();
    }

    // ──────────────────────────────────────────────────────────
    // List management
    // ──────────────────────────────────────────────────────────

    private void RefreshItemList()
    {
        var filter = txtFilter.Text ?? string.Empty;
        var filtered = _catalog.Items.Values
            .Where(i => i.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        lstItems.ItemsSource = filtered;

        if (filtered.Count > 0 && lstItems.SelectedItem == null)
            lstItems.SelectedIndex = 0;

        pnlRight.IsVisible = lstItems.SelectedItem != null;
    }

    private void txtFilter_TextChanged(object? sender, TextChangedEventArgs e)
    {
        RefreshItemList();
    }

    private void lstItems_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (lstItems.SelectedItem is not Item item) return;
        _selected = item;
        _viewModel = new ItemEditorViewModel(item);
        DataContext = _viewModel;

        numTexture.Maximum = Math.Max(0, Textures.Items.Count - 1);

        cmbType.SelectedIndex = (byte)item.Type;
        cmbReq_Class.SelectedIndex = item.ReqClassId.HasValue ? cmbReq_Class.Items.IndexOf(_catalog.Classes.Get(item.ReqClassId.Value)) : 0;
        cmbEquipment_Type.SelectedIndex = item.EquipType;

        UpdateTypePanels((byte)item.Type);
        UpdateWeaponDamageVisibility(item.EquipType);

        CurrentTextureIndex = item.Texture;
        UpdateTexturePreview(item.Texture);

        pnlRight.IsVisible = true;
    }

    // ──────────────────────────────────────────────────────────
    // New / Remove
    // ──────────────────────────────────────────────────────────

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        var item = new Item();
        _catalog.Items.Add(item.Id, item);

        RefreshItemList();
        lstItems.SelectedItem = _catalog.Items.Values.FirstOrDefault(i => i.Id == item.Id);
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        _catalog.Items.Remove(_selected.Id);
        _selected = null;
        _viewModel = null;
        DataContext = null;
        RefreshItemList();
        pnlRight.IsVisible = lstItems.SelectedItem != null;
    }

    // ──────────────────────────────────────────────────────────

    private void cmbType_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_selected == null) return;
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
        UpdateTexturePreview(CurrentTextureIndex);
    }

    private void cmbReq_Class_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_selected == null) return;
        _selected.ReqClassId = cmbReq_Class.SelectedIndex == 0
            ? null
            : (cmbReq_Class.SelectedItem as Class)?.Id;
    }

    private void cmbEquipment_Type_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_selected == null) return;
        _selected.EquipType = (byte)cmbEquipment_Type.SelectedIndex;
        UpdateWeaponDamageVisibility(_selected.EquipType);
    }

    private void UpdateWeaponDamageVisibility(byte equipTypeIndex)
    {
        pnlWeaponDamage.IsVisible = equipTypeIndex == (byte)Equipment.Weapon;
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
        PackageSender.Instance.WriteItems();
        Close();
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        PackageSender.Instance.RequestItems();
        Close();
    }
}
