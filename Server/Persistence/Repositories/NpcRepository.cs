using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities.Npc;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class NpcRepository
{
    public static NpcRepository Instance { get; } = new();

    public void Read()
    {
        // Load NPCs from disk.
        Npc.List = [];
        var file = Directories.Npcs.GetFiles();
        for (byte i = 0; i < file.Length; i++)
            using (var stream = file[i].OpenRead())
                Npc.List.Add(new Guid(file[i].Name.Remove(36)), (Npc)new BinaryFormatter().Deserialize(stream));
    }

    public void WriteAll()
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
