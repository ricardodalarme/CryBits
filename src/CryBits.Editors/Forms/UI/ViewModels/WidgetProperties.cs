using Myra.Graphics2D.UI;
using PropertyModels.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal class WidgetProperties : ReactiveObject
{
    protected readonly Widget _widget;

    public WidgetProperties(Widget widget)
    {
        _widget = widget;
    }

    [Category("Identity")]
    [DisplayName("Type")]
    [ReadOnly(true)]
    public string WidgetType => _widget.GetType().Name;

    [Category("Identity")]
    [DisplayName("Id")]
    public string Id
    {
        get => _widget.Id ?? string.Empty;
        set
        {
            _widget.Id = value;
            RaisePropertyChanged(nameof(Id));
        }
    }

    [Category("Layout")]
    [Range(-9999, 9999)]
    public int Left
    {
        get => _widget.Left;
        set
        {
            _widget.Left = value;
            RaisePropertyChanged(nameof(Left));
        }
    }

    [Category("Layout")]
    [Range(-9999, 9999)]
    public int Top
    {
        get => _widget.Top;
        set
        {
            _widget.Top = value;
            RaisePropertyChanged(nameof(Top));
        }
    }

    [Category("Layout")]
    [Range(0, 9999)]
    public int? Width
    {
        get => _widget.Width;
        set
        {
            _widget.Width = value;
            RaisePropertyChanged(nameof(Width));
        }
    }

    [Category("Layout")]
    [Range(0, 9999)]
    public int? Height
    {
        get => _widget.Height;
        set
        {
            _widget.Height = value;
            RaisePropertyChanged(nameof(Height));
        }
    }

    [Category("Appearance")]
    public bool Visible
    {
        get => _widget.Visible;
        set
        {
            _widget.Visible = value;
            RaisePropertyChanged(nameof(Visible));
        }
    }

    [Category("Appearance")]
    public bool Enabled
    {
        get => _widget.Enabled;
        set
        {
            _widget.Enabled = value;
            RaisePropertyChanged(nameof(Enabled));
        }
    }
}
