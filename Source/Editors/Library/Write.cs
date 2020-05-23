using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

class Write
{
    public static void Options()
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Options.FullName).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Options);
        Stream.Close();
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

    public static void Tiles()
    {
        // Escreve os dados
        for (byte i = 1; i < Lists.Tile.Length; i++) Tile(i);
    }

    public static void Tile(byte Index)
    {
        // Escreve os dados
        FileStream Stream = new FileInfo(Directories.Tiles.FullName + Index + Directories.Format).OpenWrite();
        new BinaryFormatter().Serialize(Stream, Lists.Tile[Index]);
        Stream.Close();
    }
}