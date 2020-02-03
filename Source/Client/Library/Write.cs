using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class Write
{
    public static void Options()
    {
        // Cria um arquivo temporário
        BinaryWriter Data = new BinaryWriter(File.OpenWrite(Directories.Options.FullName));

        // Carrega todas as opções
        Data.Write(Lists.Options.GameName);
        Data.Write(Lists.Options.Username);
        Data.Write(Lists.Options.SaveUsername);
        Data.Write(Lists.Options.Sounds);
        Data.Write(Lists.Options.Musics);
        Data.Write(Lists.Options.Chat);

        // Fecha o arquivo
        Data.Dispose();
    }

    public static void Map(short Index)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Maps_Data.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Map);
        Stream.Close();
    }
}