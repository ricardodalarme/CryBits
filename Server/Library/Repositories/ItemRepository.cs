using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;

namespace CryBits.Server.Library.Repositories;

internal static class ItemRepository
{
    public static void Read()
    {
        // Load items from disk.
        Item.List = [];
        var file = Directories.Items.GetFiles();
        for (byte i = 0; i < file.Length; i++)
            using (var stream = file[i].OpenRead())
                Item.List.Add(new Guid(file[i].Name.Remove(36)), (Item)new BinaryFormatter().Deserialize(stream));
    }

    public static void WriteAll()
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
