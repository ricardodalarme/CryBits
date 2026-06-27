using CryBits.Definitions.Catalog;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Persistence.Repositories;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace CryBits.Host;

public sealed class DataLoader(
    ContentRepository contentRepository,
    MapRepository mapRepository,
    DefinitionCatalog catalog,
    ILogger<DataLoader> logger)
{
    public void LoadAll()
    {
        catalog.Maps = mapRepository.LoadAllMaps().ToDictionary(m => m.Id, m => m);
        logger.ZLogInformation($"Loaded {catalog.Maps.Count} maps");

        catalog.Classes = contentRepository.LoadAll<Class>().ToDictionary(c => c.Id, c => c);
        logger.ZLogInformation($"Loaded {catalog.Classes.Count} classes");

        catalog.Npcs = contentRepository.LoadAll<Npc>().ToDictionary(n => n.Id, n => n);
        logger.ZLogInformation($"Loaded {catalog.Npcs.Count} NPCs");

        catalog.Items = contentRepository.LoadAll<Item>().ToDictionary(i => i.Id, i => i);
        logger.ZLogInformation($"Loaded {catalog.Items.Count} items");

        catalog.Shops = contentRepository.LoadAll<Shop>().ToDictionary(s => s.Id, s => s);
        logger.ZLogInformation($"Loaded {catalog.Shops.Count} shops");
    }
}
