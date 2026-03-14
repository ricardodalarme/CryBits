using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class ItemRepository
{
    public static ItemRepository Instance { get; } = new();

    public Dictionary<Guid, Item> Read()
    {
        // Load items from disk.
        var list = new Dictionary<Guid, Item>();
        var files = Directories.Items.GetFiles();

        foreach (var file in files)
            using (var stream = file.OpenRead())
#pragma warning disable SYSLIB0011
                list.Add(new Guid(file.Name.Remove(36)), (Item)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011

        return list;
    }

    public void WriteAll()
    {
        // Write items to disk.
        foreach (var item in Item.List.Values)
            using (var stream =
                   new FileInfo(Path.Combine(Directories.Items.FullName, item.Id.ToString()) + Directories.Format)
                       .OpenWrite())
#pragma warning disable SYSLIB0011
                new BinaryFormatter().Serialize(stream, item);
#pragma warning restore SYSLIB0011
    }
}
