using CryBits.Definitions.Catalog;
using CryBits.Definitions.Classes;
using CryBits.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CryBits.Server.Persistence.Repositories;

internal sealed class ClassRepository(DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static ClassRepository Instance { get; } = new(DefinitionCatalog.Instance);

    public Dictionary<Guid, Class> Read()
    {
        var files = Directories.Classes.GetFiles("*" + Directories.Format);

        if (files.Length == 0)
        {
            var @class = new Class();
            Write(@class);
            return new Dictionary<Guid, Class> { { @class.Id, @class } };
        }

        var list = new Dictionary<Guid, Class>();
        foreach (var file in files)
        {
            var json = File.ReadAllText(file.FullName);
            list.Add(new Guid(Path.GetFileNameWithoutExtension(file.Name)), JsonSerializer.Deserialize<Class>(json, JsonConfig.Options)!);
        }

        return list;
    }

    public void Write(Class @class)
    {
        var path = Path.Combine(Directories.Classes.FullName, @class.Id.ToString()) + Directories.Format;
        var json = JsonSerializer.Serialize(@class, JsonConfig.Options);
        File.WriteAllText(path, json);
    }

    public void WriteAll()
    {
        foreach (var @class in _catalog.Classes.Values)
            Write(@class);
    }
}
