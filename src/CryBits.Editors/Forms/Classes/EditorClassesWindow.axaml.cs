using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Editors.Network;
using CryBits.Editors.Utils;
using Class = CryBits.Definitions.Classes.Class;
using Map = CryBits.Definitions.Maps.Map;

namespace CryBits.Editors.Forms.Classes;

internal partial class EditorClassesWindow : Window
{
    private readonly DefinitionCatalog _catalog;
    private readonly PackageSender _sender;
    private ClassEditorViewModel? _viewModel;

    public static void Open(Window owner, DefinitionCatalog catalog, PackageSender sender)
    {
        if (catalog.Maps.Count == 0)
        {
            MessageBox.Show("It must have at least one map registered before editing classes.");
            return;
        }

        owner.Hide();
        var window = new EditorClassesWindow(catalog, sender);
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private static short CurrentTextureIndex { get; set; } = 1;

    private Class? _selected;
    private bool _addingToMale;

    public EditorClassesWindow(DefinitionCatalog catalog, PackageSender sender)
    {
        _catalog = catalog;
        _sender = sender;
        InitializeComponent();

        cmbItems.ItemsSource = _catalog.Items.Values.ToList();
        cmbSpawn_Map.ItemsSource = _catalog.Maps.Values.ToList();

        RefreshClassList();
    }

    private void RefreshClassList()
    {
        var filter = txtFilter.Text ?? string.Empty;
        var filtered = _catalog.Classes.Values
            .Where(c => c.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        lstClasses.ItemsSource = filtered;
        if (filtered.Count > 0 && lstClasses.SelectedItem == null)
            lstClasses.SelectedIndex = 0;

        pnlContent.IsVisible = lstClasses.SelectedItem != null;
    }

    private void lstClasses_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (lstClasses.SelectedItem is not Class cls) return;
        _selected = cls;
        _viewModel = new ClassEditorViewModel(cls, _catalog, _sender);
        DataContext = _viewModel;
        _viewModel.RequestClose += () => Close();
        _viewModel.RequestRefreshList += RefreshClassList;

        numTexture.Maximum = Textures.Characters.Count - 1;

        cmbSpawn_Map.SelectedItem = _catalog.Maps.Values.FirstOrDefault(m => m.Id == cls.SpawnMapId);

        RefreshTextureLists();
        RefreshItemList();
        HideOverlays();
        pnlContent.IsVisible = true;
    }

    private void RefreshTextureLists()
    {
        lstMale.ItemsSource = _selected?.TextureMale?.ToList();
        lstFemale.ItemsSource = _selected?.TextureFemale?.ToList();
    }

    private void RefreshItemList()
    {
        lstItems.ItemsSource = _selected?.Item?.ToList();
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
        RefreshClassList();
    }

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
        if (_selected == null || lstMale.SelectedIndex < 0) return;
        _selected.TextureMale.RemoveAt(lstMale.SelectedIndex);
        RefreshTextureLists();
    }

    private void butFDelete_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || lstFemale.SelectedIndex < 0) return;
        _selected.TextureFemale.RemoveAt(lstFemale.SelectedIndex);
        RefreshTextureLists();
    }

    private void numTexture_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        CurrentTextureIndex = (short)(e.NewValue ?? 1);
        UpdateTexturePreview(CurrentTextureIndex);
    }

    private void UpdateTexturePreview(short textureIndex)
    {
        if (textureIndex <= 0 || textureIndex >= Textures.Characters.Count)
        {
            imgTexturePreview.Source = null;
            return;
        }

        imgTexturePreview.Blit(Textures.Characters[textureIndex], cols: 4, rows: 4);
    }

    private void butTexture_Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;
        var idx = (short)(numTexture.Value ?? 1);

        if (_addingToMale)
            _selected.TextureMale.Add(idx);
        else
            _selected.TextureFemale.Add(idx);

        RefreshTextureLists();
        HideOverlays();
    }

    private void butItem_Add_Click(object? sender, RoutedEventArgs e)
    {
        if (_catalog.Items.Count == 0) return;

        cmbItems.SelectedIndex = 0;
        numItem_Amount.Value = 1;
        pnlDrop.IsVisible = false;
        pnlItemAdd.IsVisible = true;
    }

    private void butItem_Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || cmbItems.SelectedItem is not Item item) return;
        _selected.Item.Add(new ItemSlot(item.Id, (short)(numItem_Amount.Value ?? 1)));
        RefreshItemList();
        HideOverlays();
    }

    private void butItem_Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || lstItems.SelectedIndex < 0) return;
        _selected.Item.RemoveAt(lstItems.SelectedIndex);
        RefreshItemList();
    }

    private void cmbSpawn_Map_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_selected == null) return;
        if (cmbSpawn_Map.SelectedItem is Map map) _selected.SpawnMapId = map.Id;
    }
}
