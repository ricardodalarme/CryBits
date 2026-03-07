using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities.Map;

namespace CryBits.Server.Persistence.Repositories;

internal static class MapRepository
{
    public static Dictionary<Guid, Map> Read()
    {
        // Load maps from disk.
        var list = new Dictionary<Guid, Map>();
        var file = Directories.Maps.GetFiles();

        if (file.Length > 0)
            for (byte i = 0; i < file.Length; i++)
                using (var stream = file[i].OpenRead())
                {
#pragma warning disable SYSLIB0011
                    list.Add(new Guid(file[i].Name.Remove(36)), (Map)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011
                }
        // Create a default map if none exist.
        else
        {
            var map = new Map();
            list.Add(map.Id, map);
            Write(map);
        }

        return list;
    }

    public static void Write(Map map)
    {
        // Write map to disk.
        using var stream = new FileInfo(Path.Combine(Directories.Maps.FullName, map.Id.ToString()) + Directories.Format)
            .OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, map);
#pragma warning restore SYSLIB0011
    }

    public static void WriteAll()
    {
        // Write maps to disk.
        foreach (var map in Map.List.Values)
            Write(map);
    }
}
