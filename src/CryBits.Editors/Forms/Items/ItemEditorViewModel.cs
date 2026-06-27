using CryBits.Definitions.Characters;
using CryBits.Definitions.Items;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Editors.Forms.Items;

internal sealed class ItemEditorViewModel(Item model) : INotifyPropertyChanged
{
    private readonly Item _model = model;
    public Item Model => _model;

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

    public short Texture
    {
        get => _model.Texture;
        set { _model.Texture = value; Notify(); }
    }

    public bool Stackable
    {
        get => _model.Stackable;
        set { _model.Stackable = value; Notify(); }
    }

    public int RarityIndex
    {
        get => (int)_model.Rarity;
        set { _model.Rarity = (Rarity)value; Notify(); }
    }

    public int BindIndex
    {
        get => (int)_model.Bind;
        set { _model.Bind = (BindOn)value; Notify(); }
    }

    public short ReqLevel
    {
        get => _model.ReqLevel;
        set { _model.ReqLevel = value; Notify(); }
    }

    public short PotionHp
    {
        get => _model.PotionVital[(byte)Vital.Hp];
        set { _model.PotionVital[(byte)Vital.Hp] = value; Notify(); }
    }

    public short PotionMp
    {
        get => _model.PotionVital[(byte)Vital.Mp];
        set { _model.PotionVital[(byte)Vital.Mp] = value; Notify(); }
    }

    public int PotionExperience
    {
        get => _model.PotionExperience;
        set { _model.PotionExperience = value; Notify(); }
    }

    public short EquipStrength
    {
        get => _model.EquipAttribute[(byte)Attribute.Strength];
        set { _model.EquipAttribute[(byte)Attribute.Strength] = value; Notify(); }
    }

    public short EquipResistance
    {
        get => _model.EquipAttribute[(byte)Attribute.Resistance];
        set { _model.EquipAttribute[(byte)Attribute.Resistance] = value; Notify(); }
    }

    public short EquipIntelligence
    {
        get => _model.EquipAttribute[(byte)Attribute.Intelligence];
        set { _model.EquipAttribute[(byte)Attribute.Intelligence] = value; Notify(); }
    }

    public short EquipAgility
    {
        get => _model.EquipAttribute[(byte)Attribute.Agility];
        set { _model.EquipAttribute[(byte)Attribute.Agility] = value; Notify(); }
    }

    public short EquipVitality
    {
        get => _model.EquipAttribute[(byte)Attribute.Vitality];
        set { _model.EquipAttribute[(byte)Attribute.Vitality] = value; Notify(); }
    }

    public short WeaponDamage
    {
        get => _model.WeaponDamage;
        set { _model.WeaponDamage = value; Notify(); }
    }
}
