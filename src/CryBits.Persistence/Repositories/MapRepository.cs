using CryBits.Definitions.Maps;
using CryBits.Persistence.Serialization;
using MemoryPack;
using System.Text.Json;

namespace CryBits.Persistence.Repositories;

public sealed class MapRepository
{
    private const string ManifestName = "manifest.json";
    private const string ChunkFormat = ".chunk";
    private const string ChunksSubDir = "chunks";

    public DirectoryInfo MapsDirectory { get; }

    public MapRepository() : this(Directories.Content) { }

    public MapRepository(DirectoryInfo contentDirectory)
    {
        MapsDirectory = new DirectoryInfo(Path.Combine(contentDirectory.FullName, "Map"));
    }

    public void SaveMap(Map map)
    {
        var mapDir = MapDir(map.Id);
        mapDir.Create();
        var chunksDir = Path.Combine(mapDir.FullName, ChunksSubDir);
        Directory.CreateDirectory(chunksDir);

        var manifest = map with { Chunks = [] };
        var json = JsonSerializer.Serialize(manifest, JsonConfig.Options);
        File.WriteAllText(Path.Combine(mapDir.FullName, ManifestName), json);

        foreach (var (coord, chunk) in map.Chunks)
        {
            var chunkPath = Path.Combine(chunksDir, ChunkFileName(coord.X, coord.Y));
            File.WriteAllBytes(chunkPath, MemoryPackSerializer.Serialize(chunk));
        }
    }

    public void SaveAllMaps(IEnumerable<Map> maps)
    {
        var saved = new HashSet<Guid>();
        foreach (var map in maps)
        {
            SaveMap(map);
            saved.Add(map.Id);
        }

        // Delete maps on disk that are no longer in the collection
        if (!MapsDirectory.Exists) return;
        foreach (var subDir in MapsDirectory.GetDirectories())
            if (Guid.TryParse(subDir.Name, out var id) && !saved.Contains(id))
                subDir.Delete(true);
    }

    public Map? LoadMap(Guid id)
    {
        var mapDir = MapDir(id);
        if (!mapDir.Exists) return null;

        var manifestPath = Path.Combine(mapDir.FullName, ManifestName);
        if (!File.Exists(manifestPath)) return null;

        var json = File.ReadAllText(manifestPath);
        var map = JsonSerializer.Deserialize<Map>(json, JsonConfig.Options);
        if (map == null) return null;

        var chunksDir = Path.Combine(mapDir.FullName, ChunksSubDir);
        if (Directory.Exists(chunksDir))
            foreach (var file in Directory.GetFiles(chunksDir, "*" + ChunkFormat))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var parts = name.Split('_');
                if (parts.Length != 2) continue;
                if (!short.TryParse(parts[0], out var cx)) continue;
                if (!short.TryParse(parts[1], out var cy)) continue;

                var chunk = MemoryPackSerializer.Deserialize<MapChunk>(File.ReadAllBytes(file));
                if (chunk != null)
                    map.Chunks[new ChunkCoord(cx, cy)] = chunk;
            }

        return map;
    }

    public IEnumerable<Map> LoadAllMaps()
    {
        if (!MapsDirectory.Exists) yield break;
        foreach (var subDir in MapsDirectory.GetDirectories())
            if (Guid.TryParse(subDir.Name, out var id))
            {
                var map = LoadMap(id);
                if (map != null) yield return map;
            }
    }

    private DirectoryInfo MapDir(Guid id) =>
        new(Path.Combine(MapsDirectory.FullName, id.ToString()));

    private static string ChunkFileName(short x, short y) =>
        $"{x}_{y}{ChunkFormat}";
}
