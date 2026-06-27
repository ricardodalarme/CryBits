using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Items;
using CryBits.Editors.Network;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Editors.Forms.Items;

internal sealed partial class ItemEditorViewModel(Item model, DefinitionCatalog catalog) : ObservableObject
{
    private readonly Item _model = model;
    private readonly DefinitionCatalog _catalog = catalog;

    public Item Model => _model;
    public event Action? RequestClose;
    public event Action? RequestRefreshList;
    public event Action<Item>? RequestSelectItem;

    public string Name
    {
        get => _model.Name;
        set { _model.Name = value; OnPropertyChanged(); }
    }

    public string Description
    {
        get => _model.Description;
        set { _model.Description = value; OnPropertyChanged(); }
    }

    public short Texture
    {
        get => _model.Texture;
        set { _model.Texture = value; OnPropertyChanged(); }
    }

    public bool Stackable
    {
        get => _model.Stackable;
        set { _model.Stackable = value; OnPropertyChanged(); }
    }

    public int RarityIndex
    {
        get => (int)_model.Rarity;
        set { _model.Rarity = (Rarity)value; OnPropertyChanged(); }
    }

    public int BindIndex
    {
        get => (int)_model.Bind;
        set { _model.Bind = (BindOn)value; OnPropertyChanged(); }
    }

    public short ReqLevel
    {
        get => _model.ReqLevel;
        set { _model.ReqLevel = value; OnPropertyChanged(); }
    }

    public short PotionHp
    {
        get => _model.PotionVital[(byte)Vital.Hp];
        set { _model.PotionVital[(byte)Vital.Hp] = value; OnPropertyChanged(); }
    }

    public short PotionMp
    {
        get => _model.PotionVital[(byte)Vital.Mp];
        set { _model.PotionVital[(byte)Vital.Mp] = value; OnPropertyChanged(); }
    }

    public int PotionExperience
    {
        get => _model.PotionExperience;
        set { _model.PotionExperience = value; OnPropertyChanged(); }
    }

    public short EquipStrength
    {
        get => _model.EquipAttribute[(byte)Attribute.Strength];
        set { _model.EquipAttribute[(byte)Attribute.Strength] = value; OnPropertyChanged(); }
    }

    public short EquipResistance
    {
        get => _model.EquipAttribute[(byte)Attribute.Resistance];
        set { _model.EquipAttribute[(byte)Attribute.Resistance] = value; OnPropertyChanged(); }
    }

    public short EquipIntelligence
    {
        get => _model.EquipAttribute[(byte)Attribute.Intelligence];
        set { _model.EquipAttribute[(byte)Attribute.Intelligence] = value; OnPropertyChanged(); }
    }

    public short EquipAgility
    {
        get => _model.EquipAttribute[(byte)Attribute.Agility];
        set { _model.EquipAttribute[(byte)Attribute.Agility] = value; OnPropertyChanged(); }
    }

    public short EquipVitality
    {
        get => _model.EquipAttribute[(byte)Attribute.Vitality];
        set { _model.EquipAttribute[(byte)Attribute.Vitality] = value; OnPropertyChanged(); }
    }

    public short WeaponDamage
    {
        get => _model.WeaponDamage;
        set { _model.WeaponDamage = value; OnPropertyChanged(); }
    }

    [RelayCommand]
    private void CreateNew()
    {
        var item = new Item();
        _catalog.Items.Add(item.Id, item);
        RequestRefreshList?.Invoke();
        RequestSelectItem?.Invoke(item);
    }

    [RelayCommand]
    private void Remove()
    {
        _catalog.Items.Remove(_model.Id);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Save()
    {
        PackageSender.Instance.WriteItems();
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        PackageSender.Instance.RequestItems();
        RequestClose?.Invoke();
    }
}
