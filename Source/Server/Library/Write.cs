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
            for (byte n = 0; n < (byte)Game.Equipments.Amount; n++) Data.Write(Lists.Player[Index].Character[i].Equipment[n]);
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
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Classes.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Class[Index]);
        Stream.Close();
    }

    public static void NPCs()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.NPC.Length; Index++)  NPC(Index);
    }

    public static void NPC(byte Index)
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
        for (short i = 1; i < Lists.Map.Length; i++)
            Map(i);
    }

    public static void Map(short Index)
    {
        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.Maps.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write((short)(Lists.Map[Index].Revision + 1));
        Data.Write(Lists.Map[Index].Name);
        Data.Write(Lists.Map[Index].Width);
        Data.Write(Lists.Map[Index].Height);
        Data.Write(Lists.Map[Index].Moral);
        Data.Write(Lists.Map[Index].Panorama);
        Data.Write(Lists.Map[Index].Music);
        Data.Write(Lists.Map[Index].Color);
        Data.Write(Lists.Map[Index].Weather.Type);
        Data.Write(Lists.Map[Index].Weather.Intensity);
        Data.Write(Lists.Map[Index].Fog.Texture);
        Data.Write(Lists.Map[Index].Fog.Speed_X);
        Data.Write(Lists.Map[Index].Fog.Speed_Y);
        Data.Write(Lists.Map[Index].Fog.Alpha);
        Data.Write(Lists.Map[Index].Light_Global);
        Data.Write(Lists.Map[Index].Lighting);

        // Ligações
        for (short i = 0; i < (short)Game.Directions.Count; i++)
            Data.Write(Lists.Map[Index].Link[i]);

        // Camadas
        Data.Write((byte)(Lists.Map[Index].Layer.Count - 1));
        for (byte i = 0; i < Lists.Map[Index].Layer.Count; i++)
        {
            Data.Write(Lists.Map[Index].Layer[i].Name);
            Data.Write(Lists.Map[Index].Layer[i].Type);

            // Azulejos
            for (byte x = 0; x <= Lists.Map[Index].Width; x++)
                for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                {
                    Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].X);
                    Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].Y);
                    Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].Tile);
                    Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].Auto);
                }
        }


        // Dados específicos dos azulejos
        for (byte x = 0; x <= Lists.Map[Index].Width; x++)
            for (byte y = 0; y <= Lists.Map[Index].Height; y++)
            {
                Data.Write(Lists.Map[Index].Tile[x, y].Attribute);
                Data.Write(Lists.Map[Index].Tile[x, y].Data_1);
                Data.Write(Lists.Map[Index].Tile[x, y].Data_2);
                Data.Write(Lists.Map[Index].Tile[x, y].Data_3);
                Data.Write(Lists.Map[Index].Tile[x, y].Data_4);
                Data.Write(Lists.Map[Index].Tile[x, y].Zone);

                // Bloqueio direcional
                for (byte i = 0; i < (byte)Game.Directions.Count; i++)
                    Data.Write(Lists.Map[Index].Tile[x, y].Block[i]);
            }

        // Luzes
        Data.Write((byte)Lists.Map[Index].Light.Length);
        for (byte i = 0; i < Lists.Map[Index].Light.Length; i++)
        {
            Data.Write(Lists.Map[Index].Light[i].X);
            Data.Write(Lists.Map[Index].Light[i].Y);
            Data.Write(Lists.Map[Index].Light[i].Width);
            Data.Write(Lists.Map[Index].Light[i].Height);
        }

        // NPCs
        Data.Write((byte)Lists.Map[Index].NPC.Length);
        for (byte i = 0; i < Lists.Map[Index].NPC.Length; i++)
        {
            Data.Write(Lists.Map[Index].NPC[i].Index);
            Data.Write(Lists.Map[Index].NPC[i].Zone);
            Data.Write(Lists.Map[Index].NPC[i].Spawn);
            Data.Write(Lists.Map[Index].NPC[i].X);
            Data.Write(Lists.Map[Index].NPC[i].Y);
        }

        // Fecha o sistema
        Data.Dispose();
    }
}