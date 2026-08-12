using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Npcs;
using CryBits.Editors.Network;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Editors.Forms.Npcs;

internal sealed partial class NpcEditorViewModel(Npc model, DefinitionCatalog catalog, PackageSender sender)
    : ObservableObject
{
    private Npc Model { get; } = model;

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

    public string SayMsg
    {
        get => Model.SayMsg;
        set
        {
            Model.SayMsg = value;
            OnPropertyChanged();
        }
    }

    public short Texture
    {
        get => Model.Texture;
        set
        {
            Model.Texture = value;
            OnPropertyChanged();
        }
    }

    public byte Sight
    {
        get => Model.Sight;
        set
        {
            Model.Sight = value;
            OnPropertyChanged();
        }
    }

    public byte SpawnTime
    {
        get => Model.SpawnTime;
        set
        {
            Model.SpawnTime = value;
            OnPropertyChanged();
        }
    }

    public int Experience
    {
        get => Model.Experience;
        set
        {
            Model.Experience = value;
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

    public byte FleeHealth
    {
        get => Model.FleeHealth;
        set
        {
            Model.FleeHealth = value;
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    private void CreateNew()
    {
        var npc = new Npc();
        catalog.Npcs.Add(npc.Id, npc);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Remove()
    {
        catalog.Npcs.Remove(Model.Id);
        RequestRefreshList?.Invoke();
    }

    [RelayCommand]
    private void Save()
    {
        sender.WriteNpcs(catalog.Npcs);
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        sender.RequestNpcs();
        RequestClose?.Invoke();
    }
}
