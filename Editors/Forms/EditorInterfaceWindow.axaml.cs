using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CryBits.Editors.AvaloniaUI;
using CryBits.Editors.Entities;
using CryBits.Editors.Graphics;
using CryBits.Editors.ViewModels;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using Button = CryBits.Client.Framework.Interfacily.Components.Button;
using CheckBox = CryBits.Client.Framework.Interfacily.Components.CheckBox;
using Component = CryBits.Client.Framework.Interfacily.Components.Component;
using Label = CryBits.Client.Framework.Interfacily.Components.Label;
using Panel = CryBits.Client.Framework.Interfacily.Components.Panel;
using Picture = CryBits.Client.Framework.Interfacily.Components.Picture;
using Point = System.Drawing.Point;
using ProgressBar = CryBits.Client.Framework.Interfacily.Components.ProgressBar;
using SlotGrid = CryBits.Client.Framework.Interfacily.Components.SlotGrid;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Editors.Forms;

// ─── Editor window ───────────────────────────────────────────────────────────
internal partial class EditorInterfaceWindow : Window
{
    /// <summary>Opens the Interface editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorInterfaceWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    private readonly EditorInterfaceViewModel _vm = new();

    private WriteableBitmap? _previewBitmap;
    private DispatcherTimer? _timer;

    public EditorInterfaceWindow()
    {
        DataContext = _vm;
        InitializeComponent();

        // Populate window combo from tree
        foreach (var node in InterfaceData.Tree.Nodes)
            cmbWindows.Items.Add(node.Text);

        if (cmbWindows.Items.Count > 0)
            cmbWindows.SelectedIndex = 0;

        // Create offscreen SFML render target (933 × 702 to match the WinForms canvas)
        Renders.WinInterfaceRT = new RenderTexture(new Vector2u(933, 702));

        // Start refresh timer
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(33) };
        _timer.Tick += OnRenderTick;
        _timer.Start();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer?.Stop();
        Renders.WinInterfaceRT = null;
        base.OnClosed(e);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // SFML render tick → WriteableBitmap
    // ──────────────────────────────────────────────────────────────────────────

    private void OnRenderTick(object? sender, EventArgs e)
    {
        if (Renders.WinInterfaceRT == null) return;
        if (InterfaceData.Tree.Nodes.Count == 0) return;

        Renders.Interface();
        SfmlRenderBlit.Blit(Renders.WinInterfaceRT, ref _previewBitmap, imgPreview);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Window combo
    // ──────────────────────────────────────────────────────────────────────────

    private void cmbWindows_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        _vm.SetSelectedWindowIndex(cmbWindows.SelectedIndex);
        treOrder.ItemsSource = _vm.RootChildren;
        UpdatePropertiesPanel();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Tree management
    // ──────────────────────────────────────────────────────────────────────────

    private void treOrder_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Use e.AddedItems — treOrder.SelectedItem may still be null at event time
        var node = e.AddedItems.Count > 0 ? e.AddedItems[0] as TreeItemVM : null;
        _vm.SelectNode(node);
        UpdatePropertiesPanel();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // New / Remove / Pin / Unpin / Up / Down
    // ──────────────────────────────────────────────────────────────────────────

    private void butNew_Click(object? sender, RoutedEventArgs e)
    {
        cmbType.SelectedIndex = 0;
        pnlNew.IsVisible = true;
    }

    private void butCancelNew_Click(object? sender, RoutedEventArgs e)
    {
        pnlNew.IsVisible = false;
    }

    private void butConfirm_Click(object? sender, RoutedEventArgs e)
    {
        var newVM = _vm.AddComponent(cmbType.SelectedIndex);
        pnlNew.IsVisible = false;
        if (newVM != null)
            treOrder.SelectedItem = newVM;
    }

    private void butRemove_Click(object? sender, RoutedEventArgs e)
    {
        _vm.RemoveSelected();
        UpdatePropertiesPanel();
    }

    private void butOrderPin_Click(object? sender, RoutedEventArgs e)
    {
        _vm.PinSelected();
    }

    private void butOrderUnpin_Click(object? sender, RoutedEventArgs e)
    {
        _vm.UnpinSelected();
    }

    private void butOrderUp_Click(object? sender, RoutedEventArgs e)
    {
        _vm.MoveSelectedUp();
    }

    private void butOrderDown_Click(object? sender, RoutedEventArgs e)
    {
        _vm.MoveSelectedDown();
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Properties panel
    // ──────────────────────────────────────────────────────────────────────────

    private void UpdatePropertiesPanel()
    {
        var c = _vm.SelectedComponent;
        if (c == null)
        {
            pnlPropsBase.IsVisible = false;
            lblNoSelection.IsVisible = true;
            return;
        }

        _vm.BeginLoadProps();
        pnlPropsBase.IsVisible = true;
        lblNoSelection.IsVisible = false;

        txtPropName.Text = c.Name;
        numPropX.Value = c.Position.X;
        numPropY.Value = c.Position.Y;
        chkPropVisible.IsChecked = c.Visible;

        secTextureNum.IsVisible = c is Button or Panel;
        secLabelProps.IsVisible = c is Label;
        secCheckBoxProps.IsVisible = c is CheckBox;
        secTextBoxProps.IsVisible = c is TextBox;
        secProgressBarProps.IsVisible = c is ProgressBar;
        secSlotGridProps.IsVisible = c is SlotGrid;
        secPictureProps.IsVisible = c is Picture;

        switch (c)
        {
            case Button btn: numPropTexture.Value = btn.TextureNum; break;
            case Panel pnl: numPropTexture.Value = pnl.TextureNum; break;
            case Label lbl:
                txtPropLblText.Text = lbl.Text;
                cmbPropLblAlignment.SelectedIndex = (int)lbl.Alignment;
                clrPropLabel.Color = Avalonia.Media.Color.FromRgb((byte)(lbl.Color >> 16), (byte)(lbl.Color >> 8), (byte)lbl.Color);
                numPropLblMaxWidth.Value = lbl.MaxWidth;
                break;
            case CheckBox cb:
                txtPropCbText.Text = cb.Text;
                chkPropCbChecked.IsChecked = cb.Checked;
                break;
            case TextBox tb:
                txtPropTbText.Text = tb.Text;
                numPropMaxChars.Value = tb.MaxCharacters;
                numPropTbWidth.Value = tb.Width;
                chkPropPassword.IsChecked = tb.Password;
                break;
            case ProgressBar pb:
                numPropPbSourceY.Value = pb.SourceY;
                numPropPbWidth.Value = pb.Width;
                numPropPbHeight.Value = pb.Height;
                break;
            case SlotGrid sg:
                numPropSgRows.Value = sg.Rows;
                numPropSgColumns.Value = sg.Columns;
                numPropSgSlotSize.Value = sg.SlotSize;
                numPropSgPadding.Value = sg.Padding;
                txtSgSlotCount.Text = sg.SlotCount.ToString();
                break;
            case Picture pic:
                numPropPicWidth.Value = pic.Width;
                numPropPicHeight.Value = pic.Height;
                break;
        }

        _vm.EndLoadProps();
    }

    // ─── Property write-back ──────────────────────────────────────────────────

    private void txtPropName_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEditProps) return;
        _vm.SelectedComponent!.Name = txtPropName.Text ?? string.Empty;
        // INotifyPropertyChanged on TreeItemVM.Header propagates the change to the TreeView automatically
        _vm.UpdateSelectedNodeHeader();
    }

    private void numPropX_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps) return;
        _vm.SelectedComponent!.Position = new Point((int)(e.NewValue ?? 0), _vm.SelectedComponent.Position.Y);
    }

    private void numPropY_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps) return;
        _vm.SelectedComponent!.Position = new Point(_vm.SelectedComponent.Position.X, (int)(e.NewValue ?? 0));
    }

    private void chkPropVisible_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (!_vm.CanEditProps) return;
        _vm.SelectedComponent!.Visible = chkPropVisible.IsChecked ?? false;
    }

    private void numPropTexture_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps) return;
        var v = (byte)(e.NewValue ?? 0);
        if (_vm.SelectedComponent is Button btn) btn.TextureNum = v;
        else if (_vm.SelectedComponent is Panel pnl) pnl.TextureNum = v;
    }

    private void txtPropLblText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not Label lbl) return;
        lbl.Text = txtPropLblText.Text ?? string.Empty;
    }

    private void cmbPropLblAlignment_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not Label lbl) return;
        lbl.Alignment = (TextAlign)(cmbPropLblAlignment.SelectedIndex >= 0 ? cmbPropLblAlignment.SelectedIndex : 0);
    }

    private void clrPropLabel_ColorChanged(object? sender, Avalonia.Controls.ColorChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not Label lbl) return;
        var c = e.NewColor;
        lbl.Color = (c.R << 16) | (c.G << 8) | c.B;
    }

    private void numPropLblMaxWidth_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not Label lbl) return;
        lbl.MaxWidth = (int)(e.NewValue ?? 0);
    }

    private void txtPropCbText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not CheckBox cb) return;
        cb.Text = txtPropCbText.Text ?? string.Empty;
    }

    private void chkPropCbChecked_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not CheckBox cb) return;
        cb.Checked = chkPropCbChecked.IsChecked ?? false;
    }

    private void txtPropTbText_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not TextBox tb) return;
        tb.Text = txtPropTbText.Text ?? string.Empty;
    }

    private void numPropMaxChars_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not TextBox tb) return;
        tb.MaxCharacters = (short)(e.NewValue ?? 0);
    }

    private void numPropTbWidth_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not TextBox tb) return;
        tb.Width = (short)(e.NewValue ?? 0);
    }

    private void chkPropPassword_IsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not TextBox tb) return;
        tb.Password = chkPropPassword.IsChecked ?? false;
    }

    private void numPropPbSourceY_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not ProgressBar pb) return;
        pb.SourceY = (int)(e.NewValue ?? 0);
    }

    private void numPropPbWidth_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not ProgressBar pb) return;
        pb.Width = (int)(e.NewValue ?? 0);
    }

    private void numPropPbHeight_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not ProgressBar pb) return;
        pb.Height = (int)(e.NewValue ?? 0);
    }

    private void numPropSgRows_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not SlotGrid sg) return;
        sg.Rows = (byte)(e.NewValue ?? 1);
        txtSgSlotCount.Text = sg.SlotCount.ToString();
    }

    private void numPropSgColumns_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not SlotGrid sg) return;
        sg.Columns = (byte)(e.NewValue ?? 1);
        txtSgSlotCount.Text = sg.SlotCount.ToString();
    }

    private void numPropSgSlotSize_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not SlotGrid sg) return;
        sg.SlotSize = (byte)(e.NewValue ?? 32);
    }

    private void numPropSgPadding_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not SlotGrid sg) return;
        sg.Padding = (byte)(e.NewValue ?? 4);
    }

    private void numPropPicWidth_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not Picture pic) return;
        pic.Width = (int)(e.NewValue ?? 0);
    }

    private void numPropPicHeight_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_vm.CanEditProps || _vm.SelectedComponent is not Picture pic) return;
        pic.Height = (int)(e.NewValue ?? 0);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Save / Cancel
    // ──────────────────────────────────────────────────────────────────────────

    private void butSaveAll_Click(object? sender, RoutedEventArgs e)
    {
        EditorInterfaceViewModel.Save();
        Close();
    }

    private void butCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
