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
    public Item Model { get; } = model;

    public event Action? RequestClose;
    public event Action? RequestRefreshList;
    public event Action<Item>? RequestSelectItem;

    public string Name
    {
        get => Model.Name;
        set { Model.Name = value; OnPropertyChanged(); }
    }

    public string Description
    {
        get => Model.Description;
        set { Model.Description = value; OnPropertyChanged(); }
    }

    public short Texture
    {
        get => Model.Texture;
        set { Model.Texture = value; OnPropertyChanged(); }
    }

    public bool Stackable
    {
        get => Model.Stackable;
        set { Model.Stackable = value; OnPropertyChanged(); }
    }

    public int RarityIndex
    {
        get => (int)Model.Rarity;
        set { Model.Rarity = (Rarity)value; OnPropertyChanged(); }
    }

    public int BindIndex
    {
        get => (int)Model.Bind;
        set { Model.Bind = (BindOn)value; OnPropertyChanged(); }
    }

    public short ReqLevel
    {
        get => Model.ReqLevel;
        set { Model.ReqLevel = value; OnPropertyChanged(); }
    }

    public short PotionHp
    {
        get => Model.PotionVital[(byte)Vital.Hp];
        set { Model.PotionVital[(byte)Vital.Hp] = value; OnPropertyChanged(); }
    }

    public short PotionMp
    {
        get => Model.PotionVital[(byte)Vital.Mp];
        set { Model.PotionVital[(byte)Vital.Mp] = value; OnPropertyChanged(); }
    }

    public int PotionExperience
    {
        get => Model.PotionExperience;
        set { Model.PotionExperience = value; OnPropertyChanged(); }
    }

    public short EquipStrength
    {
        get => Model.EquipAttribute[(byte)Attribute.Strength];
        set { Model.EquipAttribute[(byte)Attribute.Strength] = value; OnPropertyChanged(); }
    }

    public short EquipResistance
    {
        get => Model.EquipAttribute[(byte)Attribute.Resistance];
        set { Model.EquipAttribute[(byte)Attribute.Resistance] = value; OnPropertyChanged(); }
    }

    public short EquipIntelligence
    {
        get => Model.EquipAttribute[(byte)Attribute.Intelligence];
        set { Model.EquipAttribute[(byte)Attribute.Intelligence] = value; OnPropertyChanged(); }
    }

    public short EquipAgility
    {
        get => Model.EquipAttribute[(byte)Attribute.Agility];
        set { Model.EquipAttribute[(byte)Attribute.Agility] = value; OnPropertyChanged(); }
    }

    public short EquipVitality
    {
        get => Model.EquipAttribute[(byte)Attribute.Vitality];
        set { Model.EquipAttribute[(byte)Attribute.Vitality] = value; OnPropertyChanged(); }
    }

    public short WeaponDamage
    {
        get => Model.WeaponDamage;
        set { Model.WeaponDamage = value; OnPropertyChanged(); }
    }

    [RelayCommand]
    private void CreateNew()
    {
        var item = new Item();
        catalog.Items.Add(item.Id, item);
        RequestRefreshList?.Invoke();
        RequestSelectItem?.Invoke(item);
    }

    [RelayCommand]
    private void Remove()
    {
        catalog.Items.Remove(Model.Id);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Save()
    {
        PackageSender.Instance!.WriteItems();
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        PackageSender.Instance!.RequestItems();
        RequestClose?.Invoke();
    }
}
