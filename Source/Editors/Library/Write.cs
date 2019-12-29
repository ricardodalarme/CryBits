using System.Drawing;
using System.IO;

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
        Data.Write(Lists.Button[Index].General.Name);
        Data.Write(Lists.Button[Index].General.Position.X);
        Data.Write(Lists.Button[Index].General.Position.Y);
        Data.Write(Lists.Button[Index].General.Visible);
        Data.Write(Lists.Button[Index].Texture);

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
        Data.Write(Lists.TextBox[Index].General.Name);
        Data.Write(Lists.TextBox[Index].General.Position.X);
        Data.Write(Lists.TextBox[Index].General.Position.Y);
        Data.Write(Lists.TextBox[Index].General.Visible);
        Data.Write(Lists.TextBox[Index].Max_Chars);
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
        Data.Write(Lists.Panel[Index].General.Name);
        Data.Write(Lists.Panel[Index].General.Position.X);
        Data.Write(Lists.Panel[Index].General.Position.Y);
        Data.Write(Lists.Panel[Index].General.Visible);
        Data.Write(Lists.Panel[Index].Texture);

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
        Data.Write(Lists.CheckBox[Index].General.Name);
        Data.Write(Lists.CheckBox[Index].General.Position.X);
        Data.Write(Lists.CheckBox[Index].General.Position.Y);
        Data.Write(Lists.CheckBox[Index].General.Visible);
        Data.Write(Lists.CheckBox[Index].Text);
        Data.Write(Lists.CheckBox[Index].State);

        // Fecha o sistema
        Data.Dispose();
    }
}