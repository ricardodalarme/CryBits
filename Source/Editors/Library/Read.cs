using System.IO;
using System.Drawing;

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


    #region Client
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

        // Limpa e lê os dados
        for (byte i = 1; i <= Lists.Button.Length; i++)
        {
            Clear.Button(i);
            Button(i);
        }
    }

    public static void Button(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Buttons_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Button(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Lists.Button[Index].General.Name = Data.ReadString();
        Lists.Button[Index].General.Position.X = Data.ReadInt32();
        Lists.Button[Index].General.Position.Y = Data.ReadInt32();
        Lists.Button[Index].General.Visible = Data.ReadBoolean();
        Lists.Button[Index].Texture = Data.ReadByte();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void TextBoxes()
    {
        Lists.TextBox = new Lists.Structures.TextBox[Lists.Client_Data.Num_TextBoxes + 1];

        // Limpa e lê os dados
        for (byte i = 1; i < Lists.TextBox.Length; i++)
        {
            Clear.TextBox(i);
            TextBox(i);
        }
    }

    public static void TextBox(byte Index)
    {
        FileInfo File = new FileInfo(Directories.TextBoxes_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.TextBox(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Lê os dados
        Lists.TextBox[Index].General.Name = Data.ReadString();
        Lists.TextBox[Index].General.Position.X = Data.ReadInt32();
        Lists.TextBox[Index].General.Position.Y = Data.ReadInt32();
        Lists.TextBox[Index].General.Visible = Data.ReadBoolean();
        Lists.TextBox[Index].Max_Chars = Data.ReadInt16();
        Lists.TextBox[Index].Width = Data.ReadInt16();
        Lists.TextBox[Index].Password = Data.ReadBoolean();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void Panels()
    {
        Lists.Panel = new Lists.Structures.Panel[Lists.Client_Data.Num_Panels + 1];

        // Limpa e lê os dados
        for (byte i = 1; i < Lists.Panel.Length; i++)
        {
            Clear.Panel(i);
            Panel(i);
        }
    }

    public static void Panel(byte Index)
    {
        FileInfo File = new FileInfo(Directories.Panels_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.Panel(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados
        Lists.Panel[Index].General.Name = Data.ReadString();
        Lists.Panel[Index].General.Position.X = Data.ReadInt32();
        Lists.Panel[Index].General.Position.Y = Data.ReadInt32();
        Lists.Panel[Index].General.Visible = Data.ReadBoolean();
        Lists.Panel[Index].Texture = Data.ReadByte();

        // Fecha o sistema
        Data.Dispose();
    }

    public static void CheckBoxes()
    {
        Lists.CheckBox = new Lists.Structures.CheckBox[Lists.Client_Data.Num_CheckBoxes + 1];

        // Limpa e lê os dados
        for (byte i = 1; i < Lists.CheckBox.Length; i++)
        {
            Clear.CheckBox(i);
            CheckBox(i);
        }
    }

    public static void CheckBox(byte Index)
    {
        FileInfo File = new FileInfo(Directories.CheckBoxes_Data.FullName + Index + Directories.Format);

        // Cria o arquivo caso ele não existir
        if (!File.Exists)
        {
            Write.CheckBox(Index);
            return;
        }

        // Cria um sistema binário para a manipulação dos dados
        BinaryReader Data = new BinaryReader(File.OpenRead());

        // Carrega os dados
        Lists.CheckBox[Index].General.Name = Data.ReadString();
        Lists.CheckBox[Index].General.Position.X = Data.ReadInt32();
        Lists.CheckBox[Index].General.Position.Y = Data.ReadInt32();
        Lists.CheckBox[Index].General.Visible = Data.ReadBoolean();
        Lists.CheckBox[Index].Text = Data.ReadString();
        Lists.CheckBox[Index].State = Data.ReadBoolean();

        // Fecha o sistema
        Data.Dispose();
    }
    #endregion
}