using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class Write
{
    public static void Player(byte Index)
    {
        string Directory = Directories.Accounts.FullName + Lists.Player[Index].User + Directories.Format;

        // Evita erros
        if (string.IsNullOrEmpty(Lists.Player[Index].User)) return;

        // Cria um arquivo temporário
        BinaryWriter Data = new BinaryWriter(File.OpenWrite(Directory));

        // Salva os dados no arquivo
        Data.Write(Lists.Player[Index].User);
        Data.Write(Lists.Player[Index].Password);
        Data.Write((byte)Lists.Player[Index].Acess);

        for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
        {
            Data.Write(Lists.Player[Index].Character[i].Name);
            Data.Write(Lists.Player[Index].Character[i].Class);
            Data.Write(Lists.Player[Index].Character[i].Texture_Num);
            Data.Write(Lists.Player[Index].Character[i].Genre);
            Data.Write(Lists.Player[Index].Character[i].Level);
            Data.Write(Lists.Player[Index].Character[i].Experience);
            Data.Write(Lists.Player[Index].Character[i].Points);
            Data.Write(Lists.Player[Index].Character[i].Map);
            Data.Write(Lists.Player[Index].Character[i].X);
            Data.Write(Lists.Player[Index].Character[i].Y);
            Data.Write((byte)Lists.Player[Index].Character[i].Direction);
            for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Data.Write(Lists.Player[Index].Character[i].Vital[n]);
            for (byte n = 0; n < (byte)Game.Attributes.Count; n++) Data.Write(Lists.Player[Index].Character[i].Attribute[n]);
            for (byte n = 1; n <= Game.Max_Inventory; n++)
            {
                Data.Write(Lists.Player[Index].Character[i].Inventory[n].Item_Num);
                Data.Write(Lists.Player[Index].Character[i].Inventory[n].Amount);
            }
            for (byte n = 0; n < (byte)Game.Equipments.Count; n++) Data.Write(Lists.Player[Index].Character[i].Equipment[n]);
            for (byte n = 1; n <= Game.Max_Hotbar; n++)
            {
                Data.Write(Lists.Player[Index].Character[i].Hotbar[n].Type);
                Data.Write(Lists.Player[Index].Character[i].Hotbar[n].Slot);
            }
        }

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Character(string Name)
    {
        // Cria um arquivo temporário
        StreamWriter Data = new StreamWriter(Directories.Characters.FullName, true);

        // Salva o nome do personagem no arquivo
        Data.Write(";" + Name + ":");

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Characters(string Characters_Name)
    {
        // Cria um arquivo temporário
        StreamWriter Data = new StreamWriter(Directories.Characters.FullName);

        // Salva o nome do personagem no arquivo
        Data.Write(Characters_Name);

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Server_Data()
    {
        // Escreve os dados
        FileStream Stream = Directories.Server_Data.OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Server_Data);
        Stream.Close();
    }

    public static void Classes()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.Class.Length; Index++) Class(Index);
    }

    public static void Class(byte Index)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Classes.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Class[Index]);
        Stream.Close();
    }

    public static void NPCs()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.NPC.Length; Index++) NPC(Index);
    }

    public static void NPC(short Index)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.NPCs.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.NPC[Index]);
        Stream.Close();
    }

    public static void Items()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.Item.Length; Index++) Item(Index);
    }

    public static void Item(byte Index)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Items.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Item[Index]);
        Stream.Close();
    }

    public static void Tiles()
    {
        // Escreve os dados
        for (byte i = 1; i < Lists.Tile.Length; i++) Tile(i);
    }

    public static void Tile(byte Index)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Tiles.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Tile[Index]);
        Stream.Close();
    }

    public static void Maps()
    {
        // Escreve os dados
        for (short i = 1; i < Lists.Map.Length; i++) Map(i);
    }

    public static void Map(short Index)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Maps.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Map[Index]);
        Stream.Close();
    }

    public static void Shops()
    {
        // Escreve os dados
        for (short i = 1; i < Lists.Shop.Length; i++) Shop(i);
    }

    public static void Shop(short Index)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Shops.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Shop[Index]);
        Stream.Close();
    }
}