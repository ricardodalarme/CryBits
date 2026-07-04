using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class SliderProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly SliderElement _slider = (SliderElement)config;

    public override string GetTypeDiscriminator() => "Slider";

    [Category("Range")]
    [DisplayName("Min Value")]
    [Range(0, 99999)]
    public int MinValue
    {
        get => _slider.MinValue;
        set
        {
            _slider.MinValue = value;
            if (_entity is Slider s)
                s.MinValue = value;
            RaisePropertyChanged(nameof(MinValue));
        }
    }

    [Category("Range")]
    [DisplayName("Max Value")]
    [Range(1, 99999)]
    public int MaxValue
    {
        get => _slider.MaxValue;
        set
        {
            _slider.MaxValue = value;
            if (_entity is Slider s)
                s.MaxValue = value;
            RaisePropertyChanged(nameof(MaxValue));
        }
    }

    [Category("Range")]
    [DisplayName("Current Value")]
    [Range(0, 99999)]
    public int Value
    {
        get => _slider.Value;
        set
        {
            _slider.Value = value;
            if (_entity is Slider s)
                s.ValueSafe = value;
            RaisePropertyChanged(nameof(Value));
        }
    }

    [Category("Range")]
    [DisplayName("Steps Count")]
    [Range(0, 9999)]
    public int StepsCount
    {
        get => _slider.StepsCount;
        set
        {
            _slider.StepsCount = value;
            if (_entity is Slider s)
                s.StepsCount = (uint)value;
            RaisePropertyChanged(nameof(StepsCount));
        }
    }

    [Category("Range")]
    public string Orientation
    {
        get => _slider.Orientation;
        set
        {
            _slider.Orientation = value;
            RaisePropertyChanged(nameof(Orientation));
        }
    }

    [Category("Range")]
    [DisplayName("Mouse Wheel Step")]
    [Range(0, 999)]
    public int MouseWheelStep
    {
        get => _slider.MouseWheelStep;
        set
        {
            _slider.MouseWheelStep = value;
            if (_entity is Slider s)
                s.MouseWheelStep = value;
            RaisePropertyChanged(nameof(MouseWheelStep));
        }
    }

    [Category("Range")]
    [DisplayName("Flipped Direction")]
    public bool FlippedDirection
    {
        get => _slider.FlippedDirection;
        set
        {
            _slider.FlippedDirection = value;
            if (_entity is Slider s)
                s.FlippedDirection = value;
            RaisePropertyChanged(nameof(FlippedDirection));
        }
    }
}
