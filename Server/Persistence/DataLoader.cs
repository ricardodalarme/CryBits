using System;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Server.Persistence.Repositories;

namespace CryBits.Server.Persistence;

/// <summary>Orchestrates loading all game data from disk on server startup.</summary>
internal static class DataLoader
{
    public static void LoadAll()
    {
        Console.WriteLine("Loading settings.");
        SettingsRepository.Read();
        Console.WriteLine("Loading maps.");
        Map.List = MapRepository.Read();
        Console.WriteLine("Loading classes.");
        Class.List = ClassRepository.Read();
        Console.WriteLine("Loading npcs.");
        Npc.List = NpcRepository.Read();
        Console.WriteLine("Loading items.");
        Item.List = ItemRepository.Read();
        Console.WriteLine("Loading shops.");
        Shop.List = ShopRepository.Read();
    }
}
