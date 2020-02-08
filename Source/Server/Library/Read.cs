using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

partial class Read
{
    public static void All()
    {
        // Carrega todos os dados
        Console.WriteLine("Loading data.");
        Server_Data();
        Console.WriteLine("Loading classes.");
        Classes();
        Console.WriteLine("Loading NPCs.");
        NPCs();
        Console.WriteLine("Loading items.");
        Items();
        Console.WriteLine("Loading maps.");
        Maps();
        Console.WriteLine("Loading tiles.");
        Tiles();
    }

    public static void Server_Data()
    {
        // Cria o arquivo caso ele não existir
        if (!Directories.Server_Data.Exists)
        {
            Clear.Server_Data();
            Write.Server_Data();
            return;
        }

        // Lê os dados
        FileStream Stream = Directories.Server_Data.OpenRead();
        Lists.Server_Data = (Lists.Structures.Server_Data)new BinaryFormatter().Deserialize(Stream);
        Stream.Close();
    }

    public static void Player(byte Index, string Name, bool ReadCharacter = true)
    {
        string Directory = Directories.Accounts.FullName + Name + Directories.Format;

        // Cria um arquivo temporário
        BinaryReader Data = new BinaryReader(File.OpenRead(Directory));

        // Carrega os dados e os adiciona ao cache
        Lists.Player[Index].User = Data.ReadString();
        Lists.Player[Index].Password = Data.ReadString();
        Lists.Player[Index].Acess = (Game.Accesses)Data.ReadByte();

        // Dados do personagem
        if (ReadCharacter)
            for (byte i = 1; i <= Lists.Server_Data.Max_Characters; i++)
            {
                Lists.Player[Index].Character[i].Name = Data.ReadString();
                Lists.Player[Index].Character[i].Class = Data.ReadByte();
                Lists.Player[Index].Character[i].Texture_Num = Data.ReadInt16();
                Lists.Player[Index].Character[i].Genre = Data.ReadBoolean();
                Lists.Player[Index].Character[i].Level = Data.ReadInt16();
                Lists.Player[Index].Character[i].Experience = Data.ReadInt32();
                Lists.Player[Index].Character[i].Points = Data.ReadByte();
                Lists.Player[Index].Character[i].Map = Data.ReadInt16();
                Lists.Player[Index].Character[i].X = Data.ReadByte();
                Lists.Player[Index].Character[i].Y = Data.ReadByte();
                Lists.Player[Index].Character[i].Direction = (Game.Directions)Data.ReadByte();
                for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Lists.Player[Index].Character[i].Vital[n] = Data.ReadInt16();
                for (byte n = 0; n < (byte)Game.Attributes.Count; n++) Lists.Player[Index].Character[i].Attribute[n] = Data.ReadInt16();
                for (byte n = 1; n <= Game.Max_Inventory; n++)
                {
                    Lists.Player[Index].Character[i].Inventory[n].Item_Num = Data.ReadInt16();
                    Lists.Player[Index].Character[i].Inventory[n].Amount = Data.ReadInt16();
                }
                for (byte n = 0; n < (byte)Game.Equipments.Count; n++) Lists.Player[Index].Character[i].Equipment[n] = Data.ReadInt16();
                for (byte n = 1; n <= Game.Max_Hotbar; n++)
                {
                    Lists.Player[Index].Character[i].Hotbar[n].Type = Data.ReadByte();
                    Lists.Player[Index].Character[i].Hotbar[n].Slot = Data.ReadByte();
                }
            }

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static string Player_Password(string User)
    {
        // Cria um arquivo temporário
        BinaryReader Data = new BinaryReader(File.OpenRead(Directories.Accounts.FullName + User + Directories.Format));

        // Encontra a senha da conta
        Data.ReadString();
        string Password = Data.ReadString();

        // Descarrega o arquivo
        Data.Dispose();

        // Retorna o valor da função
        return Password;
    }

    public static string Characters_Name()
    {
        // Cria o arquivo caso ele não existir
        if (!Directories.Characters.Exists)
        {
            Write.Characters(string.Empty);
            return string.Empty;
        }

        // Cria um arquivo temporário
        StreamReader Data = new StreamReader(Directories.Characters.FullName);

        // Carrega todos os nomes dos personagens
        string Characters = Data.ReadToEnd();

        // Descarrega o arquivo
        Data.Dispose();

        // Retorna o valor de acordo com o que foi carregado
        return Characters;
    }

    public static void Classes()
    {
        // Lê os dados
        Lists.Class = new Lists.Structures.Class[Lists.Server_Data.Num_Classes + 1];
        for (byte i = 1; i < Lists.Class.Length; i++) Class(i);
    }

    public static void Class(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Classes.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Clear.Class(Index);
            Write.Class(Index);
            return;
        }

        // Lê os dados
        FileStream Stream = File.OpenRead();
        Lists.Class[Index] = (Lists.Structures.Class)new BinaryFormatter().Deserialize(Stream);
        Stream.Close();
    }

    public static void Items()
    {
        // Lê os dados
        Lists.Item = new Lists.Structures.Item[Lists.Server_Data.Num_Items + 1];
        for (byte i = 1; i < Lists.Item.Length; i++) Item(i);
    }

    public static void Item(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Items.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Clear.Item(Index);
            Write.Item(Index);
            return;
        }

        // Lê os dados
        FileStream Stream = File.OpenRead();
        Lists.Item[Index] = (Lists.Structures.Item)new BinaryFormatter().Deserialize(Stream);
        Stream.Close();
    }

    public static void Maps()
    {
        // Lê os dados1
        Lists.Map = new Lists.Structures.Map[Lists.Server_Data.Num_Maps + 1];
        Lists.Temp_Map = new Lists.Structures.Temp_Map[Lists.Server_Data.Num_Maps + 1];
        for (short i = 1; i < Lists.Map.Length; i++) Map(i);
    }

    public static void Map(short Index)
    {
        FileInfo File = new FileInfo(Directories.Maps.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Clear.Map(Index);
            Write.Map(Index);
            return;
        }

        // Lê os dados
        FileStream Stream = File.OpenRead();
        Lists.Map[Index] = (Lists.Structures.Map)new BinaryFormatter().Deserialize(Stream);
        Stream.Close();

        // Dados temporários
        Lists.Temp_Map[Index].NPC = new Lists.Structures.Map_NPCs[Lists.Map[Index].NPC.Length];
        Lists.Temp_Map[Index].Item = new System.Collections.Generic.List<Lists.Structures.Map_Items>();
        for (byte i = 1; i < Lists.Temp_Map[Index].NPC.Length; i++) global::NPC.Spawn(i, Index);
        Lists.Temp_Map[Index].Item.Add(new Lists.Structures.Map_Items());
        global::Map.Spawn_Items(Index);
    }

    public static void NPCs()
    {
        // Lê os dados
        Lists.NPC = new Lists.Structures.NPC[Lists.Server_Data.Num_NPCs + 1];
        for (byte i = 1; i < Lists.NPC.Length; i++) NPC(i);
    }

    public static void NPC(byte Index)
    {
        FileInfo File = new FileInfo(Directories.NPCs.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Clear.NPC(Index);
            Write.NPC(Index);
            return;
        }

        // Lê os dados
        FileStream Stream = File.OpenRead();
        Lists.NPC[Index] = (Lists.Structures.NPC)new BinaryFormatter().Deserialize(Stream);
        Stream.Close();
    }

    public static void Tiles()
    {
        // Lê os dados
        Lists.Tile = new Lists.Structures.Tile[Lists.Server_Data.Num_Tiles + 1];
        for (byte i = 1; i < Lists.Tile.Length; i++) Tile(i);
    }

    public static void Tile(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Tiles.FullName + Index + Directories.Format);

        // Evita erros
        if (!File.Exists)
        {
            Clear.Tile(Index);
            Write.Tile(Index);
            return;
        }

        // Lê os dados
        FileStream Stream = File.OpenRead();
        Lists.Tile[Index] = (Lists.Structures.Tile)new BinaryFormatter().Deserialize(Stream);
        Stream.Close();
    }
}