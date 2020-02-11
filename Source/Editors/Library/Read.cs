using System;
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

    private static void Tools(TreeNode Node, BinaryReader Data)
    {
        // Lê todos os filhos
        byte Size = Data.ReadByte();
        for (byte i = 0; i < Size; i++)
        {
            Lists.Structures.Tool Temp = new Lists.Structures.Tool();
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

    private static Lists.Structures.Button Button(BinaryReader Data)
    {
        // Lê os dados
        Lists.Structures.Button Tool = new Lists.Structures.Button
        {
            Name = Data.ReadString(),
            Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32()),
            Visible = Data.ReadBoolean(),
            Window = (Globals.Windows)Data.ReadByte(),
            Texture_Num = Data.ReadByte()
        };
        return Tool;
    }

    private static Lists.Structures.TextBox TextBox(BinaryReader Data)
    {
        // Lê os dados
        Lists.Structures.TextBox Tool = new Lists.Structures.TextBox
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

    private static Lists.Structures.Panel Panel(BinaryReader Data)
    {
        // Carrega os dados
        Lists.Structures.Panel Tool = new Lists.Structures.Panel
        {
            Name = Data.ReadString(),
            Position = new System.Drawing.Point(Data.ReadInt32(), Data.ReadInt32()),
            Visible = Data.ReadBoolean(),
            Window = (Globals.Windows)Data.ReadByte(),
            Texture_Num = Data.ReadByte()
        };
        return Tool;
    }

    private static Lists.Structures.CheckBox CheckBox(BinaryReader Data)
    {
        // Carrega os dados
        Lists.Structures.CheckBox Tool = new Lists.Structures.CheckBox
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
}