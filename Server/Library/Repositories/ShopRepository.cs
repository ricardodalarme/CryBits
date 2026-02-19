using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities.Shop;

namespace CryBits.Server.Library.Repositories;

internal static class ShopRepository
{
    public static void Read()
    {
        // Lê os dados
        Shop.List = [];
        var file = Directories.Shops.GetFiles();
        for (byte i = 0; i < file.Length; i++)
            using (var stream = file[i].OpenRead())
                Shop.List.Add(new Guid(file[i].Name.Remove(36)), (Shop)new BinaryFormatter().Deserialize(stream));
    }

    public static void WriteAll()
    {
        // Escreve os dados
        foreach (var shop in Shop.List.Values)
            using (var stream =
                   new FileInfo(Path.Combine(Directories.Shops.FullName, shop.Id.ToString()) + Directories.Format)
                       .OpenWrite())
#pragma warning disable SYSLIB0011
                new BinaryFormatter().Serialize(stream, shop);
#pragma warning restore SYSLIB0011
    }
}
