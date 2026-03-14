using System;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
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

    public void LoadAll()
    {
        Console.WriteLine("Loading settings.");
        settingsRepository.Read();
        Console.WriteLine("Loading maps.");
        Map.List = mapRepository.Read();
        Console.WriteLine("Loading classes.");
        Class.List = classRepository.Read();
        Console.WriteLine("Loading npcs.");
        Npc.List = npcRepository.Read();
        Console.WriteLine("Loading items.");
        Item.List = itemRepository.Read();
        Console.WriteLine("Loading shops.");
        Shop.List = shopRepository.Read();
    }
}
