using System;
using CryBits.Server.Persistence.Repositories;

namespace CryBits.Server.Persistence;

/// <summary>Orchestrates loading all game data from disk on server startup.</summary>
internal sealed class DataLoader(
    SettingsRepository settingsRepository,
    MapRepository mapRepository,
    ClassRepository classRepository,
    NpcRepository npcRepository,
    ItemRepository itemRepository,
    ShopRepository shopRepository)
{
    public static DataLoader Instance { get; } = new(
        SettingsRepository.Instance,
        MapRepository.Instance,
        ClassRepository.Instance,
        NpcRepository.Instance,
        ItemRepository.Instance,
        ShopRepository.Instance);

    private readonly SettingsRepository _settingsRepository = settingsRepository;
    private readonly MapRepository _mapRepository = mapRepository;
    private readonly ClassRepository _classRepository = classRepository;
    private readonly NpcRepository _npcRepository = npcRepository;
    private readonly ItemRepository _itemRepository = itemRepository;
    private readonly ShopRepository _shopRepository = shopRepository;

    public void LoadAll()
    {
        Console.WriteLine("Loading settings.");
        _settingsRepository.Read();
        Console.WriteLine("Loading maps.");
        _mapRepository.Read();
        Console.WriteLine("Loading classes.");
        _classRepository.Read();
        Console.WriteLine("Loading npcs.");
        _npcRepository.Read();
        Console.WriteLine("Loading items.");
        _itemRepository.Read();
        Console.WriteLine("Loading shops.");
        _shopRepository.Read();
    }
}
