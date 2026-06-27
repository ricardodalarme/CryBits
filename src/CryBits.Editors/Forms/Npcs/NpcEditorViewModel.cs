using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Npcs;
using CryBits.Editors.Network;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Editors.Forms.Npcs;

internal sealed partial class NpcEditorViewModel(Npc model, DefinitionCatalog catalog) : ObservableObject
{
    private readonly Npc _model = model;
    private readonly DefinitionCatalog _catalog = catalog;

    public Npc Model => _model;
    public event Action? RequestClose;
    public event Action? RequestRefreshList;

    public string Name
    {
        get => _model.Name;
        set { _model.Name = value; OnPropertyChanged(); }
    }

    public string SayMsg
    {
        get => _model.SayMsg;
        set { _model.SayMsg = value; OnPropertyChanged(); }
    }

    public short Texture
    {
        get => _model.Texture;
        set { _model.Texture = value; OnPropertyChanged(); }
    }

    public byte Sight
    {
        get => _model.Sight;
        set { _model.Sight = value; OnPropertyChanged(); }
    }

    public byte SpawnTime
    {
        get => _model.SpawnTime;
        set { _model.SpawnTime = value; OnPropertyChanged(); }
    }

    public int Experience
    {
        get => _model.Experience;
        set { _model.Experience = value; OnPropertyChanged(); }
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

    public byte FleeHealth
    {
        get => _model.FleeHealth;
        set { _model.FleeHealth = value; OnPropertyChanged(); }
    }

    [RelayCommand]
    private void CreateNew()
    {
        var npc = new Npc();
        _catalog.Npcs.Add(npc.Id, npc);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Remove()
    {
        _catalog.Npcs.Remove(_model.Id);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Save()
    {
        PackageSender.Instance.WriteNpcs();
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        PackageSender.Instance.RequestNpcs();
        RequestClose?.Invoke();
    }
}
