using CryBits.Client.Framework.Constants;
using CryBits.Entities.Map;
using System.Runtime.Serialization.Formatters.Binary;

namespace CryBits.Client.Framework.Persistence.Repositories;

public static class MapRepository
{
    /// <summary>Read map data from disk.</summary>
    public static Map Read(Guid id)
    {
        var file = new FileInfo(Path.Combine(Directories.MapsData.FullName, id.ToString()) + Directories.Format);

        using var stream = file.OpenRead();
#pragma warning disable SYSLIB0011
        return (Map)new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011
    }

    public static void Write(Map map)
    {
        using var stream =
            new FileInfo(Path.Combine(Directories.MapsData.FullName, map.Id.ToString()) + Directories.Format)
                .OpenWrite();
#pragma warning disable SYSLIB0011
        new BinaryFormatter().Serialize(stream, map);
#pragma warning restore SYSLIB0011
    }
}
