using CryBits.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class ClassRepository
{
    public static ClassRepository Instance { get; } = new();

    public Dictionary<Guid, Class> Read()
    {
        var files = Directories.Classes.GetFiles();

        // Create a default class if none exist.
        if (files.Length == 0)
        {
            var @class = new Class();
            Write(@class);
            return new Dictionary<Guid, Class> { { @class.Id, @class } };
        }

        // Load classes from disk.
        var list = new Dictionary<Guid, Class>();
        foreach (var file in files)
            using (var stream = file.OpenRead())
#pragma warning disable SYSLIB0011
                list.Add(new Guid(file.Name.Remove(36)), (Class)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011

        return list;
    }

    public void Write(Class @class)
    {
        // Write class to disk.
        using var stream =
            new FileInfo(Path.Combine(Directories.Classes.FullName, @class.Id.ToString()) + Directories.Format)
                .OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, @class);
#pragma warning restore SYSLIB0011
    }

    public void WriteAll()
    {
        // Write classes to disk.
        foreach (var @class in Class.List.Values)
            Write(@class);
    }
}
