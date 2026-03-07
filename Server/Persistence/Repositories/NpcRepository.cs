using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities.Npc;

namespace CryBits.Server.Persistence.Repositories;

internal static class NpcRepository
{
    public static Dictionary<Guid, Npc> Read()
    {
        // Load NPCs from disk.
        var list = new Dictionary<Guid, Npc>();
        var file = Directories.Npcs.GetFiles();
        for (byte i = 0; i < file.Length; i++)
            using (var stream = file[i].OpenRead())
            {
#pragma warning disable SYSLIB0011
                list.Add(new Guid(file[i].Name.Remove(36)), (Npc)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011
            }
        return list;
    }

    public static void WriteAll()
    {
        // Write NPCs to disk.
        foreach (var npc in Npc.List.Values)
            using (var stream =
                   new FileInfo(Path.Combine(Directories.Npcs.FullName, npc.Id.ToString()) + Directories.Format)
                       .OpenWrite())
#pragma warning disable SYSLIB0011
                new BinaryFormatter().Serialize(stream, npc);
#pragma warning restore SYSLIB0011
    }
}
