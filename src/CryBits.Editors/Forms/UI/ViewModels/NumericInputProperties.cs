using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class NumericInputProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly NumericInputElement _input = (NumericInputElement)config;

    public override string GetTypeDiscriminator() => "NumericInput";

    [Category("Numeric Input")]
    [DisplayName("Default Value")]
    public double DefaultValue
    {
        get => _input.DefaultValue;
        set
        {
            _input.DefaultValue = value;
            if (_entity is NumericInput ni)
                ni.DefaultValue = (decimal)value;
            RaisePropertyChanged(nameof(DefaultValue));
        }
    }

    [Category("Numeric Input")]
    [DisplayName("Accepts Decimal")]
    public bool AcceptsDecimal
    {
        get => _input.AcceptsDecimal;
        set
        {
            _input.AcceptsDecimal = value;
            if (_entity is NumericInput ni)
                ni.AcceptsDecimal = value;
            RaisePropertyChanged(nameof(AcceptsDecimal));
        }
    }

    [Category("Numeric Input")]
    [DisplayName("Min Value")]
    public double MinValue
    {
        get => _input.MinValue;
        set
        {
            _input.MinValue = value;
            RaisePropertyChanged(nameof(MinValue));
        }
    }

    [Category("Numeric Input")]
    [DisplayName("Max Value")]
    public double MaxValue
    {
        get => _input.MaxValue;
        set
        {
            _input.MaxValue = value;
            RaisePropertyChanged(nameof(MaxValue));
        }
    }

    [Category("Numeric Input")]
    [DisplayName("Step Size")]
    [Range(0.01, 9999)]
    public double ButtonsStepSize
    {
        get => _input.ButtonsStepSize;
        set
        {
            _input.ButtonsStepSize = value;
            if (_entity is NumericInput ni)
                ni.ButtonsStepSize = (decimal)value;
            RaisePropertyChanged(nameof(ButtonsStepSize));
        }
    }
}
