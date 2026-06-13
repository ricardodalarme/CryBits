using CryBits.Definitions.Catalog;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Persistence.Stores;
using CryBits.Host.Persistence.Repositories;
using System;
using System.IO;
using System.Linq;

namespace CryBits.Host.Persistence;

internal sealed class DataLoader(
    SettingsRepository settingsRepository,
    FileContentStore contentStore,
    DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static DataLoader Instance { get; } = new(
        SettingsRepository.Instance,
        new FileContentStore(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Data"))),
        DefinitionCatalog.Instance);

    public void LoadAll()
    {
        Console.WriteLine("Loading settings.");
        settingsRepository.Read();
        Console.WriteLine("Loading maps.");
        _catalog.Maps = contentStore.LoadAll<Map>().ToDictionary(m => m.Id, m => m);
        Console.WriteLine("Loading classes.");
        _catalog.Classes = contentStore.LoadAll<Class>().ToDictionary(c => c.Id, c => c);
        Console.WriteLine("Loading npcs.");
        _catalog.Npcs = contentStore.LoadAll<Npc>().ToDictionary(n => n.Id, n => n);
        Console.WriteLine("Loading items.");
        _catalog.Items = contentStore.LoadAll<Item>().ToDictionary(i => i.Id, i => i);
        Console.WriteLine("Loading shops.");
        _catalog.Shops = contentStore.LoadAll<Shop>().ToDictionary(s => s.Id, s => s);
    }
}
