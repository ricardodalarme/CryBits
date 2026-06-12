using CryBits.Definitions.Catalog;
using CryBits.Definitions.Items;
using CryBits.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class ItemRepository(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ItemRepository Instance { get; } = new(DefinitionCatalog.Instance);

    public Dictionary<Guid, Item> Read()
    {
        var list = new Dictionary<Guid, Item>();
        var files = Directories.Items.GetFiles("*" + Directories.Format);

        foreach (var file in files)
        {
            var json = File.ReadAllText(file.FullName);
            list.Add(new Guid(Path.GetFileNameWithoutExtension(file.Name)), JsonSerializer.Deserialize<Item>(json, JsonConfig.Options)!);
        }

        return list;
    }

    public void WriteAll()
    {
        foreach (var item in _catalog.Items.Values)
        {
            var path = Path.Combine(Directories.Items.FullName, item.Id.ToString()) + Directories.Format;
            var json = JsonSerializer.Serialize(item, JsonConfig.Options);
            File.WriteAllText(path, json);
        }
    }
}
