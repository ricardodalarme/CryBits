using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Editors.Utils;

namespace CryBits.Editors.Forms.Items;

internal partial class EditorItemsWindow : Window
{
    private readonly DefinitionCatalog _catalog;
    private ItemEditorViewModel? _viewModel;

    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorItemsWindow(Program.Catalog);
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    public static short CurrentTextureIndex { get; private set; }

    private Item? _selected;
    private WriteableBitmap? _textureBitmap;

    public EditorItemsWindow(DefinitionCatalog catalog)
    {
        _catalog = catalog;
        InitializeComponent();

        numTexture.Maximum = Textures.Items.Count - 1;

        for (byte i = 0; i < (byte)Rarity.Count; i++)
            cmbRarity.Items.Add((Rarity)i);

        for (byte i = 0; i < (byte)BindOn.Count; i++)
            cmbBind.Items.Add((BindOn)i);

        cmbReq_Class.Items.Add("None");
        foreach (var cls in _catalog.Classes.Values)
            cmbReq_Class.Items.Add(cls);

        RefreshItemList();
    }

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
        _viewModel = new ItemEditorViewModel(item, _catalog);
        DataContext = _viewModel;

        _viewModel.RequestClose += Close;
        _viewModel.RequestRefreshList += RefreshItemList;
        _viewModel.RequestSelectItem +=
            i => lstItems.SelectedItem = _catalog.Items.Values.FirstOrDefault(x => x.Id == i.Id);

        numTexture.Maximum = Math.Max(0, Textures.Items.Count - 1);

        cmbType.SelectedIndex = (byte)item.Type;
        cmbReq_Class.SelectedIndex = item.ReqClassId.HasValue
            ? cmbReq_Class.Items.IndexOf(_catalog.Classes.Get(item.ReqClassId.Value))
            : 0;
        cmbEquipment_Type.SelectedIndex = item.EquipType;

        UpdateTypePanels((byte)item.Type);
        UpdateWeaponDamageVisibility(item.EquipType);

        CurrentTextureIndex = item.Texture;
        UpdateTexturePreview(item.Texture);

        pnlRight.IsVisible = true;
    }

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
        _selected?.ReqClassId = cmbReq_Class.SelectedIndex == 0
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

    private void UpdateTexturePreview(short textureIndex)
    {
        if (textureIndex <= 0 || textureIndex >= Textures.Items.Count)
        {
            imgTexturePreview.Source = null;
            return;
        }

        SfmlRenderBlit.BlitTexture(Textures.Items[textureIndex], ref _textureBitmap, imgTexturePreview);
    }
}
