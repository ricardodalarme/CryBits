using System.IO;
using System.Windows.Forms;

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

    public static void Tools()
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.Tools.FullName);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        for (short n = 0; n < Lists.Tool.Nodes.Count; n++) Tools(Lists.Tool.Nodes[n], Data);

        // Fecha o sistema
        Data.Dispose();
    }

    private static void Tools(TreeNode Node, BinaryWriter Data)
    {
        // Escreve a ferramenta
        Data.Write((byte)Node.Nodes.Count);
        for (byte i = 0; i < Node.Nodes.Count; i++)
        {
            // Salva de acordo com a ferramenta
            Lists.Structures.Tool Tool = (Lists.Structures.Tool)Node.Nodes[i].Tag;
            if (Tool is Lists.Structures.Button)  Button(Data, (Lists.Structures.Button)Tool);
            else if (Tool is Lists.Structures.TextBox) TextBox(Data, (Lists.Structures.TextBox)Tool);
            else if (Tool is Lists.Structures.CheckBox) CheckBox(Data, (Lists.Structures.CheckBox)Tool);
            else if (Tool is Lists.Structures.Panel) Panel(Data, (Lists.Structures.Panel)Tool);

            // Pula pra próxima ferramenta
            Tools(Node.Nodes[i], Data);
        }
    }

    public static void Button(BinaryWriter Data, Lists.Structures.Button Tool)
    {
        // Escreve os dados
        Data.Write((byte)Globals.Tools_Types.Button);
        Data.Write(Tool.Name);
        Data.Write(Tool.Position.X);
        Data.Write(Tool.Position.Y);
        Data.Write(Tool.Visible);
        Data.Write((byte)Tool.Window);
        Data.Write(Tool.Texture_Num);
    }

    public static void TextBox(BinaryWriter Data, Lists.Structures.TextBox Tool)
    {
        // Escreve os dados
        Data.Write((byte)Globals.Tools_Types.TextBox);
        Data.Write(Tool.Name);
        Data.Write(Tool.Position.X);
        Data.Write(Tool.Position.Y);
        Data.Write(Tool.Visible);
        Data.Write((byte)Tool.Window);
        Data.Write(Tool.Max_Characters);
        Data.Write(Tool.Width);
        Data.Write(Tool.Password);
    }

    public static void Panel(BinaryWriter Data, Lists.Structures.Panel Tool)
    {
        // Escreve os dados
        Data.Write((byte)Globals.Tools_Types.Panel);
        Data.Write(Tool.Name);
        Data.Write(Tool.Position.X);
        Data.Write(Tool.Position.Y);
        Data.Write(Tool.Visible);
        Data.Write((byte)Tool.Window);
        Data.Write(Tool.Texture_Num);
    }

    public static void CheckBox(BinaryWriter Data, Lists.Structures.CheckBox Tool)
    {
        // Escreve os dados
        Data.Write((byte)Globals.Tools_Types.CheckBox);
        Data.Write(Tool.Name);
        Data.Write(Tool.Position.X);
        Data.Write(Tool.Position.Y);
        Data.Write(Tool.Visible);
        Data.Write((byte)Tool.Window);
        Data.Write(Tool.Text);
        Data.Write(Tool.Checked);
    }
}