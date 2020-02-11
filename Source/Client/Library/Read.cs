using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

class Read
{
    public static void Data()
    {
        // Carrega todos os dados
        Tools();
        Options();
    }

    private static void Options()
    {
        // Cria o arquivo se ele não existir
        if (!Directories.Options.Exists)
            Clear.Options();
        else
        {
            // Cria um arquivo temporário
            BinaryReader File = new BinaryReader(System.IO.File.OpenRead(Directories.Options.FullName));

            // Carrega os dados
            Lists.Options.Game_Name = File.ReadString();
            Lists.Options.Username = File.ReadString();
            Lists.Options.SaveUsername = File.ReadBoolean();
            Lists.Options.Sounds = File.ReadBoolean();
            Lists.Options.Musics = File.ReadBoolean();
            Lists.Options.Chat = File.ReadBoolean();

            // Descarrega o arquivo
            File.Dispose();
        }

        // Adiciona os dados ao cache
        CheckBoxes.Get("Connect_Save_Username").Checked = Lists.Options.SaveUsername;
        if (Lists.Options.SaveUsername) TextBoxes.Get("Connect_Username").Text = Lists.Options.Username;
    }

    private static Buttons.Structure Button(BinaryReader Data)
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

    private static TextBoxes.Structure TextBox(BinaryReader Data)
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

    private static Panels.Structure Panel(BinaryReader Data)
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

    private static CheckBoxes.Structure CheckBox(BinaryReader Data)
    {
        // Carrega os dados
        CheckBoxes.Structure Tool = new CheckBoxes.Structure();
        Tool.Name = Data.ReadString();
        Tool.Position.X = Data.ReadInt32();
        Tool.Position.Y = Data.ReadInt32();
        Tool.Visible = Data.ReadBoolean();
        Tool.Window = (Tools.Windows)Data.ReadByte();
        Tool.Text = Data.ReadString();
        Tool.Checked = Data.ReadBoolean();
        return Tool;
    }

    private static void Tools()
    {
        FileInfo File = new FileInfo(Directories.Tools_Data.FullName);
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
        FileInfo File = new FileInfo(Directories.Maps_Data.FullName + Index + Directories.Format);

        // Lê os dados
        FileStream Stream = File.OpenRead();
        Lists.Map = (Lists.Structures.Maps)new BinaryFormatter().Deserialize(Stream);
        Stream.Close();

        // Redimensiona as partículas do clima
        global::Map.Weather_Update();
        global::Map.Autotile.Update();
    }
}