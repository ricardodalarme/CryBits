using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Editors.Network;
using Attribute = CryBits.Definitions.Characters.Attribute;
using Class = CryBits.Definitions.Classes.Class;

namespace CryBits.Editors.Forms.Classes;

internal sealed partial class ClassEditorViewModel(Class model, DefinitionCatalog catalog) : ObservableObject
{
    private readonly Class _model = model;
    private readonly DefinitionCatalog _catalog = catalog;

    public Class Model => _model;
    public event Action? RequestClose;
    public event Action? RequestRefreshList;

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

    public short Hp
    {
        get => _model.Vital[(byte)Vital.Hp];
        set { _model.Vital[(byte)Vital.Hp] = value; OnPropertyChanged(); }
    }

    public short Mp
    {
        get => _model.Vital[(byte)Vital.Mp];
        set { _model.Vital[(byte)Vital.Mp] = value; OnPropertyChanged(); }
    }

    public short Strength
    {
        get => _model.Attribute[(byte)Attribute.Strength];
        set { _model.Attribute[(byte)Attribute.Strength] = value; OnPropertyChanged(); }
    }

    public short Resistance
    {
        get => _model.Attribute[(byte)Attribute.Resistance];
        set { _model.Attribute[(byte)Attribute.Resistance] = value; OnPropertyChanged(); }
    }

    public short Intelligence
    {
        get => _model.Attribute[(byte)Attribute.Intelligence];
        set { _model.Attribute[(byte)Attribute.Intelligence] = value; OnPropertyChanged(); }
    }

    public short Agility
    {
        get => _model.Attribute[(byte)Attribute.Agility];
        set { _model.Attribute[(byte)Attribute.Agility] = value; OnPropertyChanged(); }
    }

    public short Vitality
    {
        get => _model.Attribute[(byte)Attribute.Vitality];
        set { _model.Attribute[(byte)Attribute.Vitality] = value; OnPropertyChanged(); }
    }

    public int SpawnX
    {
        get => _model.SpawnX;
        set { _model.SpawnX = value; OnPropertyChanged(); }
    }

    public int SpawnY
    {
        get => _model.SpawnY;
        set { _model.SpawnY = value; OnPropertyChanged(); }
    }

    public int SpawnDirectionIndex
    {
        get => _model.SpawnDirection;
        set { _model.SpawnDirection = (byte)value; OnPropertyChanged(); }
    }

    [RelayCommand]
    private void CreateNew()
    {
        var cls = new Class();
        _catalog.Classes.Add(cls.Id, cls);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Remove()
    {
        if (_catalog.Classes.Count == 1) return;
        _catalog.Classes.Remove(_model.Id);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Save()
    {
        PackageSender.Instance!.WriteClasses();
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        PackageSender.Instance!.RequestClasses();
        RequestClose?.Invoke();
    }
}
