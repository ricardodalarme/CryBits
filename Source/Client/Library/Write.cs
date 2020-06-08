using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class Write
{
    public static void Options()
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Options.FullName).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Options);
        Stream.Close();
    }

    public static void Map(Objects.Map Map)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Maps_Data.FullName + Map.ID.ToString() + Directories.Format).OpenWrite();
        // new BinaryFormatter().Serialize(Stream, Map);
        Stream.Close();
    }
}