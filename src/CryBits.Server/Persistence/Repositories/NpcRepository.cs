using CryBits.Definitions.Catalog;
using CryBits.Definitions.Npcs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class NpcRepository(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static NpcRepository Instance { get; } = new(DefinitionCatalog.Instance);

    public Dictionary<Guid, Npc> Read()
    {
        // Load NPCs from disk.
        var list = new Dictionary<Guid, Npc>();
        var files = Directories.Npcs.GetFiles();
        foreach (var file in files)
            using (var stream = file.OpenRead())
#pragma warning disable SYSLIB0011
                list.Add(new Guid(file.Name.Remove(36)), (Npc)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011

        return list;
    }

    public void WriteAll()
    {
        // Write NPCs to disk.
        foreach (var npc in _catalog.Npcs.Values)
            using (var stream =
                   new FileInfo(Path.Combine(Directories.Npcs.FullName, npc.Id.ToString()) + Directories.Format)
                       .OpenWrite())
#pragma warning disable SYSLIB0011
                new BinaryFormatter().Serialize(stream, npc);
#pragma warning restore SYSLIB0011
    }
}
