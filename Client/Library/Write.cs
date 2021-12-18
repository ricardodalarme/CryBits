using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Entities;
using static CryBits.Client.Logic.Options;

namespace CryBits.Client.Library;

internal static class Write
{
    public static void Options()
    {
        // Escreve as configurações
        using var data = new BinaryWriter(Directories.Options.OpenWrite());
        data.Write(SaveUsername);
        data.Write(Username);
        data.Write(Sounds);
        data.Write(Musics);
        data.Write(Chat);
        data.Write(FPS);
        data.Write(Latency);
        data.Write(Party);
        data.Write(Trade);
    }

    public static void Map(Map map)
    {
        // Escreve os dados
        using var stream = new FileInfo(Directories.MapsData.FullName + map.ID + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(stream, map);
    }
}