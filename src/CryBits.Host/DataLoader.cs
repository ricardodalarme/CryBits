using CryBits.Definitions.Catalog;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Persistence.Repositories;

namespace CryBits.Host;

public sealed class DataLoader(
    ContentRepository contentRepository,
    DefinitionCatalog catalog)
{
    public void LoadAll()
    {
        Console.WriteLine("Loading maps.");
        catalog.Maps = contentRepository.LoadAll<Map>().ToDictionary(m => m.Id, m => m);
        Console.WriteLine("Loading classes.");
        catalog.Classes = contentRepository.LoadAll<Class>().ToDictionary(c => c.Id, c => c);
        Console.WriteLine("Loading npcs.");
        catalog.Npcs = contentRepository.LoadAll<Npc>().ToDictionary(n => n.Id, n => n);
        Console.WriteLine("Loading items.");
        catalog.Items = contentRepository.LoadAll<Item>().ToDictionary(i => i.Id, i => i);
        Console.WriteLine("Loading shops.");
        catalog.Shops = contentRepository.LoadAll<Shop>().ToDictionary(s => s.Id, s => s);
    }
}
