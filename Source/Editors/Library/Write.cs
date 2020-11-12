using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace CryBits.Editors.Library
{
    static class Write
    {
        public static void Options()
        {
            // Escreve os dados
            using (var Stream = new FileInfo(Directories.Options.FullName).OpenWrite())
                new BinaryFormatter().Serialize(Stream, Lists.Options);
        }

        public static void Tools()
        {
            // Cria um sistema binário para a manipulação dos dados
            FileInfo File = new FileInfo(Directories.Tools.FullName);
            using (var Data = new BinaryWriter(File.OpenWrite()))
                // Escreve os dados
                for (short n = 0; n < Lists.Tool.Nodes.Count; n++)
                    Tools(Lists.Tool.Nodes[n], Data);
        }

        private static void Tools(TreeNode Node, BinaryWriter Data)
        {
            // Escreve a ferramenta
            Data.Write((byte)Node.Nodes.Count);
            for (byte i = 0; i < Node.Nodes.Count; i++)
            {
                // Salva de acordo com a ferramenta
                Entities.Tool Tool = (Entities.Tool)Node.Nodes[i].Tag;
                if (Tool is Entities.Button)
                {
                    Data.Write((byte)ToolsTypes.Button);
                    Button(Data, (Entities.Button)Tool);
                }
                else if (Tool is Entities.TextBox)
                {
                    Data.Write((byte)ToolsTypes.TextBox);
                    TextBox(Data, (Entities.TextBox)Tool);
                }
                else if (Tool is Entities.CheckBox)
                {
                    Data.Write((byte)ToolsTypes.CheckBox);
                    CheckBox(Data, (Entities.CheckBox)Tool);
                }
                else if (Tool is Entities.Panel)
                {
                    Data.Write((byte)ToolsTypes.Panel);
                    Panel(Data, (Entities.Panel)Tool);
                }

                // Pula pra próxima ferramenta
                Tools(Node.Nodes[i], Data);
            }
        }

        private static void Button(BinaryWriter Data, Entities.Button Tool)
        {
            // Escreve os dados
            Data.Write(Tool.Name);
            Data.Write(Tool.Position.X);
            Data.Write(Tool.Position.Y);
            Data.Write(Tool.Visible);
            Data.Write((byte)Tool.Window);
            Data.Write(Tool.Texture_Num);
        }

        private static void TextBox(BinaryWriter Data, Entities.TextBox Tool)
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

        private static void Panel(BinaryWriter Data, Entities.Panel Tool)
        {
            // Escreve os dados
            Data.Write(Tool.Name);
            Data.Write(Tool.Position.X);
            Data.Write(Tool.Position.Y);
            Data.Write(Tool.Visible);
            Data.Write((byte)Tool.Window);
            Data.Write(Tool.Texture_Num);
        }

        private static void CheckBox(BinaryWriter Data, Entities.CheckBox Tool)
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
            using (var Stream = new FileInfo(Directories.Tiles.FullName + Index + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(Stream, Lists.Tile[Index]);
        }
    }
}