using CryBits.Client.Framework.Constants;
using CryBits.Definitions.Maps;
using CryBits.Utils;
using System.Text.Json;

namespace CryBits.Client.Framework.Persistence.Repositories;

public static class MapRepository
{
    public static Map Read(Guid id)
    {
        var path = Path.Combine(Directories.MapsData.FullName, id.ToString()) + Directories.Format;
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Map>(json, JsonConfig.Options)!;
    }

    public static void Write(Map map)
    {
        var path = Path.Combine(Directories.MapsData.FullName, map.Id.ToString()) + Directories.Format;
        var json = JsonSerializer.Serialize(map, JsonConfig.Options);
        File.WriteAllText(path, json);
    }
}
