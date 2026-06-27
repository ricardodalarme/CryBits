using CryBits.Definitions.Characters;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Attribute = CryBits.Definitions.Characters.Attribute;
using Class = CryBits.Definitions.Classes.Class;

namespace CryBits.Editors.Forms.Classes;

internal sealed class ClassEditorViewModel(Class model) : INotifyPropertyChanged
{
    private readonly Class _model = model;

    public Class Model => _model;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Notify([CallerMemberName] string? n = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    public string Name
    {
        get => _model.Name;
        set { _model.Name = value; Notify(); }
    }

    public string Description
    {
        get => _model.Description;
        set { _model.Description = value; Notify(); }
    }

    public short Hp
    {
        get => _model.Vital[(byte)Vital.Hp];
        set { _model.Vital[(byte)Vital.Hp] = value; Notify(); }
    }

    public short Mp
    {
        get => _model.Vital[(byte)Vital.Mp];
        set { _model.Vital[(byte)Vital.Mp] = value; Notify(); }
    }

    public short Strength
    {
        get => _model.Attribute[(byte)Attribute.Strength];
        set { _model.Attribute[(byte)Attribute.Strength] = value; Notify(); }
    }

    public short Resistance
    {
        get => _model.Attribute[(byte)Attribute.Resistance];
        set { _model.Attribute[(byte)Attribute.Resistance] = value; Notify(); }
    }

    public short Intelligence
    {
        get => _model.Attribute[(byte)Attribute.Intelligence];
        set { _model.Attribute[(byte)Attribute.Intelligence] = value; Notify(); }
    }

    public short Agility
    {
        get => _model.Attribute[(byte)Attribute.Agility];
        set { _model.Attribute[(byte)Attribute.Agility] = value; Notify(); }
    }

    public short Vitality
    {
        get => _model.Attribute[(byte)Attribute.Vitality];
        set { _model.Attribute[(byte)Attribute.Vitality] = value; Notify(); }
    }

    public int SpawnX
    {
        get => _model.SpawnX;
        set { _model.SpawnX = value; Notify(); }
    }

    public int SpawnY
    {
        get => _model.SpawnY;
        set { _model.SpawnY = value; Notify(); }
    }

    public int SpawnDirectionIndex
    {
        get => _model.SpawnDirection;
        set { _model.SpawnDirection = (byte)value; Notify(); }
    }
}
