using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Editors.Network;
using Attribute = CryBits.Definitions.Characters.Attribute;
using Class = CryBits.Definitions.Classes.Class;

namespace CryBits.Editors.Forms.Classes;

internal sealed partial class ClassEditorViewModel(Class model, DefinitionCatalog catalog, PackageSender sender)
    : ObservableObject
{
    public Class Model { get; } = model;

    public event Action? RequestClose;
    public event Action? RequestRefreshList;

    public string Name
    {
        get => Model.Name;
        set
        {
            Model.Name = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => Model.Description;
        set
        {
            Model.Description = value;
            OnPropertyChanged();
        }
    }

    public short Hp
    {
        get => Model.Vital[(byte)Vital.Hp];
        set
        {
            Model.Vital[(byte)Vital.Hp] = value;
            OnPropertyChanged();
        }
    }

    public short Mp
    {
        get => Model.Vital[(byte)Vital.Mp];
        set
        {
            Model.Vital[(byte)Vital.Mp] = value;
            OnPropertyChanged();
        }
    }

    public short Strength
    {
        get => Model.Attribute[(byte)Attribute.Strength];
        set
        {
            Model.Attribute[(byte)Attribute.Strength] = value;
            OnPropertyChanged();
        }
    }

    public short Resistance
    {
        get => Model.Attribute[(byte)Attribute.Resistance];
        set
        {
            Model.Attribute[(byte)Attribute.Resistance] = value;
            OnPropertyChanged();
        }
    }

    public short Intelligence
    {
        get => Model.Attribute[(byte)Attribute.Intelligence];
        set
        {
            Model.Attribute[(byte)Attribute.Intelligence] = value;
            OnPropertyChanged();
        }
    }

    public short Agility
    {
        get => Model.Attribute[(byte)Attribute.Agility];
        set
        {
            Model.Attribute[(byte)Attribute.Agility] = value;
            OnPropertyChanged();
        }
    }

    public short Vitality
    {
        get => Model.Attribute[(byte)Attribute.Vitality];
        set
        {
            Model.Attribute[(byte)Attribute.Vitality] = value;
            OnPropertyChanged();
        }
    }

    public int SpawnX
    {
        get => Model.SpawnX;
        set
        {
            Model.SpawnX = value;
            OnPropertyChanged();
        }
    }

    public int SpawnY
    {
        get => Model.SpawnY;
        set
        {
            Model.SpawnY = value;
            OnPropertyChanged();
        }
    }

    public int SpawnDirectionIndex
    {
        get => Model.SpawnDirection;
        set
        {
            Model.SpawnDirection = (byte)value;
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    private void CreateNew()
    {
        var cls = new Class();
        catalog.Classes.Add(cls.Id, cls);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Remove()
    {
        if (catalog.Classes.Count == 1) return;
        catalog.Classes.Remove(Model.Id);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Save()
    {
        sender.WriteClasses(catalog.Classes);
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        sender.RequestClasses();
        RequestClose?.Invoke();
    }
}
