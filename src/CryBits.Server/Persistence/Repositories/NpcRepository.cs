using CryBits.Definitions.Catalog;
using CryBits.Definitions.Npcs;
using CryBits.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class NpcRepository(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static NpcRepository Instance { get; } = new(DefinitionCatalog.Instance);

    public Dictionary<Guid, Npc> Read()
    {
        var list = new Dictionary<Guid, Npc>();
        var files = Directories.Npcs.GetFiles("*" + Directories.Format);

        foreach (var file in files)
        {
            var json = File.ReadAllText(file.FullName);
            list.Add(new Guid(Path.GetFileNameWithoutExtension(file.Name)), JsonSerializer.Deserialize<Npc>(json, JsonConfig.Options)!);
        }

        return list;
    }

    public void WriteAll()
    {
        foreach (var npc in _catalog.Npcs.Values)
        {
            var path = Path.Combine(Directories.Npcs.FullName, npc.Id.ToString()) + Directories.Format;
            var json = JsonSerializer.Serialize(npc, JsonConfig.Options);
            File.WriteAllText(path, json);
        }
    }
}
