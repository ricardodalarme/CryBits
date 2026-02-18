using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Client.Framework.Constants;
using CryBits.Entities.Map;

namespace CryBits.Client.Framework.Library.Repositories;

public static class MapRepository
{
    public static void Read(Guid id)
    {
        var file = new FileInfo(Path.Combine(Directories.MapsData.FullName, id.ToString()) + Directories.Format);

        // Lê os dados
        using var stream = file.OpenRead();
#pragma warning disable SYSLIB0011
        CryBits.Entities.Map.Map.List.Add(id, (Map)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011
    }

    public static void Write(Map map)
    {
        using var stream =
            new FileInfo(Path.Combine(Directories.MapsData.FullName, map.Id.ToString()) + Directories.Format)
                .OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, map);
#pragma warning restore SYSLIB0011
    }
}
