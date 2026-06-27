using CryBits.Definitions.Characters;
using CryBits.Definitions.Npcs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Editors.Forms.Npcs;

internal sealed class NpcEditorViewModel(Npc model) : INotifyPropertyChanged
{
    private readonly Npc _model = model;
    public Npc Model => _model;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Notify([CallerMemberName] string? n = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

    public string Name
    {
        get => _model.Name;
        set { _model.Name = value; Notify(); }
    }

    public string SayMsg
    {
        get => _model.SayMsg;
        set { _model.SayMsg = value; Notify(); }
    }

    public short Texture
    {
        get => _model.Texture;
        set { _model.Texture = value; Notify(); }
    }

    public byte Sight
    {
        get => _model.Sight;
        set { _model.Sight = value; Notify(); }
    }

    public byte SpawnTime
    {
        get => _model.SpawnTime;
        set { _model.SpawnTime = value; Notify(); }
    }

    public int Experience
    {
        get => _model.Experience;
        set { _model.Experience = value; Notify(); }
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

    public byte FleeHealth
    {
        get => _model.FleeHealth;
        set { _model.FleeHealth = value; Notify(); }
    }
}
