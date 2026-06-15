using CryBits.Definitions.Common;
using CryBits.Persistence.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CryBits.Persistence.Repositories;

public class ContentRepository()
{
    private const string Format = ".json";

    public T? Load<T>(Guid id) where T : Entity
    {
        var path = PathFor<T>(id);
        if (!File.Exists(path)) return null;

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, JsonConfig.Options);
    }

    public IEnumerable<T> LoadAll<T>() where T : Entity
    {
        var dir = DirectoryFor<T>();
        if (!dir.Exists) yield break;

        foreach (var file in dir.GetFiles("*" + Format))
        {
            var json = File.ReadAllText(file.FullName);
            var entity = JsonSerializer.Deserialize<T>(json, JsonConfig.Options);
            if (entity is not null) yield return entity;
        }
    }

    public void Save<T>(T entity) where T : Entity
    {
        var dir = DirectoryFor<T>();
        if (!dir.Exists) dir.Create();

        var json = JsonSerializer.Serialize(entity, JsonConfig.Options);
        File.WriteAllText(PathFor<T>(entity.Id), json);
    }

    public void SaveAll<T>(IEnumerable<T> entities) where T : Entity
    {
        foreach (var entity in entities)
            Save(entity);
    }

    public DirectoryInfo DirectoryFor<T>() where T : Entity =>
        new(Path.Combine(Directories.Content.FullName, typeof(T).Name));

    private string PathFor<T>(Guid id) where T : Entity =>
        Path.Combine(DirectoryFor<T>().FullName, id.ToString()) + Format;
}
