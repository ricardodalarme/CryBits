using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;

namespace CryBits.Server.Persistence.Repositories;

internal static class ClassRepository
{
    public static Dictionary<Guid, Class> Read()
    {
        var list = new Dictionary<Guid, Class>();
        var file = Directories.Classes.GetFiles();

        // Load classes from disk.
        if (file.Length > 0)
            for (byte i = 0; i < file.Length; i++)
                using (var stream = file[i].OpenRead())
                {
#pragma warning disable SYSLIB0011
                    list.Add(new Guid(file[i].Name.Remove(36)), (Class)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011
                }
        // Create a default class if none exist.
        else
        {
            var @class = new Class();
            list.Add(@class.Id, @class);
            Write(@class);
        }

        return list;
    }

    public static void Write(Class @class)
    {
        // Write class to disk.
        using var stream =
            new FileInfo(Path.Combine(Directories.Classes.FullName, @class.Id.ToString()) + Directories.Format)
                .OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, @class);
#pragma warning restore SYSLIB0011
    }

    public static void WriteAll()
    {
        // Write classes to disk.
        foreach (var @class in Class.List.Values)
            Write(@class);
    }
}
