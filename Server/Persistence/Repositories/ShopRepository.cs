using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities.Shop;

namespace CryBits.Server.Persistence.Repositories;

internal static class ShopRepository
{
    public static Dictionary<Guid, Shop> Read()
    {
        // Load shops from disk.
        var list = new Dictionary<Guid, Shop>();
        var files = Directories.Shops.GetFiles();
        foreach (var file in files)
            using (var stream = file.OpenRead())
#pragma warning disable SYSLIB0011
                list.Add(new Guid(file.Name.Remove(36)), (Shop)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011

        return list;
    }

    public static void WriteAll()
    {
        // Write shops to disk.
        foreach (var shop in Shop.List.Values)
            using (var stream =
                   new FileInfo(Path.Combine(Directories.Shops.FullName, shop.Id.ToString()) + Directories.Format)
                       .OpenWrite())
#pragma warning disable SYSLIB0011
                new BinaryFormatter().Serialize(stream, shop);
#pragma warning restore SYSLIB0011
    }
}
