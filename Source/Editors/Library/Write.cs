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
            using (var stream = new FileInfo(Directories.Options.FullName).OpenWrite())
                new BinaryFormatter().Serialize(stream, Lists.Options);
        }

        public static void Tools()
        {
            // Cria um sistema binário para a manipulação dos dados
            FileInfo file = new FileInfo(Directories.Tools.FullName);
            using (var data = new BinaryWriter(file.OpenWrite()))
                // Escreve os dados
                for (short n = 0; n < Lists.Tool.Nodes.Count; n++)
                    Tools(Lists.Tool.Nodes[n], data);
        }

        private static void Tools(TreeNode node, BinaryWriter data)
        {
            // Escreve a ferramenta
            data.Write((byte)node.Nodes.Count);
            for (byte i = 0; i < node.Nodes.Count; i++)
            {
                // Salva de acordo com a ferramenta
                Entities.Tool tool = (Entities.Tool)node.Nodes[i].Tag;
                if (tool is Entities.Button)
                {
                    data.Write((byte)ToolsTypes.Button);
                    Button(data, (Entities.Button)tool);
                }
                else if (tool is Entities.TextBox)
                {
                    data.Write((byte)ToolsTypes.TextBox);
                    TextBox(data, (Entities.TextBox)tool);
                }
                else if (tool is Entities.CheckBox)
                {
                    data.Write((byte)ToolsTypes.CheckBox);
                    CheckBox(data, (Entities.CheckBox)tool);
                }
                else if (tool is Entities.Panel)
                {
                    data.Write((byte)ToolsTypes.Panel);
                    Panel(data, (Entities.Panel)tool);
                }

                // Pula pra próxima ferramenta
                Tools(node.Nodes[i], data);
            }
        }

        private static void Button(BinaryWriter data, Entities.Button tool)
        {
            // Escreve os dados
            data.Write(tool.Name);
            data.Write(tool.Position.X);
            data.Write(tool.Position.Y);
            data.Write(tool.Visible);
            data.Write((byte)tool.Window);
            data.Write(tool.Texture_Num);
        }

        private static void TextBox(BinaryWriter data, Entities.TextBox tool)
        {
            // Escreve os dados
            data.Write(tool.Name);
            data.Write(tool.Position.X);
            data.Write(tool.Position.Y);
            data.Write(tool.Visible);
            data.Write((byte)tool.Window);
            data.Write(tool.Max_Characters);
            data.Write(tool.Width);
            data.Write(tool.Password);
        }

        private static void Panel(BinaryWriter data, Entities.Panel tool)
        {
            // Escreve os dados
            data.Write(tool.Name);
            data.Write(tool.Position.X);
            data.Write(tool.Position.Y);
            data.Write(tool.Visible);
            data.Write((byte)tool.Window);
            data.Write(tool.Texture_Num);
        }

        private static void CheckBox(BinaryWriter data, Entities.CheckBox tool)
        {
            // Escreve os dados
            data.Write(tool.Name);
            data.Write(tool.Position.X);
            data.Write(tool.Position.Y);
            data.Write(tool.Visible);
            data.Write((byte)tool.Window);
            data.Write(tool.Text);
            data.Write(tool.Checked);
        }

        public static void Tiles()
        {
            // Escreve os dados
            for (byte i = 1; i < Lists.Tile.Length; i++) Tile(i);
        }

        public static void Tile(byte index)
        {
            // Escreve os dados
            using (var stream = new FileInfo(Directories.Tiles.FullName + index + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(stream, Lists.Tile[index]);
        }
    }
}