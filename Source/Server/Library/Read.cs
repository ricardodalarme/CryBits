using System;
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

    private static void Server_Data()
    {
        // Cria o arquivo caso ele não existir
        if (!Directories.Server_Data.Exists)
        {
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
        Account.Character = new Objects.Player(Account);
        Account.Character.Name = Data.ReadString();
        Account.Character.Texture_Num = Data.ReadInt16();
        Account.Character.Level = Data.ReadInt16();
        Account.Character.Class = (Objects.Class)Lists.GetData(Lists.Class, new Guid(Data.ReadString()));
        Account.Character.Genre = Data.ReadBoolean();
        Account.Character.Experience = Data.ReadInt32();
        Account.Character.Points = Data.ReadByte();
        Account.Character.Map = (Objects.TMap)Lists.GetData(Lists.Temp_Map, new Guid(Data.ReadString()));
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
            Class.Spawn_Map = Lists.Map.ElementAt(0).Value;
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
        // Lê os dados
        Lists.Map = new Dictionary<Guid, Objects.Map>();
        FileInfo[] File = Directories.Maps.GetFiles();

        // Lê os dados
        if (File.Length > 0)
        {
            for (byte i = 0; i < File.Length; i++)
            {
                FileStream Stream = File[i].OpenRead();
                Lists.Map.Add(new Guid(File[i].Name.Remove(36)), (Objects.Map)new BinaryFormatter().Deserialize(Stream));
                Stream.Close();
            };
        }
        // Cria um mapa novo caso não houver nenhuma
        else
        {
            // Cria um mapa novo
            Objects.Map Map = new Objects.Map(Guid.NewGuid());
            Lists.Map.Add(Map.ID, Map);
            
            // Dados do mapa
            Map.Width = Game.Min_Map_Width;
            Map.Height = Game.Min_Map_Height;
            Map.Layer.Add(new Objects.Map_Layer(Game.Min_Map_Width, Game.Min_Map_Height));
            Map.Layer[0].Name = "Ground";
            Map.Tile = new Objects.Map_Tile[Game.Min_Map_Width, Game.Min_Map_Height];
            Map.Tile = new Objects.Map_Tile[Map.Width + 1, Map.Height + 1];
            for (byte x = 0; x <= Map.Width; x++)
                for (byte y = 0; y <= Map.Height; y++)
                    Map.Tile[x, y] = new Objects.Map_Tile();

            // Escreve os dados
            Write.Map(Map);
        }
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