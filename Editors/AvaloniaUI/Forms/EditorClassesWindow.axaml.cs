using System;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CryBits.Client.Framework.Graphics;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using Map = CryBits.Entities.Map.Map;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Editors.AvaloniaUI.Forms;

internal partial class EditorClassesWindow : Window
{
    // Consumed by Renders.cs EditorClass() instead of EditorClasses.Form.numTexture.Value
    public static short CurrentTextureIndex { get; private set; } = 1;

    private Class? _selected;
    private bool _loading;
    private bool _addingToMale;

    public EditorClassesWindow()
    {
        InitializeComponent();

        // Populate comboboxes from live data
        cmbItems.ItemsSource = Item.List.Values.ToList();
        cmbSpawn_Map.ItemsSource = Map.List.Values.ToList();

        // Wire SFML window: fires first time sfmlHost becomes part of the layout tree
        // (replaced with WriteableBitmap rendering - no native host needed)

        // Filter textbox change
        txtFilter.TextChanged += txtFilter_TextChanged;

        // Initial list population
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
        var filter = txtFilter.Text ?? string.Empty;
        var filtered = Class.List.Values
            .Where(c => c.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        lstClasses.ItemsSource = filtered;

        if (filtered.Count > 0 && lstClasses.SelectedItem == null)
            lstClasses.SelectedIndex = 0;

        pnlContent.IsVisible = lstClasses.SelectedItem != null;
    }

    private void RefreshClassListKeepSelection()
    {
        var savedSelected = _selected;
        _loading = true;

        var filter = txtFilter.Text ?? string.Empty;
        lstClasses.ItemsSource = Class.List.Values
            .Where(c => c.Name.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        lstClasses.SelectedItem = savedSelected;
        _loading = false;
    }

    private void lstClasses_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading) return;
        if (lstClasses.SelectedItem is not Class cls) return;
        LoadClass(cls);
        pnlContent.IsVisible = true;
    }

    private void LoadClass(Class cls)
    {
        _loading = true;
        _selected = cls;

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

        _loading = false;
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

    // ──────────────────────────────────────────────────────────
    // Filter
    // ──────────────────────────────────────────────────────────

    private void txtFilter_TextChanged(object? sender, TextChangedEventArgs e)
    {
        RefreshClassList();
    }

    // ──────────────────────────────────────────────────────────
    // New / Remove
    // ──────────────────────────────────────────────────────────

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        var cls = new Class();
        Class.List.Add(cls.Id, cls);
        RefreshClassList();
        lstClasses.SelectedItem = Class.List.Values.FirstOrDefault(c => c.Id == cls.Id);
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null) return;

        if (Class.List.Count == 1)
        {
            // Must have at least one class – silently ignore (or show a dialog)
            return;
        }

        Class.List.Remove(_selected.Id);
        _selected = null;
        RefreshClassList();
        pnlContent.IsVisible = lstClasses.SelectedItem != null;
    }

    // ──────────────────────────────────────────────────────────
    // General
    // ──────────────────────────────────────────────────────────

    private void txtName_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Name = txtName.Text ?? string.Empty;
        RefreshClassListKeepSelection();
    }

    private void txtDescription_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.Description = txtDescription.Text ?? string.Empty;
    }

    // ──────────────────────────────────────────────────────────
    // Attributes
    // ──────────────────────────────────────────────────────────

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

    // ──────────────────────────────────────────────────────────
    // Textures
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

        var sfmlTexture = Textures.Characters[textureIndex];
        var sfmlImage = sfmlTexture.CopyToImage();
        var fullW = (int)sfmlImage.Size.X;
        var fullH = (int)sfmlImage.Size.Y;

        // Spritesheet is 4 cols × 4 rows – preview just the first frame (top-left)
        var frameW = fullW / 4;
        var frameH = fullH / 4;

        var pixelBytes = sfmlImage.Pixels; // RGBA byte[] for the entire image

        var bitmap = new WriteableBitmap(
            new PixelSize(frameW, frameH),
            new Avalonia.Vector(96, 96),
            PixelFormat.Rgba8888,
            AlphaFormat.Unpremul);

        using var fb = bitmap.Lock();
        for (int y = 0; y < frameH; y++)
        {
            var srcOffset = y * fullW * 4;
            var dstPtr = fb.Address + y * fb.RowBytes;
            Marshal.Copy(pixelBytes, srcOffset, dstPtr, frameW * 4);
        }

        imgTexturePreview.Source = bitmap;
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

    // ──────────────────────────────────────────────────────────
    // Initial items
    // ──────────────────────────────────────────────────────────

    private void butItem_Add_Click(object? sender, RoutedEventArgs e)
    {
        if (Item.List.Count == 0) return;   // no items registered

        cmbItems.SelectedIndex = 0;
        numItem_Amount.Value = 1;
        pnlDrop.IsVisible = false;
        pnlItemAdd.IsVisible = true;
    }

    private void butItem_Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || cmbItems.SelectedItem is not Item item) return;
        _selected.Item.Add(new ItemSlot(item, (short)(numItem_Amount.Value ?? 1)));
        RefreshItemList();
        HideOverlays();
    }

    private void butItem_Delete_Click(object? sender, RoutedEventArgs e)
    {
        if (_selected == null || lstItems.SelectedIndex < 0) return;
        _selected.Item.RemoveAt(lstItems.SelectedIndex);
        RefreshItemList();
    }

    // ──────────────────────────────────────────────────────────
    // Spawn
    // ──────────────────────────────────────────────────────────

    private void cmbSpawn_Map_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        if (cmbSpawn_Map.SelectedItem is Map map) _selected.SpawnMap = map;
    }

    private void cmbSpawn_Direction_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.SpawnDirection = (byte)cmbSpawn_Direction.SelectedIndex;
    }

    private void numSpawn_X_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.SpawnX = (byte)(e.NewValue ?? 0);
    }

    private void numSpawn_Y_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (_loading || _selected == null) return;
        _selected.SpawnY = (byte)(e.NewValue ?? 0);
    }

    // ──────────────────────────────────────────────────────────
    // Save / Cancel
    // ──────────────────────────────────────────────────────────

    private void butSave_Click(object? sender, RoutedEventArgs e)
    {
        Send.WriteClasses();
        Close();
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        Send.RequestClasses();
        Close();
    }
}
