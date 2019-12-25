using System.IO;

class Write
{
    public static void Player(byte Index)
    {
        string Directory = Directories.Accounts.FullName + Lists.Player[Index].User + Directories.Format;

        // Evita erros
        if (Lists.Player[Index].User == string.Empty) return;

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
            Data.Write(Lists.Player[Index].Character[i].Genre);
            Data.Write(Lists.Player[Index].Character[i].Level);
            Data.Write(Lists.Player[Index].Character[i].Experience);
            Data.Write(Lists.Player[Index].Character[i].Points);
            Data.Write(Lists.Player[Index].Character[i].Map);
            Data.Write(Lists.Player[Index].Character[i].X);
            Data.Write(Lists.Player[Index].Character[i].Y);
            Data.Write((byte)Lists.Player[Index].Character[i].Direction);
            for (byte n = 0; n < (byte)Game.Vitals.Amount; n++) Data.Write(Lists.Player[Index].Character[i].Vital[n]);
            for (byte n = 0; n < (byte)Game.Attributes.Amount; n++) Data.Write(Lists.Player[Index].Character[i].Attribute[n]);
            for (byte n = 1; n <= Game.Max_Inventory; n++)
            {
                Data.Write(Lists.Player[Index].Character[i].Inventory[n].Item_Num);
                Data.Write(Lists.Player[Index].Character[i].Inventory[n].Amount);
            }
            for (byte n = 0; n < (byte)Game.Equipments.Amount ; n++) Data.Write(Lists.Player[Index].Character[i].Equipment[n]);
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
        // Cria um sistema binário para a manipulação dos dados
        BinaryWriter Data = new BinaryWriter(Directories.Server_Data.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Server_Data.Game_Name);
        Data.Write(Lists.Server_Data.Welcome);
        Data.Write(Lists.Server_Data.Port);
        Data.Write(Lists.Server_Data.Max_Players);
        Data.Write(Lists.Server_Data.Max_Characters);
        Data.Write(Lists.Server_Data.Num_Classes);
        Data.Write(Lists.Server_Data.Num_Tiles);
        Data.Write(Lists.Server_Data.Num_Maps);
        Data.Write(Lists.Server_Data.Num_NPCs);
        Data.Write(Lists.Server_Data.Num_Items);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Classes()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.Class.Length; Index++)
            Class(Index);
    }

    public static void Class(byte Index)
    {
        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.Classes.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Class[Index].Name);
        Data.Write(Lists.Class[Index].Texture_Male);
        Data.Write(Lists.Class[Index].Texture_Female);
        Data.Write(Lists.Class[Index].Spawn_Map);
        Data.Write(Lists.Class[Index].Spawn_Direction);
        Data.Write(Lists.Class[Index].Spawn_X);
        Data.Write(Lists.Class[Index].Spawn_Y);
        for (byte i = 0; i < (byte)Game.Vitals.Amount; i++) Data.Write(Lists.Class[Index].Vital[i]);
        for (byte i = 0; i < (byte)Game.Attributes.Amount; i++) Data.Write(Lists.Class[Index].Attribute[i]);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void NPCs()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.NPC.Length; Index++)
            NPC(Index);
    }

    public static void NPC(byte Index)
    {
        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.NPCs.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.NPC[Index].Name);
        Data.Write(Lists.NPC[Index].Texture);
        Data.Write(Lists.NPC[Index].Behaviour);
        Data.Write(Lists.NPC[Index].SpawnTime);
        Data.Write(Lists.NPC[Index].Sight);
        Data.Write(Lists.NPC[Index].Experience);
        for (byte i = 0; i < (byte)Game.Vitals.Amount; i++) Data.Write(Lists.NPC[Index].Vital[i]);
        for (byte i = 0; i < (byte)Game.Attributes.Amount; i++) Data.Write(Lists.NPC[Index].Attribute[i]);
        for (byte i = 0; i < Game.Max_NPC_Drop; i++)
        {
            Data.Write(Lists.NPC[Index].Drop[i].Item_Num);
            Data.Write(Lists.NPC[Index].Drop[i].Amount);
            Data.Write(Lists.NPC[Index].Drop[i].Chance);
        }

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Items()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.Item.Length; Index++)
            Item(Index);
    }

    public static void Item(byte Index)
    {
        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.Items.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Item[Index].Name);
        Data.Write(Lists.Item[Index].Description);
        Data.Write(Lists.Item[Index].Texture);
        Data.Write(Lists.Item[Index].Type);
        Data.Write(Lists.Item[Index].Price);
        Data.Write(Lists.Item[Index].Stackable);
        Data.Write(Lists.Item[Index].Bind);
        Data.Write(Lists.Item[Index].Req_Level);
        Data.Write(Lists.Item[Index].Req_Class);
        Data.Write(Lists.Item[Index].Potion_Experience);
        for (byte i = 0; i < (byte)Game.Vitals.Amount; i++) Data.Write(Lists.Item[Index].Potion_Vital[i]);
        Data.Write(Lists.Item[Index].Equip_Type);
        for (byte i = 0; i < (byte)Game.Attributes.Amount; i++) Data.Write(Lists.Item[Index].Equip_Attribute[i]);
        Data.Write(Lists.Item[Index].Weapon_Damage);

        // Fecha o sistema
        Data.Dispose();
    }
}