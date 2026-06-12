using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class MapRepository(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static MapRepository Instance { get; } = new(DefinitionCatalog.Instance);

    public Dictionary<Guid, Map> Read()
    {
        var files = Directories.Maps.GetFiles("*" + Directories.Format);

        if (files.Length == 0)
        {
            var map = new Map();
            Write(map);
            return new Dictionary<Guid, Map> { { map.Id, map } };
        }

        var list = new Dictionary<Guid, Map>();
        foreach (var file in files)
        {
            var json = File.ReadAllText(file.FullName);
            list.Add(new Guid(Path.GetFileNameWithoutExtension(file.Name)), JsonSerializer.Deserialize<Map>(json, JsonConfig.Options)!);
        }

        return list;
    }

    public void Write(Map map)
    {
        var path = Path.Combine(Directories.Maps.FullName, map.Id.ToString()) + Directories.Format;
        var json = JsonSerializer.Serialize(map, JsonConfig.Options);
        File.WriteAllText(path, json);
    }

    public void WriteAll()
    {
        foreach (var map in _catalog.Maps.Values)
            Write(map);
    }
}
