using System.IO;

class Read
{
    public static void Data()
    {
        // Carrega todos os dados
        Client_Data();
        Buttons_Data();
        TextBoxes_Data();
        Panels_Data();
        CheckBoxes_Data();
        Options();
    }

    public static void Options()
    {
        // Cria o arquivo se ele não existir
        if (!Directories.Options.Exists)
            Clear.Options();
        else
        {
            // Cria um arquivo temporário
            BinaryReader File = new BinaryReader(System.IO.File.OpenRead(Directories.Options.FullName));

            // Carrega os dados
            Lists.Options.GameName = File.ReadString();
            Lists.Options.SaveUsername = File.ReadBoolean();
            Lists.Options.Sounds = File.ReadBoolean();
            Lists.Options.Musics = File.ReadBoolean();
            Lists.Options.Username = File.ReadString();

            // Descarrega o arquivo
            File.Dispose();
        }

        // Adiciona os dados ao cache
        CheckBoxes.Get("Sons").State = Lists.Options.Sounds;
        CheckBoxes.Get("Músicas").State = Lists.Options.Musics;
        CheckBoxes.Get("SalvarUsuário").State = Lists.Options.SaveUsername;
        if (Lists.Options.SaveUsername) TextBoxes.Get("Conectar_Usuário").Text = Lists.Options.Username;
    }

    public static void Client_Data()
    {
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

    public static void Buttons_Data()
    {
        Buttons.List = new Buttons.Structure[Lists.Client_Data.Num_Buttons + 1];

        // Lê os dados
        for (byte i = 1; i < Buttons.List.Length; i++)
            Button_Data(i);
    }

    public static void Button_Data(byte Index)
    {
        // Limpa os valores
        Clear.Button(Index);

        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.Buttons_Data.FullName + Index + Directories.Format);
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Buttons.List[Index].Name = Data.ReadString();
        Buttons.List[Index].Position.X = Data.ReadInt32();
        Buttons.List[Index].Position.Y = Data.ReadInt32();
        Buttons.List[Index].Visible = Data.ReadBoolean();
        Buttons.List[Index].Texture = Data.ReadByte();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void TextBoxes_Data()
    {
        TextBoxes.List = new TextBoxes.Structure[Lists.Client_Data.Num_TextBoxes + 1];

        // Lê os dados
        for (byte i = 1; i < TextBoxes.List.Length; i++)
            TextBox_Data(i);
    }

    public static void TextBox_Data(byte Index)
    {
        // Limpa os valores
        Clear.TextBox(Index);

        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.TextBoxes_Data.FullName + Index + Directories.Format);
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        TextBoxes.List[Index].Name = Data.ReadString();
        TextBoxes.List[Index].Position.X = Data.ReadInt32();
        TextBoxes.List[Index].Position.Y = Data.ReadInt32();
        TextBoxes.List[Index].Visible = Data.ReadBoolean();
        TextBoxes.List[Index].Lenght = Data.ReadInt16();
        TextBoxes.List[Index].Width = Data.ReadInt16();
        TextBoxes.List[Index].Password = Data.ReadBoolean();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Panels_Data()
    {
        Panels.List = new Panels.Structure[Lists.Client_Data.Num_Panels + 1];

        // Lê os dados
        for (byte i = 1; i < Panels.List.Length; i++)
            Panel_Data(i);
    }

    public static void Panel_Data(byte Index)
    {
        // Limpa os valores
        Clear.Panel(Index);

        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.Panels_Data.FullName + Index + Directories.Format);
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados
        Panels.List[Index].Name = Data.ReadString();
        Panels.List[Index].Position.X = Data.ReadInt32();
        Panels.List[Index].Position.Y = Data.ReadInt32();
        Panels.List[Index].Visible = Data.ReadBoolean();
        Panels.List[Index].Texture = Data.ReadByte();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void CheckBoxes_Data()
    {
        CheckBoxes.List = new CheckBoxes.Structure[Lists.Client_Data.Num_CheckBoxes + 1];

        // Lê os dados
        for (byte i = 1; i < CheckBoxes.List.Length; i++)
            CheckBox_Data(i);
    }

    public static void CheckBox_Data(byte Index)
    {
        // Limpa os valores
        Clear.CheckBox(Index);

        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.CheckBoxes_Data.FullName + Index + Directories.Format);
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados
        CheckBoxes.List[Index].Name = Data.ReadString();
        CheckBoxes.List[Index].Position.X = Data.ReadInt32();
        CheckBoxes.List[Index].Position.Y = Data.ReadInt32();
        CheckBoxes.List[Index].Visible = Data.ReadBoolean();
        CheckBoxes.List[Index].Text = Data.ReadString();
        CheckBoxes.List[Index].State = Data.ReadBoolean();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Map(int Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.Maps_Data.FullName + Index + Directories.Format);
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Lists.Map.Revision = Data.ReadInt16();
        Lists.Map.Name = Data.ReadString();
        Lists.Map.Width = Data.ReadByte();
        Lists.Map.Height = Data.ReadByte();
        Lists.Map.Moral = Data.ReadByte();
        Lists.Map.Panorama = Data.ReadByte();
        Lists.Map.Music = Data.ReadByte();
        Lists.Map.Color = Data.ReadInt32();
        Lists.Map.Weather.Type = Data.ReadByte();
        Lists.Map.Weather.Intensity = Data.ReadByte();
        Lists.Map.Fog.Texture = Data.ReadByte();
        Lists.Map.Fog.Speed_X = Data.ReadSByte();
        Lists.Map.Fog.Speed_Y = Data.ReadSByte();
        Lists.Map.Fog.Alpha = Data.ReadByte();

        // Redimensiona as ligações
        Lists.Map.Link = new short[(byte)Game.Directions.Amount];
        for (short i = 0; i <= (short)Game.Directions.Amount - 1; i++)
            Lists.Map.Link[i] = Data.ReadInt16();

        // Redimensiona os azulejos 
        Lists.Map.Tile = new Lists.Structures.Map_Tile[Lists.Map.Width + 1, Lists.Map.Height + 1];

        // Lê os dados
        byte NumLayers = Data.ReadByte();
        for (byte x = 0; x <= Lists.Map.Width; x++)
            for (byte y = 0; y <= Lists.Map.Height; y++)
            {
                // Redimensiona os dados dos azulejos
                Lists.Map.Tile[x, y].Data = new Lists.Structures.Map_Tile_Data[(byte)global::Map.Layers.Amount, NumLayers + 1];

                for (byte c = 0; c <= (byte)global::Map.Layers.Amount - 1; c++)
                    for (byte q = 0; q <= NumLayers; q++)
                    {
                        Lists.Map.Tile[x, y].Data[c, q].X = Data.ReadByte();
                        Lists.Map.Tile[x, y].Data[c, q].Y = Data.ReadByte();
                        Lists.Map.Tile[x, y].Data[c, q].Tile = Data.ReadByte();
                        Lists.Map.Tile[x, y].Data[c, q].Automatic = Data.ReadBoolean();
                        Lists.Map.Tile[x, y].Data[c, q].Mini = new System.Drawing.Point[4];
                    }
            }

        // Dados específicos dos azulejos
        for (byte x = 0; x <= Lists.Map.Width; x++)
            for (byte y = 0; y <= Lists.Map.Height; y++)
            {
                Lists.Map.Tile[x, y].Attribute = Data.ReadByte();
                Lists.Map.Tile[x, y].Block = new bool[(byte)Game.Directions.Amount];
                for (byte i = 0; i <= (byte)Game.Directions.Amount - 1; i++)
                    Lists.Map.Tile[x, y].Block[i] = Data.ReadBoolean();
            }

        // Luzes
        Lists.Map.Light = new Lists.Structures.Map_Light[Data.ReadInt32() + 1];
        if (Lists.Map.Light.GetUpperBound(0) > 0)
            for (byte i = 0; i < Lists.Map.Light.Length; i++)
            {
                Lists.Map.Light[i].X = Data.ReadByte();
                Lists.Map.Light[i].Y = Data.ReadByte();
                Lists.Map.Light[i].Width = Data.ReadByte();
                Lists.Map.Light[i].Height = Data.ReadByte();
            }

        // NPCs
        Lists.Map.NPC = new short[Data.ReadInt32() + 1];
        Lists.Map.Temp_NPC = new Lists.Structures.Map_NPCs[Lists.Map.NPC.GetUpperBound(0) + 1];
        if (Lists.Map.NPC.GetUpperBound(0) > 0)
            for (byte i = 1; i < Lists.Map.NPC.Length; i++)
            {
                Lists.Map.NPC[i] = Data.ReadInt16();
                Lists.Map.Temp_NPC[i].Index = Lists.Map.NPC[i];
            }

        // Fecha o sistema
        Data.Dispose();

        // Redimensiona as partículas do clima
        global::Map.Weather_Update();
        global::Map.Autotile.Update();
    }
}