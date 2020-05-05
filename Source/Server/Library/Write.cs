using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class Write
{
    public static void Account(Account.Structure Account)
    {
        FileInfo File = new FileInfo(Directories.Accounts.FullName + Account.User + "\\Data" + Directories.Format);

        // Evita erros
        if (!File.Directory.Exists) File.Directory.Create();

        // Cria um arquivo temporário
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Salva os dados no arquivo
        Data.Write(Account.User);
        Data.Write(Account.Password);
        Data.Write((byte)Account.Acess);

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Character(Account.Structure Account)
    {
        FileInfo File = new FileInfo(Directories.Accounts.FullName + Account.User + "\\Characters\\" + Account.Character.Name + Directories.Format);

        // Evita erros
        if (!File.Directory.Exists) File.Directory.Create();

        // Cria um arquivo temporário
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Salva os dados no arquivo
        Data.Write(Account.Character.Name);
        Data.Write(Account.Character.Texture_Num);
        Data.Write(Account.Character.Level);
        Data.Write(Account.Character.Class_Num);
        Data.Write(Account.Character.Genre);
        Data.Write(Account.Character.Experience);
        Data.Write(Account.Character.Points);
        Data.Write(Account.Character.Map_Num);
        Data.Write(Account.Character.X);
        Data.Write(Account.Character.Y);
        Data.Write((byte)Account.Character.Direction);
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Data.Write(Account.Character.Vital[n]);
        for (byte n = 0; n < (byte)Game.Attributes.Count; n++) Data.Write(Account.Character.Attribute[n]);
        for (byte n = 1; n <= Game.Max_Inventory; n++)
        {
            Data.Write(Account.Character.Inventory[n].Item_Num);
            Data.Write(Account.Character.Inventory[n].Amount);
        }
        for (byte n = 0; n < (byte)Game.Equipments.Count; n++) Data.Write(Account.Character.Equipment[n]);
        for (byte n = 1; n <= Game.Max_Hotbar; n++)
        {
            Data.Write(Account.Character.Hotbar[n].Type);
            Data.Write(Account.Character.Hotbar[n].Slot);
        }

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Character_Name(string Name)
    {
        // Cria um arquivo temporário
        StreamWriter Data = new StreamWriter(Directories.Characters.FullName, true);

        // Salva o nome do personagem no arquivo
        Data.Write(";" + Name + ":");

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Characters_Name(string Characters_Name)
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