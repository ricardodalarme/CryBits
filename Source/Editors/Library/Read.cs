using System.IO;
using System.Windows.Forms;

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
        Send.Request_Server_Data();

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

        // Lê os dados
        for (byte i = 1; i < Lists.Button.Length; i++) Button(i);
    }

    public static void Button(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Buttons_Data.FullName + Index + Directories.Format);
        Lists.Button[Index] = new Lists.Structures.Button();

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Button(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Lists.Button[Index].Name = Data.ReadString();
        Lists.Button[Index].Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32());
        Lists.Button[Index].Visible = Data.ReadBoolean();
        Lists.Button[Index].Window = (Globals.Windows)Data.ReadByte();
        Lists.Button[Index].Texture_Num = Data.ReadByte();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void TextBoxes()
    {
        Lists.TextBox = new Lists.Structures.TextBox[Lists.Client_Data.Num_TextBoxes + 1];

        // Lê os dados
        for (byte i = 1; i < Lists.TextBox.Length; i++) TextBox(i);
    }

    public static void TextBox(byte Index)
    {
        FileInfo File = new FileInfo(Directories.TextBoxes_Data.FullName + Index + Directories.Format);
        Lists.TextBox[Index] = new Lists.Structures.TextBox();

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.TextBox(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Lists.TextBox[Index].Name = Data.ReadString();
        Lists.TextBox[Index].Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32());
        Lists.TextBox[Index].Visible = Data.ReadBoolean();
        Lists.TextBox[Index].Window = (Globals.Windows)Data.ReadByte();
        Lists.TextBox[Index].Max_Characters = Data.ReadInt16();
        Lists.TextBox[Index].Width = Data.ReadInt16();
        Lists.TextBox[Index].Password = Data.ReadBoolean();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Panels()
    {
        Lists.Panel = new Lists.Structures.Panel[Lists.Client_Data.Num_Panels + 1];

        // Lê os dados
        for (byte i = 1; i < Lists.Panel.Length; i++) Panel(i);
    }

    public static void Panel(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Panels_Data.FullName + Index + Directories.Format);
        Lists.Panel[Index] = new Lists.Structures.Panel();

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Panel(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados
        Lists.Panel[Index].Name = Data.ReadString();
        Lists.Panel[Index].Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32());
        Lists.Panel[Index].Visible = Data.ReadBoolean();
        Lists.Panel[Index].Window = (Globals.Windows)Data.ReadByte();
        Lists.Panel[Index].Texture_Num = Data.ReadByte();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void CheckBoxes()
    {
        Lists.CheckBox = new Lists.Structures.CheckBox[Lists.Client_Data.Num_CheckBoxes + 1];

        // Lê os dados
        for (byte i = 1; i < Lists.CheckBox.Length; i++) CheckBox(i);
    }

    public static void CheckBox(byte Index)
    {
        FileInfo File = new FileInfo(Directories.CheckBoxes_Data.FullName + Index + Directories.Format);
        Lists.CheckBox[Index] = new Lists.Structures.CheckBox();

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.CheckBox(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados
        Lists.CheckBox[Index].Name = Data.ReadString();
        Lists.CheckBox[Index].Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32());
        Lists.CheckBox[Index].Visible = Data.ReadBoolean();
        Lists.CheckBox[Index].Window = (Globals.Windows)Data.ReadByte();
        Lists.CheckBox[Index].Text = Data.ReadString();
        Lists.CheckBox[Index].State = Data.ReadBoolean();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Tool_Order()
    {
        FileInfo File = new FileInfo(Directories.Tool_Order.FullName);
        Lists.Tool_Order = new TreeNode();
        for (byte i = 0; i < (byte)Globals.Windows.Count; i++) Lists.Tool_Order.Nodes.Add(((Globals.Windows)i).ToString());

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Globals.Add_Tools_Order();
            Write.Tool_Order();
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê todos os nós
        for (byte n = 0; n < Lists.Tool_Order.Nodes.Count; n++) Order_Nodes(Lists.Tool_Order.Nodes[n], Data);

        // Fecha o sistema
        Data.Dispose();
    }

    private static void Order_Nodes(TreeNode Node, BinaryReader Data)
    {
        // Lê todos os filhos
        byte Size = Data.ReadByte();
        for (byte i = 0; i < Size; i++)
        {
            // Lê a ferramenta
            Lists.Structures.Tool_Order Temp = new Lists.Structures.Tool_Order();
            Temp.Index = Data.ReadByte();
            Temp.Type = (Globals.Tools_Types)Data.ReadByte();

            // Adiciona o nó
            Node.Nodes.Add("[" + Temp.Type.ToString() + "] " + Temp.Data.Name);
            Node.LastNode.Tag = Temp;

            // Pula pro próximo
            Order_Nodes(Node.Nodes[i], Data);
        }
    }
}