using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;

namespace CryBits.Server.Library.Repositories;

internal static class ClassRepository
{
    public static void Read()
    {
        Class.List = [];
        var file = Directories.Classes.GetFiles();

        // Lê os dados
        if (file.Length > 0)
            for (byte i = 0; i < file.Length; i++)
                using (var stream = file[i].OpenRead())
                    Class.List.Add(new Guid(file[i].Name.Remove(36)), (Class)new BinaryFormatter().Deserialize(stream));
        // Cria uma classe caso não houver nenhuma
        else
        {
            var @class = new Class();
            Class.List.Add(@class.Id, @class);
            Write(@class);
        }
    }

    public static void Write(Class @class)
    {
        // Escreve os dados
        using var stream =
            new FileInfo(Path.Combine(Directories.Classes.FullName, @class.Id.ToString()) + Directories.Format)
                .OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, @class);
#pragma warning restore SYSLIB0011
    }

    public static void WriteAll()
    {
        // Escreve os dados
        foreach (var @class in Class.List.Values)
            Write(@class);
    }
}
