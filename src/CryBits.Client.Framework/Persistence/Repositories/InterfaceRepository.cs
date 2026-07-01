using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Dtos;

namespace CryBits.Client.Framework.Persistence.Repositories;

/// <summary>Repository for loading and saving UI layout configuration files.</summary>
public static class InterfaceRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault,
        PropertyNameCaseInsensitive = true
    };

    public static UILayout Load(string path)
    {
        if (!File.Exists(path)) return new UILayout();
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<UILayout>(json, JsonOptions) ?? new UILayout();
    }

    public static void Save(string path, UILayout config)
    {
        var json = JsonSerializer.Serialize(config, JsonOptions);
        var dir = Path.GetDirectoryName(path);
        if (dir != null) Directory.CreateDirectory(dir);
        File.WriteAllText(path, json);
    }
}
