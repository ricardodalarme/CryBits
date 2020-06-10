using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using static Utils;

namespace Library
{
    static class Write
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
                Objects.Tool Tool = (Objects.Tool)Node.Nodes[i].Tag;
                if (Tool is Objects.Button)
                {
                    Data.Write((byte)Tools_Types.Button);
                    Button(Data, (Objects.Button)Tool);
                }
                else if (Tool is Objects.TextBox)
                {
                    Data.Write((byte)Tools_Types.TextBox);
                    TextBox(Data, (Objects.TextBox)Tool);
                }
                else if (Tool is Objects.CheckBox)
                {
                    Data.Write((byte)Tools_Types.CheckBox);
                    CheckBox(Data, (Objects.CheckBox)Tool);
                }
                else if (Tool is Objects.Panel)
                {
                    Data.Write((byte)Tools_Types.Panel);
                    Panel(Data, (Objects.Panel)Tool);
                }

                // Pula pra próxima ferramenta
                Tools(Node.Nodes[i], Data);
            }
        }

        private static void Button(BinaryWriter Data, Objects.Button Tool)
        {
            // Escreve os dados
            Data.Write(Tool.Name);
            Data.Write(Tool.Position.X);
            Data.Write(Tool.Position.Y);
            Data.Write(Tool.Visible);
            Data.Write((byte)Tool.Window);
            Data.Write(Tool.Texture_Num);
        }

        private static void TextBox(BinaryWriter Data, Objects.TextBox Tool)
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

        private static void Panel(BinaryWriter Data, Objects.Panel Tool)
        {
            // Escreve os dados
            Data.Write(Tool.Name);
            Data.Write(Tool.Position.X);
            Data.Write(Tool.Position.Y);
            Data.Write(Tool.Visible);
            Data.Write((byte)Tool.Window);
            Data.Write(Tool.Texture_Num);
        }

        private static void CheckBox(BinaryWriter Data, Objects.CheckBox Tool)
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
}