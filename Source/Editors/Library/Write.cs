using System.IO;
using System.Drawing;

class Write
{
    public static void Options()
    {
        // Cria um sistema binário para a manipulação dos dados
        BinaryWriter Data = new BinaryWriter(Directories.Options.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Options.Directory_Client);
        Data.Write(Lists.Options.Directory_Server);
        Data.Write(Lists.Options.Pre_Map_Grid);
        Data.Write(Lists.Options.Pre_Map_View);
        Data.Write(Lists.Options.Pre_Map_Audio);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Client_Data()
    {
        // Cria um sistema binário para a manipulação dos dados
        BinaryWriter Data = new BinaryWriter(Directories.Client_Data.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Client_Data.Num_Buttons);
        Data.Write(Lists.Client_Data.Num_TextBoxes);
        Data.Write(Lists.Client_Data.Num_Panels);
        Data.Write(Lists.Client_Data.Num_CheckBoxes);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Server_Data()
    {
        // Cria um sistema binário para a manipulação dos dados
        BinaryWriter Data = new BinaryWriter(Directories.Server_Data.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Server_Data.GameName);
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
        for (byte Index = 1; Index <= Lists.Class.GetUpperBound(0); Index++)
            Class(Index);
    }

    public static void Class(byte Index)
    {
        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.Classes_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Class[Index].Name);
        Data.Write(Lists.Class[Index].Texture_Male);
        Data.Write(Lists.Class[Index].Texture_Female);
        Data.Write(Lists.Class[Index].Spawn_Map);
        Data.Write(Lists.Class[Index].Spawn_Direction);
        Data.Write(Lists.Class[Index].Spawn_X);
        Data.Write(Lists.Class[Index].Spawn_Y);
        for (byte i = 0; i <= (byte)Globals.Vitals.Amount - 1; i++) Data.Write(Lists.Class[Index].Vital[i]);
        for (byte i = 0; i <= (byte)Globals.Attributes.Amount - 1; i++) Data.Write(Lists.Class[Index].Attribute[i]);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void NPCs()
    {
        // Escreve os dados
        for (byte Index = 1; Index <= Lists.NPC.GetUpperBound(0); Index++)
            NPC(Index);
    }

    public static void NPC(byte Index)
    {
        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.NPCs_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.NPC[Index].Name);
        Data.Write(Lists.NPC[Index].Texture);
        Data.Write(Lists.NPC[Index].Behaviour);
        Data.Write(Lists.NPC[Index].SpawnTime);
        Data.Write(Lists.NPC[Index].Sight);
        Data.Write(Lists.NPC[Index].Experience);
        for (byte i = 0; i <= (byte)Globals.Vitals.Amount - 1; i++) Data.Write(Lists.NPC[Index].Vital[i]);
        for (byte i = 0; i <= (byte)Globals.Attributes.Amount - 1; i++) Data.Write(Lists.NPC[Index].Attribute[i]);
        for (byte i = 0; i <= Globals.Max_NPC_Drop - 1; i++)
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
        for (byte Index = 1; Index <= Lists.Item.GetUpperBound(0); Index++)
            Item(Index);
    }

    public static void Item(byte Index)
    {
        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.Items_Data.FullName + Index + Directories.Format);
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
        for (byte i = 0; i <= (byte)Globals.Vitals.Amount - 1; i++) Data.Write(Lists.Item[Index].Potion_Vital[i]);
        Data.Write(Lists.Item[Index].Equip_Type);
        for (byte i = 0; i <= (byte)Globals.Attributes.Amount - 1; i++) Data.Write(Lists.Item[Index].Equip_Attribute[i]);
        Data.Write(Lists.Item[Index].Weapon_Damage);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Tiles()
    {
        // Escreve os dados
        for (byte i = 1; i <= Lists.Tile.GetUpperBound(0); i++)
            Tile(i);
    }

    public static void Tile(byte Index)
    {
        Size Size = new Size(Lists.Tile[Index].Data.GetUpperBound(0), Lists.Tile[Index].Data.GetUpperBound(1));

        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.Tile_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Dados básicos
        Data.Write((byte)Size.Width);
        Data.Write((byte)Size.Height);

        // Gerais
        for (byte x = 0; x <= Size.Width; x++)
            for (byte y = 0; y <= Size.Height; y++)
            {
                Data.Write(Lists.Tile[Index].Data[x, y].Attribute);

                // Bloqueio direcional
                for (byte i = 0; i <= (byte)Globals.Directions.Amount - 1; i++)
                    Data.Write(Lists.Tile[Index].Data[x, y].Block[i]);
            }

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Maps()
    {
        // Escreve os dados
        for (short Index = 1; Index <= Lists.Map.GetUpperBound(0); Index++)
            Map(Index);
    }

    public static void Map(short Index)
    {
        // Cria um arquivo temporário
        FileInfo File = new FileInfo(Directories.Maps_Data.FullName + Index + Directories.Format);
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
        for (short i = 0; i <= (short)Globals.Directions.Amount - 1; i++)
            Data.Write(Lists.Map[Index].Link[i]);

        // Camadas
        Data.Write((byte)(Lists.Map[Index].Layer.Count - 1));
        for (byte i = 0; i <= Lists.Map[Index].Layer.Count - 1; i++)
        {
            Data.Write(Lists.Map[Index].Layer[i].Name);
            Data.Write(Lists.Map[Index].Layer[i].Type);

            // Azulejos
            for (byte x = 0; x <= Lists.Map[Index].Width; x++)
                for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                {
                    Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].x);
                    Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].y);
                    Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].Tile);
                    Data.Write(Lists.Map[Index].Layer[i].Tile[x, y].Auto);
                }
        }


        // Lists.Map[Index] específicos dos azulejos
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
                for (byte i = 0; i <= (byte)Globals.Directions.Amount - 1; i++)
                    Data.Write(Lists.Map[Index].Tile[x, y].Block[i]);
            }

        // Luzes
        Data.Write((byte)Lists.Map[Index].Light.Count);
        if (Lists.Map[Index].Light.Count > 0)
            for (byte i = 0; i <= Lists.Map[Index].Light.Count - 1; i++)
            {
                Data.Write(Lists.Map[Index].Light[i].X);
                Data.Write(Lists.Map[Index].Light[i].Y);
                Data.Write(Lists.Map[Index].Light[i].Width);
                Data.Write(Lists.Map[Index].Light[i].Height);
            }

        // Itens
        Data.Write((byte)Lists.Map[Index].NPC.Count);
        if (Lists.Map[Index].NPC.Count > 0)
            for (byte i = 0; i <= Lists.Map[Index].NPC.Count - 1; i++)
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

    public static void Tools()
    {
        // Escreve os dados de todas as ferramentas
        Buttons();
        TextBoxes();
        Panels();
        CheckBoxes();
    }

    public static void Buttons()
    {
        // Escreve os dados
        for (byte Index = 1; Index <= Lists.Button.GetUpperBound(0); Index++)
            Button(Index);
    }

    public static void Button(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.Buttons_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Button[Index].General.Name);
        Data.Write(Lists.Button[Index].General.Position.X);
        Data.Write(Lists.Button[Index].General.Position.Y);
        Data.Write(Lists.Button[Index].General.Visible);
        Data.Write(Lists.Button[Index].Texture);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void TextBoxes()
    {
        // Escreve os dados
        for (byte Index = 1; Index <= Lists.TextBox.GetUpperBound(0); Index++)
            TextBox(Index);
    }

    public static void TextBox(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.TextBoxes_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.TextBox[Index].General.Name);
        Data.Write(Lists.TextBox[Index].General.Position.X);
        Data.Write(Lists.TextBox[Index].General.Position.Y);
        Data.Write(Lists.TextBox[Index].General.Visible);
        Data.Write(Lists.TextBox[Index].Max_Chars);
        Data.Write(Lists.TextBox[Index].Width);
        Data.Write(Lists.TextBox[Index].Password);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Panels()
    {
        // Escreve os dados
        for (byte Index = 1; Index <= Lists.Panel.GetUpperBound(0); Index++)
            Panel(Index);
    }

    public static void Panel(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.Panels_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Panel[Index].General.Name);
        Data.Write(Lists.Panel[Index].General.Position.X);
        Data.Write(Lists.Panel[Index].General.Position.Y);
        Data.Write(Lists.Panel[Index].General.Visible);
        Data.Write(Lists.Panel[Index].Texture);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void CheckBoxes()
    {
        // Escreve os dados
        for (byte Index = 1; Index <= Lists.CheckBox.GetUpperBound(0); Index++)
            CheckBox(Index);
    }

    public static void CheckBox(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.CheckBoxes_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.CheckBox[Index].General.Name);
        Data.Write(Lists.CheckBox[Index].General.Position.X);
        Data.Write(Lists.CheckBox[Index].General.Position.Y);
        Data.Write(Lists.CheckBox[Index].General.Visible);
        Data.Write(Lists.CheckBox[Index].Text);
        Data.Write(Lists.CheckBox[Index].State);

        // Fecha o sistema
        Data.Dispose();
    }
}