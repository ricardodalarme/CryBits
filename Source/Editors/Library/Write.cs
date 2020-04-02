using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
            if (Tool is Lists.Structures.Button)
            {
                Data.Write((byte)Globals.Tools_Types.Button);
                Button(Data, (Lists.Structures.Button)Tool);
            }
            else if (Tool is Lists.Structures.TextBox)
            {
                Data.Write((byte)Globals.Tools_Types.TextBox);
                TextBox(Data, (Lists.Structures.TextBox)Tool);
            }
            else if (Tool is Lists.Structures.CheckBox)
            {
                Data.Write((byte)Globals.Tools_Types.CheckBox);
                CheckBox(Data, (Lists.Structures.CheckBox)Tool);
            }
            else if (Tool is Lists.Structures.Panel)
            {
                Data.Write((byte)Globals.Tools_Types.Panel);
                Panel(Data, (Lists.Structures.Panel)Tool);
            }

            // Pula pra próxima ferramenta
            Tools(Node.Nodes[i], Data);
        }
    }

    private static void Button(BinaryWriter Data, Lists.Structures.Button Tool)
    {
        // Escreve os dados
        Data.Write(Tool.Name);
        Data.Write(Tool.Position.X);
        Data.Write(Tool.Position.Y);
        Data.Write(Tool.Visible);
        Data.Write((byte)Tool.Window);
        Data.Write(Tool.Texture_Num);
    }

    private static void TextBox(BinaryWriter Data, Lists.Structures.TextBox Tool)
    {
        // Escreve os dados
        Data.Write(Tool.Name);
        Data.Write(Tool.Position.X);
        Data.Write(Tool.Position.Y);
        Data.Write(Tool.Visible);
        Data.Write((byte)Tool.Window);
        Data.Write(Tool.Max_Characters);
        Data.Write(Tool.Width);
        Data.Write(Tool.Password);
    }

    private static void Panel(BinaryWriter Data, Lists.Structures.Panel Tool)
    {
        // Escreve os dados
        Data.Write(Tool.Name);
        Data.Write(Tool.Position.X);
        Data.Write(Tool.Position.Y);
        Data.Write(Tool.Visible);
        Data.Write((byte)Tool.Window);
        Data.Write(Tool.Texture_Num);
    }

    private static void CheckBox(BinaryWriter Data, Lists.Structures.CheckBox Tool)
    {
        // Escreve os dados
        Data.Write(Tool.Name);
        Data.Write(Tool.Position.X);
        Data.Write(Tool.Position.Y);
        Data.Write(Tool.Visible);
        Data.Write((byte)Tool.Window);
        Data.Write(Tool.Text);
        Data.Write(Tool.Checked);
    }

    public static void Sprites()
    {
        // Escreve os dados
        for (short i = 1; i < Lists.Sprite.Length; i++) Sprite(i);
    }

    public static void Sprite(short Index)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Sprites.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Sprite[Index]);
        Stream.Close();
    }
}