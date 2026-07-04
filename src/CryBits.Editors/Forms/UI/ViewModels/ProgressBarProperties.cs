using CryBits.Client.Framework.Persistence.Dtos;
using Iguina.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CryBits.Editors.Forms.UI.ViewModels;

internal sealed class ProgressBarProperties(Element config, Entity entity) : UIElementProperties(config, entity)
{
    private readonly ProgressBarElement _bar = (ProgressBarElement)config;

    public override string GetTypeDiscriminator() => "ProgressBar";

    [Category("Range")]
    [DisplayName("Min Value")]
    [Range(0, 99999)]
    public int MinValue
    {
        get => _bar.MinValue;
        set
        {
            _bar.MinValue = value;
            if (_entity is Slider slider)
                slider.MinValue = value;
            RaisePropertyChanged(nameof(MinValue));
        }
    }

    [Category("Range")]
    [DisplayName("Max Value")]
    [Range(1, 99999)]
    public int MaxValue
    {
        get => _bar.MaxValue;
        set
        {
            _bar.MaxValue = value;
            if (_entity is Slider slider)
                slider.MaxValue = value;
            RaisePropertyChanged(nameof(MaxValue));
        }
    }

    [Category("Range")]
    [DisplayName("Current Value")]
    [Range(0, 99999)]
    public int Value
    {
        get => _bar.Value;
        set
        {
            _bar.Value = value;
            if (_entity is Slider slider)
                slider.ValueSafe = value;
            RaisePropertyChanged(nameof(Value));
        }
    }
}
