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

    public static void Client_Data()
    {
        // Cria um sistema binário para a manipulação dos dados
        BinaryWriter Data = new BinaryWriter(Directories.Client_Data.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Client_Data.Num_Buttons);
        Data.Write(Lists.Client_Data.Num_TextBoxes);
        Data.Write(Lists.Client_Data.Num_Panels);
        Data.Write(Lists.Client_Data.Num_CheckBoxes);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Tools()
    {
        // Escreve os dados de todas as ferramentas
        Buttons();
        TextBoxes();
        Panels();
        CheckBoxes();
    }

    public static void Buttons()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.Button.Length; Index++)
            Button(Index);
    }

    public static void Button(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.Buttons_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Button[Index].Name);
        Data.Write(Lists.Button[Index].Position.X);
        Data.Write(Lists.Button[Index].Position.Y);
        Data.Write(Lists.Button[Index].Visible);
        Data.Write((byte)Lists.Button[Index].Window);
        Data.Write(Lists.Button[Index].Texture_Num);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void TextBoxes()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.TextBox.Length; Index++)
            TextBox(Index);
    }

    public static void TextBox(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.TextBoxes_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.TextBox[Index].Name);
        Data.Write(Lists.TextBox[Index].Position.X);
        Data.Write(Lists.TextBox[Index].Position.Y);
        Data.Write(Lists.TextBox[Index].Visible);
        Data.Write((byte)Lists.TextBox[Index].Window);
        Data.Write(Lists.TextBox[Index].Max_Characters);
        Data.Write(Lists.TextBox[Index].Width);
        Data.Write(Lists.TextBox[Index].Password);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Panels()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.Panel.Length; Index++)
            Panel(Index);
    }

    public static void Panel(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.Panels_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.Panel[Index].Name);
        Data.Write(Lists.Panel[Index].Position.X);
        Data.Write(Lists.Panel[Index].Position.Y);
        Data.Write(Lists.Panel[Index].Visible);
        Data.Write((byte)Lists.Panel[Index].Window);
        Data.Write(Lists.Panel[Index].Texture_Num);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void CheckBoxes()
    {
        // Escreve os dados
        for (byte Index = 1; Index < Lists.CheckBox.Length; Index++)
            CheckBox(Index);
    }

    public static void CheckBox(byte Index)
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.CheckBoxes_Data.FullName + Index + Directories.Format);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        Data.Write(Lists.CheckBox[Index].Name);
        Data.Write(Lists.CheckBox[Index].Position.X);
        Data.Write(Lists.CheckBox[Index].Position.Y);
        Data.Write(Lists.CheckBox[Index].Visible);
        Data.Write((byte)Lists.CheckBox[Index].Window);
        Data.Write(Lists.CheckBox[Index].Text);
        Data.Write(Lists.CheckBox[Index].State);

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Tool_Order()
    {
        // Cria um sistema binário para a manipulação dos dados
        FileInfo File = new FileInfo(Directories.Tool_Order.FullName);
        BinaryWriter Data = new BinaryWriter(File.OpenWrite());

        // Escreve os dados
        for (short n = 0; n < Lists.Tool_Order.Nodes.Count; n++) Tree_Nodes(Lists.Tool_Order.Nodes[n], Data);

        // Fecha o sistema
        Data.Dispose();
    }

    private static void Tree_Nodes(TreeNode Node, BinaryWriter Data)
    {
        // Escreve a ferramenta
        Data.Write((byte)Node.Nodes.Count);
        for (byte i = 0; i < Node.Nodes.Count; i++)
        {
            Data.Write(((Lists.Structures.Tool_Order)Node.Nodes[i].Tag).Index);
            Data.Write((byte)((Lists.Structures.Tool_Order)Node.Nodes[i].Tag).Type);
            Tree_Nodes(Node.Nodes[i], Data);
        }
    }
}