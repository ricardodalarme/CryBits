using System.Drawing;
using System.IO;

class Read
{
    public static void Options()
    {
        // Se o arquivo não existir, não é necessário carregá-lo
        if (!Directories.Options.Exists)
            Clear.Options();
        else
        {
            // Cria um sistema binário para a manipulação dos dados
            BinaryReader Data = new BinaryReader(Directories.Options.OpenRead());

            // Lê os dados
            Lists.Options.Directory_Client = Data.ReadString();
            Lists.Options.Directory_Server = Data.ReadString();
            Lists.Options.Pre_Map_Grid = Data.ReadBoolean();
            Lists.Options.Pre_Map_View = Data.ReadBoolean();
            Lists.Options.Pre_Map_Audio = Data.ReadBoolean();

            // Fecha o sistema
            Data.Dispose();
        }

        // Define e cria os diretórios
        Directories.SetClient();
        Directories.SetServer();

        // Atualiza os valores
        Editor_Maps.Objects.butGrid.Checked = Lists.Options.Pre_Map_Grid;
        Editor_Maps.Objects.butAudio.Checked = Lists.Options.Pre_Map_Audio;

        if (!Lists.Options.Pre_Map_View)
        {
            Editor_Maps.Objects.butVisualization.Checked = false;
            Editor_Maps.Objects.butEdition.Checked = true;
        }
    }

    public static void Client_Data()
    {
        // Limpa os dados
        Clear.Client_Data();

        // Se o arquivo não existir, não é necessário carregá-lo
        if (!Directories.Client_Data.Exists)
        {
            Write.Client_Data();
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(Directories.Client_Data.OpenRead());

        // Lê os dados
        Lists.Client_Data.Num_Buttons = Data.ReadByte();
        Lists.Client_Data.Num_TextBoxes = Data.ReadByte();
        Lists.Client_Data.Num_Panels = Data.ReadByte();
        Lists.Client_Data.Num_CheckBoxes = Data.ReadByte();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Server_Data()
    {
        // Limpa os dados
        Clear.Server_Data();

        // Se o arquivo não existir, não é necessário carregá-lo
        if (!Directories.Server_Data.Exists)
        {
            Write.Server_Data();
            return;
        }

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

    #region Server
    public static void Classes()
    {
        Lists.Class = new Lists.Structures.Class[Lists.Server_Data.Num_Classes + 1];

        // Limpa e lê os dados
        for (byte i = 1; i <= Lists.Class.GetUpperBound(0); i++)
        {
            Clear.Class(i);
            Class(i);
        }
    }

    public static void Class(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Classes_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Class(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Lists.Class[Index].Name = Data.ReadString();
        Lists.Class[Index].Texture_Male = Data.ReadInt16();
        Lists.Class[Index].Texture_Female = Data.ReadInt16();
        Lists.Class[Index].Spawn_Map = Data.ReadInt16();
        Lists.Class[Index].Spawn_Direction = Data.ReadByte();
        Lists.Class[Index].Spawn_X = Data.ReadByte();
        Lists.Class[Index].Spawn_Y = Data.ReadByte();
        for (byte i = 0; i <= (byte)Globals.Vitals.Amount - 1; i++) Lists.Class[Index].Vital[i] = Data.ReadInt16();
        for (byte i = 0; i <= (byte)Globals.Attributes.Amount - 1; i++) Lists.Class[Index].Attribute[i] = Data.ReadInt16();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Tiles()
    {
        Lists.Server_Data.Num_Tiles = (byte)Graphics.Tex_Tile.GetUpperBound(0);
        Lists.Tile = new Lists.Structures.Tile[Lists.Server_Data.Num_Tiles + 1];

        // Salva a quantidade dos azulejos
        Write.Server_Data();

        // Limpa e lê os dados
        for (byte i = 1; i <= Lists.Tile.GetUpperBound(0); i++)
        {
            Clear.Tile(i);
            Tile(i);
        }
    }

    public static void Tile(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Tile_Data.FullName + Index + Directories.Format);
        Size Texture_Size = Graphics.TSize(Graphics.Tex_Tile[Index]);
        Size Size = new Size(Texture_Size.Width / Globals.Grid - 1, Texture_Size.Height / Globals.Grid - 1);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Tile(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Dados básicos
        Lists.Tile[Index].Width = Data.ReadByte();
        Lists.Tile[Index].Height = Data.ReadByte();

        // Previne erros (Foi trocado de azulejo)
        if (Size != new Size(Lists.Tile[Index].Width, Lists.Tile[Index].Height))
        {
            Data.Dispose();
            Clear.Tile(Index);
            Write.Tile(Index);
            return;
        }

        for (byte x = 0; x <= Size.Width; x++)
            for (byte y = 0; y <= Size.Height; y++)
            {
                // Atributos
                Lists.Tile[Index].Data[x, y].Attribute = Data.ReadByte();

                // Bloqueio direcional
                for (byte i = 0; i <= (byte)Globals.Directions.Amount - 1; i++)
                    Lists.Tile[Index].Data[x, y].Block[i] = Data.ReadBoolean();
            }

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Maps()
    {
        Lists.Map = new Lists.Structures.Map[Lists.Server_Data.Num_Maps + 1];

        // Limpa e lê os dados
        for (short i = 1; i <= Lists.Map.GetUpperBound(0); i++)
        {
            Clear.Map(i);
            Map(i);
        }
    }

    public static void Map(short Index)
    {
        FileInfo File = new FileInfo(Directories.Maps_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Map(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

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
        for (short i = 0; i <= (short)Globals.Directions.Amount - 1; i++)
            Lists.Map[Index].Link[i] = Data.ReadInt16();

        // Quantidade de camadas
        byte Num_Layers = Data.ReadByte();
        Lists.Map[Index].Layer = new System.Collections.Generic.List<Lists.Structures.Map_Layer>();

        // Camadas
        for (byte i = 0; i <= Num_Layers; i++)
        {
            // Dados básicos
            Lists.Map[Index].Layer.Add(new Lists.Structures.Map_Layer());
            Lists.Map[Index].Layer[i].Name = Data.ReadString();
            Lists.Map[Index].Layer[i].Type = Data.ReadByte();

            // Redimensiona os azulejos
            Lists.Map[Index].Layer[i].Tile = new Lists.Structures.Map_Tile_Data[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];

            // Azulejos
            for (byte x = 0; x <= Lists.Map[Index].Width; x++)
                for (byte y = 0; y <= Lists.Map[Index].Height; y++)
                {
                    Lists.Map[Index].Layer[i].Tile[x, y].x = Data.ReadByte();
                    Lists.Map[Index].Layer[i].Tile[x, y].y = Data.ReadByte();
                    Lists.Map[Index].Layer[i].Tile[x, y].Tile = Data.ReadByte();
                    Lists.Map[Index].Layer[i].Tile[x, y].Auto = Data.ReadBoolean();
                    Lists.Map[Index].Layer[i].Tile[x, y].Mini = new Point[4];
                }
        }

        // Dados específicos dos azulejos
        Lists.Map[Index].Tile = new Lists.Structures.Map_Tile[Lists.Map[Index].Width + 1, Lists.Map[Index].Height + 1];
        for (byte x = 0; x <= Lists.Map[Index].Width; x++)
            for (byte y = 0; y <= Lists.Map[Index].Height; y++)
            {
                Lists.Map[Index].Tile[x, y].Attribute = Data.ReadByte();
                Lists.Map[Index].Tile[x, y].Data_1 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_2 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_3 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Data_4 = Data.ReadInt16();
                Lists.Map[Index].Tile[x, y].Zone = Data.ReadByte();
                Lists.Map[Index].Tile[x, y].Block = new bool[(byte)Globals.Directions.Amount];

                for (byte i = 0; i <= (byte)Globals.Directions.Amount - 1; i++)
                    Lists.Map[Index].Tile[x, y].Block[i] = Data.ReadBoolean();
            }

        // Luzes
        byte Num_Lights = Data.ReadByte();
        Lists.Map[Index].Light = new System.Collections.Generic.List<Lists.Structures.Map_Light>();
        if (Num_Lights > 0)
            for (byte i = 0; i <= Num_Lights - 1; i++)
                Lists.Map[Index].Light.Add(new Lists.Structures.Map_Light(new Rectangle(Data.ReadByte(), Data.ReadByte(), Data.ReadByte(), Data.ReadByte())));

        // NPCs
        byte Num_NPCs = Data.ReadByte();
        Lists.Map[Index].NPC = new System.Collections.Generic.List<Lists.Structures.Map_NPC>();
        Lists.Structures.Map_NPC NPC = new Lists.Structures.Map_NPC();
        if (Num_NPCs > 0)
            for (byte i = 0; i <= Num_NPCs - 1; i++)
            {
                NPC.Index = Data.ReadInt16();
                NPC.Zone = Data.ReadByte();
                NPC.Spawn = Data.ReadBoolean();
                NPC.X = Data.ReadByte();
                NPC.Y = Data.ReadByte();
                Lists.Map[Index].NPC.Add(NPC);
            }

        // Fecha o sistema
        Data.Dispose();
    }

    public static void NPCs()
    {
        Lists.NPC = new Lists.Structures.NPC[Lists.Server_Data.Num_NPCs + 1];

        // Limpa e lê os dados
        for (byte i = 1; i <= Lists.NPC.GetUpperBound(0); i++)
        {
            Clear.NPC(i);
            NPC(i);
        }
    }

    public static void NPC(byte Index)
    {
        FileInfo File = new FileInfo(Directories.NPCs_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.NPC(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Lists.NPC[Index].Name = Data.ReadString();
        Lists.NPC[Index].Texture = Data.ReadInt16();
        Lists.NPC[Index].Behaviour = Data.ReadByte();
        Lists.NPC[Index].SpawnTime = Data.ReadByte();
        Lists.NPC[Index].Sight = Data.ReadByte();
        Lists.NPC[Index].Experience = Data.ReadByte();
        for (byte i = 0; i <= (byte)Globals.Vitals.Amount - 1; i++) Lists.NPC[Index].Vital[i] = Data.ReadInt16();
        for (byte i = 0; i <= (byte)Globals.Attributes.Amount - 1; i++) Lists.NPC[Index].Attribute[i] = Data.ReadInt16();
        for (byte i = 0; i <= Globals.Max_NPC_Drop - 1; i++)
        {
            Lists.NPC[Index].Drop[i].Item_Num = Data.ReadInt16();
            Lists.NPC[Index].Drop[i].Amount = Data.ReadInt16();
            Lists.NPC[Index].Drop[i].Chance = Data.ReadByte();
        }

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Items()
    {
        Lists.Item = new Lists.Structures.Item[Lists.Server_Data.Num_Items + 1];

        // Limpa e lê os dados
        for (byte i = 1; i <= Lists.Item.GetUpperBound(0); i++)
        {
            Clear.Item(i);
            Item(i);
        }
    }

    public static void Item(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Items_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Item(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

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
        for (byte i = 0; i <= (byte)Globals.Vitals.Amount - 1; i++) Lists.Item[Index].Potion_Vital[i] = Data.ReadInt16();
        Lists.Item[Index].Equip_Type = Data.ReadByte();
        for (byte i = 0; i <= (byte)Globals.Attributes.Amount - 1; i++) Lists.Item[Index].Equip_Attribute[i] = Data.ReadInt16();
        Lists.Item[Index].Weapon_Damage = Data.ReadInt16();

        // Fecha o sistema
        Data.Dispose();
    }
    #endregion

    #region Client
    public static void Tools()
    {
        // Lê todas as ferramentas
        Buttons();
        TextBoxes();
        Panels();
        CheckBoxes();
    }

    public static void Buttons()
    {
        Lists.Button = new Lists.Structures.Button[Lists.Client_Data.Num_Buttons + 1];

        // Limpa e lê os dados
        for (byte i = 1; i <= Lists.Button.GetUpperBound(0); i++)
        {
            Clear.Button(i);
            Button(i);
        }
    }

    public static void Button(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Buttons_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Button(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Lists.Button[Index].General.Name = Data.ReadString();
        Lists.Button[Index].General.Position.X = Data.ReadInt32();
        Lists.Button[Index].General.Position.Y = Data.ReadInt32();
        Lists.Button[Index].General.Visible = Data.ReadBoolean();
        Lists.Button[Index].Texture = Data.ReadByte();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void TextBoxes()
    {
        Lists.TextBox = new Lists.Structures.TextBox[Lists.Client_Data.Num_TextBoxes + 1];

        // Limpa e lê os dados
        for (byte i = 1; i <= Lists.TextBox.GetUpperBound(0); i++)
        {
            Clear.TextBox(i);
            TextBox(i);
        }
    }

    public static void TextBox(byte Index)
    {
        FileInfo File = new FileInfo(Directories.TextBoxes_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.TextBox(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Lists.TextBox[Index].General.Name = Data.ReadString();
        Lists.TextBox[Index].General.Position.X = Data.ReadInt32();
        Lists.TextBox[Index].General.Position.Y = Data.ReadInt32();
        Lists.TextBox[Index].General.Visible = Data.ReadBoolean();
        Lists.TextBox[Index].Max_Chars = Data.ReadInt16();
        Lists.TextBox[Index].Width = Data.ReadInt16();
        Lists.TextBox[Index].Password = Data.ReadBoolean();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Panels()
    {
        Lists.Panel = new Lists.Structures.Panel[Lists.Client_Data.Num_Panels + 1];

        // Limpa e lê os dados
        for (byte i = 1; i <= Lists.Panel.GetUpperBound(0); i++)
        {
            Clear.Panel(i);
            Panel(i);
        }
    }

    public static void Panel(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Panels_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Panel(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados
        Lists.Panel[Index].General.Name = Data.ReadString();
        Lists.Panel[Index].General.Position.X = Data.ReadInt32();
        Lists.Panel[Index].General.Position.Y = Data.ReadInt32();
        Lists.Panel[Index].General.Visible = Data.ReadBoolean();
        Lists.Panel[Index].Texture = Data.ReadByte();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void CheckBoxes()
    {
        Lists.CheckBox = new Lists.Structures.CheckBox[Lists.Client_Data.Num_CheckBoxes + 1];

        // Limpa e lê os dados
        for (byte i = 1; i <= Lists.CheckBox.GetUpperBound(0); i++)
        {
            Clear.CheckBox(i);
            CheckBox(i);
        }
    }

    public static void CheckBox(byte Index)
    {
        FileInfo File = new FileInfo(Directories.CheckBoxes_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.CheckBox(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados
        Lists.CheckBox[Index].General.Name = Data.ReadString();
        Lists.CheckBox[Index].General.Position.X = Data.ReadInt32();
        Lists.CheckBox[Index].General.Position.Y = Data.ReadInt32();
        Lists.CheckBox[Index].General.Visible = Data.ReadBoolean();
        Lists.CheckBox[Index].Text = Data.ReadString();
        Lists.CheckBox[Index].State = Data.ReadBoolean();

        // Fecha o sistema
        Data.Dispose();
    }
    #endregion
}