using CryBits.Definitions.Catalog;
using CryBits.Definitions.Shops;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class ShopRepository(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ShopRepository Instance { get; } = new(DefinitionCatalog.Instance);

    public Dictionary<Guid, Shop> Read()
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

    public void WriteAll()
    {
        // Write shops to disk.
        foreach (var shop in _catalog.Shops.Values)
            using (var stream =
                   new FileInfo(Path.Combine(Directories.Shops.FullName, shop.Id.ToString()) + Directories.Format)
                       .OpenWrite())
#pragma warning disable SYSLIB0011
                new BinaryFormatter().Serialize(stream, shop);
#pragma warning restore SYSLIB0011
    }
}
