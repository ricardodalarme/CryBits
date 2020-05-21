﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

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
        Console.WriteLine("Loading shops.");
        Shops();
    }

    private static void Server_Data()
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

    public static void Account(Account.Structure Account, string Name)
    {
        FileInfo File = new FileInfo(Directories.Accounts.FullName + Name + "\\Data" + Directories.Format);

        // Cria um arquivo temporário
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados e os adiciona ao cache
        Account.User = Data.ReadString();
        Account.Password = Data.ReadString();
        Account.Acess = (Game.Accesses)Data.ReadByte();

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Characters(Account.Structure Account)
    {
        DirectoryInfo Directory = new DirectoryInfo(Directories.Accounts.FullName + Account.User.ToString() + "\\Characters");

        // Previne erros
        if (!Directory.Exists) Directory.Create();

        // Lê odos os personagens
        FileInfo[] File = Directory.GetFiles();
        Account.Characters = new System.Collections.Generic.List<Account.Structure.TempCharacter>();
        for (byte i = 0; i < File.Length; i++)
        {
            // Cria um arquivo temporário
            BinaryReader Data = new BinaryReader(File[i].OpenRead());

            // Carrega os dados e os adiciona ao cache
            Account.Structure.TempCharacter Temp = new Account.Structure.TempCharacter
            {
                Name = Data.ReadString(),
                Texture_Num = Data.ReadInt16(),
                Level = Data.ReadInt16()
            };
            Account.Characters.Add(Temp);

            // Descarrega o arquivo
            Data.Dispose();
        };
    }

    public static void Character(Account.Structure Account, string Name)
    {
        FileInfo File = new FileInfo(Directories.Accounts.FullName + Account.User + "\\Characters\\" + Name + Directories.Format);

        // Verifica se o diretório existe
        if (!File.Directory.Exists) return;

        // Cria um arquivo temporário
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados e os adiciona ao cache
        Account.Character = new Player(Account);
        Account.Character.Name = Data.ReadString();
        Account.Character.Texture_Num = Data.ReadInt16();
        Account.Character.Level = Data.ReadInt16();
        Account.Character.Class = Lists.Class.ElementAt(0).Value;
        Account.Character.Genre = Data.ReadBoolean();
        Account.Character.Experience = Data.ReadInt32();
        Account.Character.Points = Data.ReadByte();
        Account.Character.Map_Num = Data.ReadInt16();
        Account.Character.X = Data.ReadByte();
        Account.Character.Y = Data.ReadByte();
        Account.Character.Direction = (Game.Directions)Data.ReadByte();
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Account.Character.Vital[n] = Data.ReadInt16();
        for (byte n = 0; n < (byte)Game.Attributes.Count; n++) Account.Character.Attribute[n] = Data.ReadInt16();
        for (byte n = 1; n <= Game.Max_Inventory; n++)
        {
            Account.Character.Inventory[n].Item = (Objects.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
            Account.Character.Inventory[n].Amount = Data.ReadInt16();
        }
        for (byte n = 0; n < (byte)Game.Equipments.Count; n++) Account.Character.Equipment[n] = (Objects.Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
        for (byte n = 1; n <= Game.Max_Hotbar; n++)
        {
            Account.Character.Hotbar[n].Type = Data.ReadByte();
            Account.Character.Hotbar[n].Slot = Data.ReadByte();
        }

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static string Characters_Name()
    {
        // Cria o arquivo caso ele não existir
        if (!Directories.Characters.Exists)
        {
            Write.Characters_Name(string.Empty);
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

    private static void Classes()
    {
        Lists.Class = new Dictionary<Guid, Objects.Class>();
        FileInfo[] File = Directories.Classes.GetFiles();

        // Lê os dados
        if (File.Length > 0)
            for (byte i = 0; i < File.Length; i++)
            {
                FileStream Stream = File[i].OpenRead();
                Lists.Class.Add(new Guid(File[i].Name.Remove(36)), (Objects.Class)new BinaryFormatter().Deserialize(Stream));
                Stream.Close();
            }
        // Cria uma classe caso não houver nenhuma
        else
        {
            Objects.Class Class = new Objects.Class(Guid.NewGuid());
            Class.Tex_Male = Class.Tex_Female = new short[1];
            Class.Tex_Male[0] = Class.Tex_Female[0] = 1;
            Class.Spawn_Map = 1;
            Lists.Class.Add(Class.ID, Class);
            Write.Class(Class);
        }
    }

    private static void Items()
    {
        // Lê os dados
        Lists.Item = new Dictionary<Guid, Objects.Item>();
        FileInfo[] File = Directories.Items.GetFiles();
        for (byte i = 0; i < File.Length; i++)
        {
            FileStream Stream = File[i].OpenRead();
            Lists.Item.Add(new Guid(File[i].Name.Remove(36)), (Objects.Item)new BinaryFormatter().Deserialize(Stream));
            Stream.Close();
        };
    }

    private static void Maps()
    {
        // Lê os dados1
        Lists.Map = new Lists.Structures.Map[Lists.Server_Data.Num_Maps + 1];
        Lists.Temp_Map = new Lists.Structures.Temp_Map[Lists.Server_Data.Num_Maps + 1];
        for (short i = 1; i < Lists.Map.Length; i++) Map(i);
    }

    private static void Map(short Index)
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

        // NPCs do mapa
        Lists.Temp_Map[Index].NPC = new NPC.Structure[Lists.Map[Index].NPC.Length];
        for (byte i = 1; i < Lists.Temp_Map[Index].NPC.Length; i++)
        {
            Lists.Temp_Map[Index].NPC[i] = new NPC.Structure(i, Index, Lists.Map[Index].NPC[i].NPC);
            Lists.Temp_Map[Index].NPC[i].Spawn();
        }

        // Itens do mapa
        Lists.Temp_Map[Index].Item = new List<Lists.Structures.Map_Items>();
        Lists.Temp_Map[Index].Item.Add(new Lists.Structures.Map_Items());
        global::Map.Spawn_Items(Index);
    }

    private static void NPCs()
    {
        // Lê os dados
        Lists.NPC = new Dictionary<Guid, Objects.NPC>();
        FileInfo[] File = Directories.NPCs.GetFiles();
        for (byte i = 0; i < File.Length; i++)
        {
            FileStream Stream = File[i].OpenRead();
            Lists.NPC.Add(new Guid(File[i].Name.Remove(36)), (Objects.NPC)new BinaryFormatter().Deserialize(Stream));
            Stream.Close();
        };
    }

    private static void Shops()
    {
        // Lê os dados
        Lists.Shop = new Dictionary<Guid, Objects.Shop>();
        FileInfo[] File = Directories.Shops.GetFiles();
        for (byte i = 0; i < File.Length; i++)
        {
            FileStream Stream = File[i].OpenRead();
            Lists.Shop.Add(new Guid(File[i].Name.Remove(36)), (Objects.Shop)new BinaryFormatter().Deserialize(Stream));
            Stream.Close();
        };
    }
}