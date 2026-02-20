using System;
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
        MapRepository.Read();
        Console.WriteLine("Loading classes.");
        ClassRepository.Read();
        Console.WriteLine("Loading npcs.");
        NpcRepository.Read();
        Console.WriteLine("Loading items.");
        ItemRepository.Read();
        Console.WriteLine("Loading shops.");
        ShopRepository.Read();
    }
}
