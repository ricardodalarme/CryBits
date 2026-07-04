using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class SlotGridProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly SlotGridElement _grid = (SlotGridElement)config;
    private readonly CryBits.Client.Framework.UI.Entities.SlotGrid _gridEntity = (CryBits.Client.Framework.UI.Entities.SlotGrid)entity;

    public override string GetTypeDiscriminator() => "SlotGrid";

    [Browsable(false)]
    public new int Width => _grid.Columns * _grid.SlotSize + _grid.Spacing * (_grid.Columns - 1);

    [Browsable(false)]
    public new int Height => _grid.Rows * _grid.SlotSize + _grid.Spacing * (_grid.Rows - 1);

    [Category("Slot Grid")]
    [Range(1, 99)]
    public int Columns
    {
        get => _grid.Columns;
        set
        {
            _grid.Columns = value;
            _gridEntity.Columns = value;
            RaisePropertyChanged(nameof(Columns));
            RaisePropertyChanged(nameof(Width));
        }
    }

    [Category("Slot Grid")]
    [Range(1, 99)]
    public int Rows
    {
        get => _grid.Rows;
        set
        {
            _grid.Rows = value;
            _gridEntity.Rows = value;
            RaisePropertyChanged(nameof(Rows));
            RaisePropertyChanged(nameof(Height));
        }
    }

    [Category("Slot Grid")]
    [DisplayName("Slot Size")]
    [Range(8, 128)]
    public int SlotSize
    {
        get => _grid.SlotSize;
        set
        {
            _grid.SlotSize = value;
            _gridEntity.SlotSize = value;
            RaisePropertyChanged(nameof(SlotSize));
            RaisePropertyChanged(nameof(Width));
            RaisePropertyChanged(nameof(Height));
        }
    }

    [Category("Slot Grid")]
    [DisplayName("Spacing")]
    [Range(0, 32)]
    public int Spacing
    {
        get => _grid.Spacing;
        set
        {
            _grid.Spacing = value;
            _gridEntity.Spacing = value;
            RaisePropertyChanged(nameof(Spacing));
            RaisePropertyChanged(nameof(Width));
            RaisePropertyChanged(nameof(Height));
        }
    }
}
