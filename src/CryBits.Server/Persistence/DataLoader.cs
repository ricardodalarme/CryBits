using CryBits.Definitions.Catalog;
using CryBits.Server.Persistence.Repositories;
using System;

namespace CryBits.Server.Persistence;

/// <summary>Orchestrates loading all game data from disk on server startup.</summary>
internal sealed class DataLoader(
    SettingsRepository settingsRepository,
    MapRepository mapRepository,
    ClassRepository classRepository,
    NpcRepository npcRepository,
    ItemRepository itemRepository,
    ShopRepository shopRepository,
    DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static DataLoader Instance { get; } = new(
        SettingsRepository.Instance,
        MapRepository.Instance,
        ClassRepository.Instance,
        NpcRepository.Instance,
        ItemRepository.Instance,
        ShopRepository.Instance,
        DefinitionCatalog.Instance);

    public void LoadAll()
    {
        Console.WriteLine("Loading settings.");
        settingsRepository.Read();
        Console.WriteLine("Loading maps.");
        _catalog.Maps = mapRepository.Read();
        Console.WriteLine("Loading classes.");
        _catalog.Classes = classRepository.Read();
        Console.WriteLine("Loading npcs.");
        _catalog.Npcs = npcRepository.Read();
        Console.WriteLine("Loading items.");
        _catalog.Items = itemRepository.Read();
        Console.WriteLine("Loading shops.");
        _catalog.Shops = shopRepository.Read();
    }
}
