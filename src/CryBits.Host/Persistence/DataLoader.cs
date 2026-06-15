using CryBits.Definitions.Catalog;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Persistence.Stores;
using CryBits.Host.Persistence.Repositories;
using System;
using System.Linq;

namespace CryBits.Host.Persistence;

internal sealed class DataLoader(
    SettingsRepository settingsRepository,
    FileContentStore contentStore,
    DefinitionCatalog catalog)
{
    public void LoadAll()
    {
        Console.WriteLine("Loading settings.");
        settingsRepository.Read();
        Console.WriteLine("Loading maps.");
        catalog.Maps = contentStore.LoadAll<Map>().ToDictionary(m => m.Id, m => m);
        Console.WriteLine("Loading classes.");
        catalog.Classes = contentStore.LoadAll<Class>().ToDictionary(c => c.Id, c => c);
        Console.WriteLine("Loading npcs.");
        catalog.Npcs = contentStore.LoadAll<Npc>().ToDictionary(n => n.Id, n => n);
        Console.WriteLine("Loading items.");
        catalog.Items = contentStore.LoadAll<Item>().ToDictionary(i => i.Id, i => i);
        Console.WriteLine("Loading shops.");
        catalog.Shops = contentStore.LoadAll<Shop>().ToDictionary(s => s.Id, s => s);
    }
}
