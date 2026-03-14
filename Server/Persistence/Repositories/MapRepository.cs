using CryBits.Entities.Map;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class MapRepository
{
    public static MapRepository Instance { get; } = new();

    public Dictionary<Guid, Map> Read()
    {
        // Load maps from disk.
        var files = Directories.Maps.GetFiles();

        // Create a default map if none exist.
        if (files.Length == 0)
        {
            var map = new Map();
            Write(map);
            return new Dictionary<Guid, Map> { { map.Id, map } };
        }

        var list = new Dictionary<Guid, Map>();
        foreach (var file in files)
            using (var stream = file.OpenRead())
#pragma warning disable SYSLIB0011
                list.Add(new Guid(file.Name.Remove(36)), (Map)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011

        return list;
    }

    public void Write(Map map)
    {
        // Write map to disk.
        using var stream = new FileInfo(Path.Combine(Directories.Maps.FullName, map.Id.ToString()) + Directories.Format)
            .OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, map);
#pragma warning restore SYSLIB0011
    }

    public void WriteAll()
    {
        // Write maps to disk.
        foreach (var map in Map.List.Values)
            Write(map);
    }
}
