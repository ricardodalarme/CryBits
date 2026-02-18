using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities.Map;

namespace CryBits.Server.Library.Repositories;

internal static class MapRepository
{
    public static void Read()
    {
        // Lê os dados
        Map.List = new System.Collections.Generic.Dictionary<Guid, Map>();
        var file = Directories.Maps.GetFiles();

        if (file.Length > 0)
            for (byte i = 0; i < file.Length; i++)
                using (var stream = file[i].OpenRead())
                    Map.List.Add(new Guid(file[i].Name.Remove(36)), (Map)new BinaryFormatter().Deserialize(stream));
        // Cria um mapa novo caso não houver nenhuma
        else
        {
            var map = new Map();
            Map.List.Add(map.Id, map);
            Write(map);
        }
    }

    public static void Write(Map map)
    {
        // Escreve os dados
        using var stream = new FileInfo(Path.Combine(Directories.Maps.FullName, map.Id.ToString()) + Directories.Format).OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, map);
#pragma warning restore SYSLIB0011
    }

    public static void WriteAll()
    {
        // Escreve os dados
        foreach (var map in Map.List.Values)
            Write(map);
    }
}
