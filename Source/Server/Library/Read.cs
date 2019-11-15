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
        Data.ReadByte();
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
}