using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Objects;
using static Utils;

static class Read
{
    public static void All()
    {
        // Carrega todos os dados
        Console.WriteLine("Loading settings.");
        Settings();
        Console.WriteLine("Loading maps.");
        Maps();
        Console.WriteLine("Loading classes.");
        Classes();
        Console.WriteLine("Loading NPCs.");
        NPCs();
        Console.WriteLine("Loading items.");
        Items();
        Console.WriteLine("Loading shops.");
        Shops();
    }

    private static void Settings()
    {
        // Cria o arquivo caso ele não existir
        if (!Directories.Settings.Exists)
        {
            Write.Settings();
            return;
        }

        // Cria um arquivo temporário
        BinaryReader Data = new BinaryReader(Directories.Settings.OpenRead());

        // Carrega os dados
        Game_Name = Data.ReadString();
        Welcome_Message = Data.ReadString();
        Port = Data.ReadInt16();
        Max_Players = Data.ReadByte();
        Max_Characters = Data.ReadByte();
        Max_Party_Members = Data.ReadByte();
        Max_Map_Items = Data.ReadByte();
        Num_Points = Data.ReadByte();
        Max_Name_Length = Data.ReadByte();
        Min_Name_Length = Data.ReadByte();
        Max_Password_Length = Data.ReadByte();
        Min_Password_Length = Data.ReadByte();

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Account(Account Account, string Name)
    {
        FileInfo File = new FileInfo(Directories.Accounts.FullName + Name + "\\Data" + Directories.Format);

        // Cria um arquivo temporário
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados e os adiciona ao cache
        Account.User = Data.ReadString();
        Account.Password = Data.ReadString();
        Account.Acess = (Accesses)Data.ReadByte();

        // Descarrega o arquivo
        Data.Dispose();
    }

    public static void Characters(Account Account)
    {
        DirectoryInfo Directory = new DirectoryInfo(Directories.Accounts.FullName + Account.User.ToString() + "\\Characters");

        // Previne erros
        if (!Directory.Exists) Directory.Create();

        // Lê odos os personagens
        FileInfo[] File = Directory.GetFiles();
        Account.Characters = new List<Account.TempCharacter>();
        for (byte i = 0; i < File.Length; i++)
        {
            // Cria um arquivo temporário
            BinaryReader Data = new BinaryReader(File[i].OpenRead());

            // Carrega os dados e os adiciona ao cache
            var Temp = new Account.TempCharacter
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

    public static void Character(Account Account, string Name)
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
        Account.Character.Class = (Class)Lists.GetData(Lists.Class, new Guid(Data.ReadString()));
        Account.Character.Genre = Data.ReadBoolean();
        Account.Character.Experience = Data.ReadInt32();
        Account.Character.Points = Data.ReadByte();
        Account.Character.Map = (TMap)Lists.GetData(Lists.Temp_Map, new Guid(Data.ReadString()));
        Account.Character.X = Data.ReadByte();
        Account.Character.Y = Data.ReadByte();
        Account.Character.Direction = (Directions)Data.ReadByte();
        for (byte n = 0; n < (byte)Vitals.Count; n++) Account.Character.Vital[n] = Data.ReadInt16();
        for (byte n = 0; n < (byte)Attributes.Count; n++) Account.Character.Attribute[n] = Data.ReadInt16();
        for (byte n = 1; n <= Max_Inventory; n++)
        {
            Account.Character.Inventory[n].Item = (Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
            Account.Character.Inventory[n].Amount = Data.ReadInt16();
        }
        for (byte n = 0; n < (byte)Equipments.Count; n++) Account.Character.Equipment[n] = (Item)Lists.GetData(Lists.Item, new Guid(Data.ReadString()));
        for (byte n = 0; n < Max_Hotbar; n++) Account.Character.Hotbar[n] = new Hotbar((Hotbars)Data.ReadByte(), Data.ReadByte());

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
        Lists.Class = new Dictionary<Guid, Class>();
        FileInfo[] File = Directories.Classes.GetFiles();

        // Lê os dados
        if (File.Length > 0)
            for (byte i = 0; i < File.Length; i++)
            {
                FileStream Stream = File[i].OpenRead();
                Lists.Class.Add(new Guid(File[i].Name.Remove(36)), (Class)new BinaryFormatter().Deserialize(Stream));
                Stream.Close();
            }
        // Cria uma classe caso não houver nenhuma
        else
        {
            Class Class = new Class(Guid.NewGuid());
            Class.Name = "New class";
            Class.Spawn_Map = Lists.Map.ElementAt(0).Value;
            Lists.Class.Add(Class.ID, Class);
            Write.Class(Class);
        }
    }

    private static void Items()
    {
        // Lê os dados
        Lists.Item = new Dictionary<Guid, Item>();
        FileInfo[] File = Directories.Items.GetFiles();
        for (byte i = 0; i < File.Length; i++)
        {
            FileStream Stream = File[i].OpenRead();
            Lists.Item.Add(new Guid(File[i].Name.Remove(36)), (Item)new BinaryFormatter().Deserialize(Stream));
            Stream.Close();
        };
    }

    private static void Maps()
    {
        // Lê os dados
        Lists.Map = new Dictionary<Guid, Map>();
        FileInfo[] File = Directories.Maps.GetFiles();

        // Lê os dados
        if (File.Length > 0)
        {
            for (byte i = 0; i < File.Length; i++)
            {
                FileStream Stream = File[i].OpenRead();
                Lists.Map.Add(new Guid(File[i].Name.Remove(36)), (Map)new BinaryFormatter().Deserialize(Stream));
                Stream.Close();
            };
        }
        // Cria um mapa novo caso não houver nenhuma
        else
        {
            // Cria um mapa novo
            Map Map = new Map(Guid.NewGuid());
            Lists.Map.Add(Map.ID, Map);

            // Dados do mapa
            Map.Name = "New map";
            Map.Layer = new Map_Layer[1];
            Map.Layer[0] = new Map_Layer();
            Map.Layer[0].Name = "Ground";

            // Escreve os dados
            Write.Map(Map);
        }
    }

    private static void NPCs()
    {
        // Lê os dados
        Lists.NPC = new Dictionary<Guid, NPC>();
        FileInfo[] File = Directories.NPCs.GetFiles();
        for (byte i = 0; i < File.Length; i++)
        {
            FileStream Stream = File[i].OpenRead();
            Lists.NPC.Add(new Guid(File[i].Name.Remove(36)), (NPC)new BinaryFormatter().Deserialize(Stream));
            Stream.Close();
        };
    }

    private static void Shops()
    {
        // Lê os dados
        Lists.Shop = new Dictionary<Guid, Shop>();
        FileInfo[] File = Directories.Shops.GetFiles();
        for (byte i = 0; i < File.Length; i++)
        {
            FileStream Stream = File[i].OpenRead();
            Lists.Shop.Add(new Guid(File[i].Name.Remove(36)), (Shop)new BinaryFormatter().Deserialize(Stream));
            Stream.Close();
        };
    }
}