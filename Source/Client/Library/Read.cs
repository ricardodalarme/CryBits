using System.Collections.Generic;
using System.IO;

class Read
{
    public static void Data()
    {
        // Carrega todos os dados
        Tools();
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
        CheckBoxes.Get("Sounds").State = Lists.Options.Sounds;
        CheckBoxes.Get("Musics").State = Lists.Options.Musics;
        CheckBoxes.Get("Connect_Save_Username").State = Lists.Options.SaveUsername;
        if (Lists.Options.SaveUsername) TextBoxes.Get("Connect_Username").Text = Lists.Options.Username;
    }

    public static Buttons.Structure Button(BinaryReader Data)
    {
        // Lê os dados
        Buttons.Structure Tool = new Buttons.Structure();
        Tool.Name = Data.ReadString();
        Tool.Position.X = Data.ReadInt32();
        Tool.Position.Y = Data.ReadInt32();
        Tool.Visible = Data.ReadBoolean();
        Tool.Window = (Tools.Windows)Data.ReadByte();
        Tool.Texture_Num = Data.ReadByte();
        return Tool;
    }

    public static TextBoxes.Structure TextBox(BinaryReader Data)
    {
        // Lê os dados
        TextBoxes.Structure Tool = new TextBoxes.Structure();
        Tool.Name = Data.ReadString();
        Tool.Position.X = Data.ReadInt32();
        Tool.Position.Y = Data.ReadInt32();
        Tool.Visible = Data.ReadBoolean();
        Tool.Window = (Tools.Windows)Data.ReadByte();
        Tool.Lenght = Data.ReadInt16();
        Tool.Width = Data.ReadInt16();
        Tool.Password = Data.ReadBoolean();
        return Tool;
    }

    public static Panels.Structure Panel(BinaryReader Data)
    {
        // Carrega os dados
        Panels.Structure Tool = new Panels.Structure();
        Tool.Name = Data.ReadString();
        Tool.Position.X = Data.ReadInt32();
        Tool.Position.Y = Data.ReadInt32();
        Tool.Visible = Data.ReadBoolean();
        Tool.Window = (Tools.Windows)Data.ReadByte();
        Tool.Texture_Num = Data.ReadByte();
        return Tool;
    }

    public static CheckBoxes.Structure CheckBox(BinaryReader Data)
    {
        // Carrega os dados
        CheckBoxes.Structure Tool = new CheckBoxes.Structure();
        Tool.Name = Data.ReadString();
        Tool.Position.X = Data.ReadInt32();
        Tool.Position.Y = Data.ReadInt32();
        Tool.Visible = Data.ReadBoolean();
        Tool.Window = (Tools.Windows)Data.ReadByte();
        Tool.Text = Data.ReadString();
        Tool.State = Data.ReadBoolean();
        return Tool;
    }

    public static void Tools()
    {
        FileInfo File = new FileInfo(Directories.Tools.FullName);
        for (byte i = 0; i < (byte)global::Tools.Windows.Count; i++) global::Tools.All_Order[i] = new List<Tools.Order_Structure>();

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê todos os nós
        for (byte n = 0; n < global::Tools.All_Order.Length; n++) Tools(null, ref global::Tools.All_Order[n], Data);

        // Fecha o sistema
        Data.Dispose();
    }

    private static void Tools(Tools.Order_Structure Parent, ref List<Tools.Order_Structure> Node, BinaryReader Data)
    {
        // Lê todos os filhos
        byte Size = Data.ReadByte();

        for (byte i = 0; i < Size; i++)
        {
            // Lê a ferramenta
            Tools.Structure Temp_Tool = new Tools.Structure();
            switch ((Tools.Types)Data.ReadByte())
            {
                case global::Tools.Types.Button: Temp_Tool = Button(Data); Buttons.List.Add((Buttons.Structure)Temp_Tool); break;
                case global::Tools.Types.TextBox: Temp_Tool = TextBox(Data); TextBoxes.List.Add((TextBoxes.Structure)Temp_Tool); break;
                case global::Tools.Types.Panel: Temp_Tool = Panel(Data); Panels.List.Add((Panels.Structure)Temp_Tool); break;
                case global::Tools.Types.CheckBox: Temp_Tool = CheckBox(Data); CheckBoxes.List.Add((CheckBoxes.Structure)Temp_Tool); break;
            }

            // Adiciona à lista
            Tools.Order_Structure Temp_Order = new Tools.Order_Structure();
            Temp_Order.Nodes = new List<Tools.Order_Structure>();
            Temp_Order.Parent = Parent;
            Temp_Order.Data = Temp_Tool;
            Node.Add(Temp_Order);

            // Pula pro próximo
            Tools(Node[i], ref Node[i].Nodes, Data);
        }
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
        Lists.Map.Link = new short[(byte)Game.Directions.Count];
        for (short i = 0; i < (short)Game.Directions.Count; i++)
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

                for (byte c = 0; c < (byte)global::Map.Layers.Amount; c++)
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
                Lists.Map.Tile[x, y].Block = new bool[(byte)Game.Directions.Count];
                for (byte i = 0; i < (byte)Game.Directions.Count; i++)
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