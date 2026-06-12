using CryBits.Definitions.Catalog;
using CryBits.Definitions.Shops;
using CryBits.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class ShopRepository(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ShopRepository Instance { get; } = new(DefinitionCatalog.Instance);

    public Dictionary<Guid, Shop> Read()
    {
        var list = new Dictionary<Guid, Shop>();
        var files = Directories.Shops.GetFiles("*" + Directories.Format);

        foreach (var file in files)
        {
            var json = File.ReadAllText(file.FullName);
            list.Add(new Guid(Path.GetFileNameWithoutExtension(file.Name)), JsonSerializer.Deserialize<Shop>(json, JsonConfig.Options)!);
        }

        return list;
    }

    public void WriteAll()
    {
        foreach (var shop in _catalog.Shops.Values)
        {
            var path = Path.Combine(Directories.Shops.FullName, shop.Id.ToString()) + Directories.Format;
            var json = JsonSerializer.Serialize(shop, JsonConfig.Options);
            File.WriteAllText(path, json);
        }
    }
}
