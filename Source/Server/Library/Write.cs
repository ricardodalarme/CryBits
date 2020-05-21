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
        Data.Write(Lists.GetID(Account.Character.Class));
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
            Data.Write(Lists.GetID(Account.Character.Inventory[n].Item));
            Data.Write(Account.Character.Inventory[n].Amount);
        }
        for (byte n = 0; n < (byte)Game.Equipments.Count; n++) Data.Write(Lists.GetID(Account.Character.Equipment[n]));
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

    public static void Class(Objects.Class Class)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Classes.FullName + Class.ID + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Class);
        Stream.Close();
    }

    public static void NPC(Objects.NPC NPC)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.NPCs.FullName + NPC.ID + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, NPC);
        Stream.Close();
    }

    public static void Item(Objects.Item Item)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Items.FullName + Item.ID + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Item);
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

    public static void Shop(Objects.Shop Shop)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Shops.FullName + Shop.ID + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Shop);
        Stream.Close();
    }
}