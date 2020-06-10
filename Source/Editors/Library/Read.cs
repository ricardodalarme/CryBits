using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

static class Read
{
    public static void Options()
    {
        // Lê os dados
        FileStream Stream = Directories.Options.OpenRead();
        Lists.Options = (Lists.Structures.Options)new BinaryFormatter().Deserialize(Stream);
        Stream.Close();

        // Define os dados
        Directories.SetClient();
        Login.Form.txtUsername.Text = Lists.Options.Username;
        Login.Form.chkUsername.Checked = Lists.Options.Username != string.Empty;
    }

    public static void Tools()
    {
        FileInfo File = new FileInfo(Directories.Tools.FullName);

        // Limpa a árvore de ordem
        Lists.Tool = new TreeNode();
        for (byte i = 0; i < Enum.GetValues(typeof(Globals.Windows)).Length; i++) Lists.Tool.Nodes.Add(((Globals.Windows)i).ToString());

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Tools();
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê todos os nós
        for (byte n = 0; n < Lists.Tool.Nodes.Count; n++) Tools(Lists.Tool.Nodes[n], Data);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Tools(TreeNode Node, BinaryReader Data)
    {
        // Lê todos os filhos
        byte Size = Data.ReadByte();
        for (byte i = 0; i < Size; i++)
        {
            Objects.Tool Temp = new Objects.Tool();
            Globals.Tools_Types Type = (Globals.Tools_Types)Data.ReadByte();

            // Lê a ferramenta
            if (Type == Globals.Tools_Types.Button) Temp = Button(Data);
            else if (Type == Globals.Tools_Types.CheckBox) Temp = CheckBox(Data);
            else if (Type == Globals.Tools_Types.Panel) Temp = Panel(Data);
            else if (Type == Globals.Tools_Types.TextBox) Temp = TextBox(Data);

            // Adiciona o nó
            Node.Nodes.Add("[" + Type.ToString() + "] " + Temp.Name);
            Node.LastNode.Tag = Temp;

            // Pula pro próximo
            Tools(Node.Nodes[i], Data);
        }
    }

    private static Objects.Button Button(BinaryReader Data)
    {
        // Lê os dados
        Objects.Button Tool = new Objects.Button
        {
            Name = Data.ReadString(),
            Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32()),
            Visible = Data.ReadBoolean(),
            Window = (Globals.Windows)Data.ReadByte(),
            Texture_Num = Data.ReadByte()
        };
        return Tool;
    }

    private static Objects.TextBox TextBox(BinaryReader Data)
    {
        // Lê os dados
        Objects.TextBox Tool = new Objects.TextBox
        {
            Name = Data.ReadString(),
            Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32()),
            Visible = Data.ReadBoolean(),
            Window = (Globals.Windows)Data.ReadByte(),
            Max_Characters = Data.ReadInt16(),
            Width = Data.ReadInt16(),
            Password = Data.ReadBoolean()
        };
        return Tool;
    }

    private static Objects.Panel Panel(BinaryReader Data)
    {
        // Carrega os dados
        Objects.Panel Tool = new Objects.Panel
        {
            Name = Data.ReadString(),
            Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32()),
            Visible = Data.ReadBoolean(),
            Window = (Globals.Windows)Data.ReadByte(),
            Texture_Num = Data.ReadByte()
        };
        return Tool;
    }

    private static Objects.CheckBox CheckBox(BinaryReader Data)
    {
        // Carrega os dados
        Objects.CheckBox Tool = new Objects.CheckBox
        {
            Name = Data.ReadString(),
            Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32()),
            Visible = Data.ReadBoolean(),
            Window = (Globals.Windows)Data.ReadByte(),
            Text = Data.ReadString(),
            Checked = Data.ReadBoolean()
        };
        return Tool;
    }

    public static void Tiles()
    {
        // Lê os dados
        Lists.Tile = new Lists.Structures.Tile[Graphics.Tex_Tile.Length];
        for (byte i = 1; i < Lists.Tile.Length; i++) Tile(i);
    }

    public static void Tile(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Tiles.FullName + Index + Directories.Format);

        // Evita erros
        if (!File.Exists)
        {
            Clear.Tile(Index);
            Write.Tile(Index);
            return;
        }

        // Lê os dados
        FileStream Stream = File.OpenRead();
        Lists.Tile[Index] = (Lists.Structures.Tile)new BinaryFormatter().Deserialize(Stream);
        Stream.Close();
    }
}