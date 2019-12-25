using System;
using System.IO;

partial class Read
{
    public static void All()
    {
        // Carrega todos os dados
        Server_Data();
        Console.WriteLine("Loaging data.");
        Classes();
        Console.WriteLine("Loaging classes.");
        NPCs();
        Console.WriteLine("Loaging NPCs.");
        Items();
        Console.WriteLine("Loaging items.");
        Maps();
        Console.WriteLine("Loaging maps.");
    }

    public static void Server_Data()
    {
        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(Directories.Server_Data.OpenRead());

        // Lê os dados
        Lists.Server_Data.Game_Name = Data.ReadString();
        Lists.Server_Data.Welcome = Data.ReadString();
        Lists.Server_Data.Port = Data.ReadInt16();
        Lists.Server_Data.Max_Players = Data.ReadByte();
        Lists.Server_Data.Max_Characters = Data.ReadByte();
        Lists.Server_Data.Num_Classes = Data.ReadByte();
        Lists.Server_Data.Num_Tiles = Data.ReadByte();
        Lists.Server_Data.Num_Maps = Data.ReadInt16();
        Lists.Server_Data.Num_NPCs = Data.ReadInt16();
        Lists.Server_Data.Num_Items = Data.ReadInt16();

        // Fecha o sistema
        Data.Dispose();
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
                Lists.Player[Index].Character[i].Genre = Data.ReadBoolean();
                Lists.Player[Index].Character[i].Level = Data.ReadInt16();
                Lists.Player[Index].Character[i].Experience = Data.ReadInt16();
                Lists.Player[Index].Character[i].Points = Data.ReadByte();
                Lists.Player[Index].Character[i].Map = Data.ReadInt16();
                Lists.Player[Index].Character[i].X = Data.ReadByte();
                Lists.Player[Index].Character[i].Y = Data.ReadByte();
                Lists.Player[Index].Character[i].Direction = (Game.Directions)Data.ReadByte();
                for (byte n = 0; n <= (byte)Game.Vitals.Amount - 1; n++) Lists.Player[Index].Character[i].Vital[n] = Data.ReadInt16();
                for (byte n = 0; n <= (byte)Game.Attributes.Amount - 1; n++) Lists.Player[Index].Character[i].Attribute[n] = Data.ReadInt16();
                for (byte n = 1; n <= Game.Max_Inventory; n++)
                {
                    Lists.Player[Index].Character[i].Inventory[n].Item_Num = Data.ReadInt16();
                    Lists.Player[Index].Character[i].Inventory[n].Amount = Data.ReadInt16();
                }
                for (byte n = 0; n <= (byte)Game.Equipments.Amount - 1; n++) Lists.Player[Index].Character[i].Equipment[n] = Data.ReadInt16();
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
        Lists.Class = new Lists.Structures.Classes[Lists.Server_Data.Num_Classes + 1];

        // Lê os dados
        for (byte i = 1; i <= Lists.Class.GetUpperBound(0); i++)
            Class(i);
    }

    public static void Class(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(new FileInfo(Directories.Classes.FullName + Index + Directories.Format).OpenRead());

        // Redimensiona os valores necessários 
        Lists.Class[Index].Vital = new short[(byte)Game.Vitals.Amount];
        Lists.Class[Index].Attribute = new short[(byte)Game.Attributes.Amount];

        // Lê os dados
        Lists.Class[Index].Name = Data.ReadString();
        Lists.Class[Index].Texture_Male = Data.ReadInt16();
        Lists.Class[Index].Texture_Female = Data.ReadInt16();
        Lists.Class[Index].Spawn_Map = Data.ReadInt16();
        Lists.Class[Index].Spawn_Direction = Data.ReadByte();
        Lists.Class[Index].Spawn_X = Data.ReadByte();
        Lists.Class[Index].Spawn_Y = Data.ReadByte();
        for (byte i = 0; i <= (byte)Game.Vitals.Amount - 1; i++) Lists.Class[Index].Vital[i] = Data.ReadInt16();
        for (byte i = 0; i <= (byte)Game.Attributes.Amount - 1; i++) Lists.Class[Index].Attribute[i] = Data.ReadInt16();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Items()
    {
        Lists.Item = new Lists.Structures.Items[Lists.Server_Data.Num_Items + 1];

        // Lê os dados
        for (byte i = 1; i <= Lists.Item.GetUpperBound(0); i++)
            Item(i);
    }

    public static void Item(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(new FileInfo(Directories.Items.FullName + Index + Directories.Format).OpenRead());

        // Redimensiona os valores necessários 
        Lists.Item[Index].Potion_Vital = new short[(byte)Game.Vitals.Amount];
        Lists.Item[Index].Equip_Attribute = new short[(byte)Game.Attributes.Amount];

        // Lê os dados
        Lists.Item[Index].Name = Data.ReadString();
        Lists.Item[Index].Description = Data.ReadString();
        Lists.Item[Index].Texture = Data.ReadInt16();
        Lists.Item[Index].Type = Data.ReadByte();
        Lists.Item[Index].Price = Data.ReadInt16();
        Lists.Item[Index].Stackable = Data.ReadBoolean();
        Lists.Item[Index].Bind = Data.ReadBoolean();
        Lists.Item[Index].Req_Level = Data.ReadInt16();
        Lists.Item[Index].Req_Class = Data.ReadByte();
        Lists.Item[Index].Potion_Experience = Data.ReadInt16();
        for (byte i = 0; i <= (byte)Game.Vitals.Amount - 1; i++) Lists.Item[Index].Potion_Vital[i] = Data.ReadInt16();
        Lists.Item[Index].Equip_Type = Data.ReadByte();
        for (byte i = 0; i <= (byte)Game.Attributes.Amount - 1; i++) Lists.Item[Index].Equip_Attribute[i] = Data.ReadInt16();
        Lists.Item[Index].Weapon_Damage = Data.ReadInt16();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Maps()
    {
        Lists.Map = new Lists.Structures.Maps[Lists.Server_Data.Num_Maps + 1];

        // Lê os dados
        for (short i = 1; i <= Lists.Map.GetUpperBound(0); i++)
            Map(i);
    }

    public static void Map(short Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(new FileInfo(Directories.Maps.FullName + Index + Directories.Format).OpenRead());

        // Lê os dados
        Lists.Map[Index].Revision = Data.ReadInt16();
        Lists.Map[Index].Name = Data.ReadString();
        Lists.Map[Index].Width = Data.ReadByte();
        Lists.Map[Index].Height = Data.ReadByte();
        Lists.Map[Index].Moral = Data.ReadByte();
        Lists.Map[Index].Panorama = Data.ReadByte();
        Lists.Map[Index].Music = Data.ReadByte();
        Lists.Map[Index].Color = Data.ReadInt32();
        Lists.Map[Index].Weather.Type = Data.ReadByte();
        Lists.Map[Index].Weather.Intensity = Data.ReadByte();
        Lists.Map[Index].Fog.Texture = Data.ReadByte();
        Lists.Map[Index].Fog.Speed_X = Data.ReadSByte();
        Lists.Map[Index].Fog.Speed_Y = Data.ReadSByte();
        Lists.Map[Index].Fog.Alpha = Data.ReadByte();
        Lists.Map[Index].Light_Global = Data.ReadByte();
        Lists.Map[Index].Lighting = Data.ReadByte();

        // Ligações
        Lists.Map[Index].Link = new short[(byte)Game.Directions.Amount];
        for (short i = 0; i <= (short)Game.Directions.Amount - 1; i++)
            Lists.Map[Index].Link[i] = Data.ReadInt16();

        // Azulejos
        Map_Tile(Index, Data);

        // Dados específicos dos azulejos
        for (byte x = 0; x <= Lists.Map[Index].Width; x++)
            for (byte y = 0; y <= Lists.Map[Index].Height; y++)
            {
                Lists.Map[Index].Tile[x, y].Attribute = Data.ReadByte();
                Lists.Map[Index].Tile[x, y].Data_1 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_2 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_3 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_4 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Zone = Data.ReadByte();

                // Bloqueio direcional
                Lists.Map[Index].Tile[x, y].Block = new bool[(byte)Game.Directions.Amount];
                for (byte i = 0; i <= (byte)Game.Directions.Amount - 1; i++)
                    Lists.Map[Index].Tile[x, y].Block[i] = Data.ReadBoolean();
            }

        // Luzes
        Lists.Map[Index].Light = new Lists.Structures.Map_Light[Data.ReadByte()];
        if (Lists.Map[Index].Light.GetUpperBound(0) > 0)
            for (byte i = 0; i <= Lists.Map[Index].Light.GetUpperBound(0); i++)
            {
                Lists.Map[Index].Light[i].X = Data.ReadByte();
                Lists.Map[Index].Light[i].Y = Data.ReadByte();
                Lists.Map[Index].Light[i].Width = Data.ReadByte();
                Lists.Map[Index].Light[i].Height = Data.ReadByte();
            }

        // NPCs
        Lists.Map[Index].NPC = new Lists.Structures.Map_NPC[Data.ReadByte() + 1];
        Lists.Map[Index].Temp_NPC = new Lists.Structures.Map_NPCs[Lists.Map[Index].NPC.GetUpperBound(0) + 1];
        if (Lists.Map[Index].NPC.GetUpperBound(0) > 0)
            for (byte i = 1; i <= Lists.Map[Index].NPC.GetUpperBound(0); i++)
            {
                Lists.Map[Index].NPC[i].Index = Data.ReadInt16();
                Lists.Map[Index].NPC[i].Zone = Data.ReadByte();
                Lists.Map[Index].NPC[i].Spawn = Data.ReadBoolean();
                Lists.Map[Index].NPC[i].X = Data.ReadByte();
                Lists.Map[Index].NPC[i].Y = Data.ReadByte();
                global::NPC.Spawn(i, Index);
            }

        // Items
        Lists.Map[Index].Temp_Item = new System.Collections.Generic.List<Lists.Structures.Map_Items>();
        Lists.Map[Index].Temp_Item.Add(new Lists.Structures.Map_Items()); // Nulo
        global::Map.Spawn_Items(Index);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Map_Tile(short Index, BinaryReader Binário)
    {
        byte Num_Layers = Binário.ReadByte();

        // Redimensiona os dados
        Lists.Map[Index].Tile = new Lists.Structures.Map_Tile[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];
        for (byte x = 0; x <= Lists.Map[Index].Width; x++)
            for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                Lists.Map[Index].Tile[x, y].Data = new Lists.Structures.Map_Tile_Data[(byte)global::Map.Layers.Amount, Num_Layers + 1];

        // Lê os azulejos
        for (byte i = 0; i <= Num_Layers; i++)
        {
            // Dados básicos
            Binário.ReadString(); // Name
            byte t = Binário.ReadByte(); // Tipo

            // Azulejos
            for (byte x = 0; x <= Lists.Map[Index].Width; x++)
                for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                {
                    Lists.Map[Index].Tile[x, y].Data[t, i].X = Binário.ReadByte();
                    Lists.Map[Index].Tile[x, y].Data[t, i].Y = Binário.ReadByte();
                    Lists.Map[Index].Tile[x, y].Data[t, i].Tile = Binário.ReadByte();
                    Lists.Map[Index].Tile[x, y].Data[t, i].Automatic = Binário.ReadBoolean();
                }
        }
    }

    public static void NPCs()
    {
        Lists.NPC = new Lists.Structures.NPCs[Lists.Server_Data.Num_NPCs + 1];

        // Lê os dados
        for (byte i = 1; i <= Lists.NPC.GetUpperBound(0); i++)
            NPC(i);
    }

    public static void NPC(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.NPCs.FullName + Index + Directories.Format);
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Redimensiona os valores necessários 
        Lists.NPC[Index].Vital = new short[(byte)Game.Vitals.Amount];
        Lists.NPC[Index].Attribute = new short[(byte)Game.Attributes.Amount];
        Lists.NPC[Index].Drop = new Lists.Structures.NPC_Drop[Game.Max_NPC_Drop];

        // Lê os dados
        Lists.NPC[Index].Name = Data.ReadString();
        Lists.NPC[Index].Texture = Data.ReadInt16();
        Lists.NPC[Index].Behaviour = Data.ReadByte();
        Lists.NPC[Index].SpawnTime = Data.ReadByte();
        Lists.NPC[Index].Sight = Data.ReadByte();
        Lists.NPC[Index].Experience = Data.ReadByte();
        for (byte i = 0; i <= (byte)Game.Vitals.Amount - 1; i++) Lists.NPC[Index].Vital[i] = Data.ReadInt16();
        for (byte i = 0; i <= (byte)Game.Attributes.Amount - 1; i++) Lists.NPC[Index].Attribute[i] = Data.ReadInt16();
        for (byte i = 0; i <= Game.Max_NPC_Drop - 1; i++)
        {
            Lists.NPC[Index].Drop[i].Item_Num = Data.ReadInt16();
            Lists.NPC[Index].Drop[i].Amount = Data.ReadInt16();
            Lists.NPC[Index].Drop[i].Chance = Data.ReadByte();
        }

        // Fecha o sistema
        Data.Dispose();
    }
}