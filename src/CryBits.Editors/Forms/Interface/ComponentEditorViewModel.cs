using CommunityToolkit.Mvvm.ComponentModel;
using CryBits.Definitions.Common;
using System.Drawing;
using Button = CryBits.Client.Framework.Interfacily.Components.Button;
using CheckBox = CryBits.Client.Framework.Interfacily.Components.CheckBox;
using Component = CryBits.Client.Framework.Interfacily.Components.Component;
using Label = CryBits.Client.Framework.Interfacily.Components.Label;
using Panel = CryBits.Client.Framework.Interfacily.Components.Panel;
using Picture = CryBits.Client.Framework.Interfacily.Components.Picture;
using ProgressBar = CryBits.Client.Framework.Interfacily.Components.ProgressBar;
using SlotGrid = CryBits.Client.Framework.Interfacily.Components.SlotGrid;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Editors.Forms.Interface;

internal sealed class ComponentEditorViewModel(Component model) : ObservableObject
{
    private readonly Component _model = model;
    public Component Component => _model;

    // ── Common properties ──────────────────────────────────────────────

    public string Name
    {
        get => _model.Name;
        set { _model.Name = value; OnPropertyChanged(); }
    }

    public int X
    {
        get => _model.Position.X;
        set { _model.Position = new Point(value, _model.Position.Y); OnPropertyChanged(); }
    }

    public int Y
    {
        get => _model.Position.Y;
        set { _model.Position = new Point(_model.Position.X, value); OnPropertyChanged(); }
    }

    public bool Visible
    {
        get => _model.Visible;
        set { _model.Visible = value; OnPropertyChanged(); }
    }

    // ── Type identification ────────────────────────────────────────────

    public bool IsButton => _model is Button;
    public bool IsPanel => _model is Panel;
    public bool IsLabel => _model is Label;
    public bool IsCheckBox => _model is CheckBox;
    public bool IsTextBox => _model is TextBox;
    public bool IsProgressBar => _model is ProgressBar;
    public bool IsSlotGrid => _model is SlotGrid;
    public bool IsPicture => _model is Picture;
    public bool HasTextureNum => _model is Button or Panel;

    // ── TextureNum (Button / Panel) ────────────────────────────────────

    public byte TextureNum
    {
        get => _model switch { Button b => b.TextureNum, Panel p => p.TextureNum, _ => (byte)0 };
        set
        {
            switch (_model)
            {
                case Button b: b.TextureNum = value; break;
                case Panel p: p.TextureNum = value; break;
                default: return;
            }
            OnPropertyChanged();
        }
    }

    // ── Label ──────────────────────────────────────────────────────────

    public string LabelText
    {
        get => _model is Label lbl ? lbl.Text : string.Empty;
        set { if (_model is Label lbl) { lbl.Text = value; OnPropertyChanged(); } }
    }

    public int LabelAlignment
    {
        get => _model is Label lbl ? (int)lbl.Alignment : 0;
        set { if (_model is Label lbl) { lbl.Alignment = (TextAlign)value; OnPropertyChanged(); } }
    }

    public int LabelMaxWidth
    {
        get => _model is Label lbl ? lbl.MaxWidth : 0;
        set { if (_model is Label lbl) { lbl.MaxWidth = value; OnPropertyChanged(); } }
    }

    // ── CheckBox ───────────────────────────────────────────────────────

    public string CbText
    {
        get => _model is CheckBox cb ? cb.Text : string.Empty;
        set { if (_model is CheckBox cb) { cb.Text = value; OnPropertyChanged(); } }
    }

    public bool CbChecked
    {
        get => _model is CheckBox cb && cb.Checked;
        set { if (_model is CheckBox cb) { cb.Checked = value; OnPropertyChanged(); } }
    }

    // ── TextBox ───────────────────────────────────────────────────────

    public string TbText
    {
        get => _model is TextBox tb ? tb.Text : string.Empty;
        set { if (_model is TextBox tb) { tb.Text = value; OnPropertyChanged(); } }
    }

    public short TbMaxChars
    {
        get => _model is TextBox tb ? tb.MaxCharacters : (short)0;
        set { if (_model is TextBox tb) { tb.MaxCharacters = value; OnPropertyChanged(); } }
    }

    public short TbWidth
    {
        get => _model is TextBox tb ? tb.Width : (short)0;
        set { if (_model is TextBox tb) { tb.Width = value; OnPropertyChanged(); } }
    }

    public bool TbPassword
    {
        get => _model is TextBox tb && tb.Password;
        set { if (_model is TextBox tb) { tb.Password = value; OnPropertyChanged(); } }
    }

    // ── ProgressBar ────────────────────────────────────────────────────

    public int PbSourceY
    {
        get => _model is ProgressBar pb ? pb.SourceY : 0;
        set { if (_model is ProgressBar pb) { pb.SourceY = value; OnPropertyChanged(); } }
    }

    public int PbWidth
    {
        get => _model is ProgressBar pb ? pb.Width : 0;
        set { if (_model is ProgressBar pb) { pb.Width = value; OnPropertyChanged(); } }
    }

    public int PbHeight
    {
        get => _model is ProgressBar pb ? pb.Height : 0;
        set { if (_model is ProgressBar pb) { pb.Height = value; OnPropertyChanged(); } }
    }

    // ── SlotGrid ───────────────────────────────────────────────────────

    public byte SgRows
    {
        get => _model is SlotGrid sg ? sg.Rows : (byte)1;
        set
        {
            if (_model is SlotGrid sg)
            {
                sg.Rows = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SgSlotCount));
            }
        }
    }

    public byte SgColumns
    {
        get => _model is SlotGrid sg ? sg.Columns : (byte)1;
        set
        {
            if (_model is SlotGrid sg)
            {
                sg.Columns = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SgSlotCount));
            }
        }
    }

    public byte SgSlotSize
    {
        get => _model is SlotGrid sg ? sg.SlotSize : (byte)32;
        set { if (_model is SlotGrid sg) { sg.SlotSize = value; OnPropertyChanged(); } }
    }

    public byte SgPadding
    {
        get => _model is SlotGrid sg ? sg.Padding : (byte)4;
        set { if (_model is SlotGrid sg) { sg.Padding = value; OnPropertyChanged(); } }
    }

    public int SgSlotCount => _model is SlotGrid sg ? sg.SlotCount : 0;

    // ── Picture ────────────────────────────────────────────────────────

    public int PicWidth
    {
        get => _model is Picture pic ? pic.Width : 0;
        set { if (_model is Picture pic) { pic.Width = value; OnPropertyChanged(); } }
    }

    public int PicHeight
    {
        get => _model is Picture pic ? pic.Height : 0;
        set { if (_model is Picture pic) { pic.Height = value; OnPropertyChanged(); } }
    }
}
