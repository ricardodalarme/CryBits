using CryBits.Editors.Entities.Tools;
using CryBits.Enums;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using static CryBits.Editors.Logic.Options;
using Button = CryBits.Editors.Entities.Tools.Button;
using CheckBox = CryBits.Editors.Entities.Tools.CheckBox;
using Panel = CryBits.Editors.Entities.Tools.Panel;
using TextBox = CryBits.Editors.Entities.Tools.TextBox;

namespace CryBits.Editors.Library
{
    internal static class Write
    {
        public static void Options()
        {
            // Escreve as configurações
            using (var data = new BinaryWriter(Directories.Options.OpenWrite()))
            {
                data.Write(PreMapGrid);
                data.Write(PreMapView);
                data.Write(PreMapAudio);
                data.Write(Username);
            }
        }

        public static void Tools()
        {
            // Cria um sistema binário para a manipulação dos dados
            FileInfo file = new FileInfo(Directories.Tools.FullName);
            using (var data = new BinaryWriter(file.OpenWrite()))
                // Escreve os dados
                for (short n = 0; n < Tool.Tree.Nodes.Count; n++)
                    Tools(Tool.Tree.Nodes[n], data);
        }

        private static void Tools(TreeNode node, BinaryWriter data)
        {
            // Escreve a ferramenta
            data.Write((byte)node.Nodes.Count);
            for (byte i = 0; i < node.Nodes.Count; i++)
            {
                // Salva de acordo com a ferramenta
                Tool tool = (Tool)node.Nodes[i].Tag;
                if (tool is Button)
                {
                    data.Write((byte)ToolType.Button);
                    Button(data, (Button)tool);
                }
                else if (tool is TextBox)
                {
                    data.Write((byte)ToolType.TextBox);
                    TextBox(data, (TextBox)tool);
                }
                else if (tool is CheckBox)
                {
                    data.Write((byte)ToolType.CheckBox);
                    CheckBox(data, (CheckBox)tool);
                }
                else if (tool is Panel)
                {
                    data.Write((byte)ToolType.Panel);
                    Panel(data, (Panel)tool);
                }

                // Pula pra próxima ferramenta
                Tools(node.Nodes[i], data);
            }
        }

        private static void Button(BinaryWriter data, Button tool)
        {
            // Escreve os dados
            data.Write(tool.Name);
            data.Write(tool.Position.X);
            data.Write(tool.Position.Y);
            data.Write(tool.Visible);
            data.Write((byte)tool.Window);
            data.Write(tool.TextureNum);
        }

        private static void TextBox(BinaryWriter data, TextBox tool)
        {
            // Escreve os dados
            data.Write(tool.Name);
            data.Write(tool.Position.X);
            data.Write(tool.Position.Y);
            data.Write(tool.Visible);
            data.Write((byte)tool.Window);
            data.Write(tool.MaxCharacters);
            data.Write(tool.Width);
            data.Write(tool.Password);
        }

        private static void Panel(BinaryWriter data, Panel tool)
        {
            // Escreve os dados
            data.Write(tool.Name);
            data.Write(tool.Position.X);
            data.Write(tool.Position.Y);
            data.Write(tool.Visible);
            data.Write((byte)tool.Window);
            data.Write(tool.TextureNum);
        }

        private static void CheckBox(BinaryWriter data, CheckBox tool)
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
            for (byte i = 1; i < Entities.Tile.List.Length; i++) Tile(i);
        }

        public static void Tile(byte index)
        {
            // Escreve os dados
            using (var stream = new FileInfo(Directories.Tiles.FullName + index + Directories.Format).OpenWrite())
                new BinaryFormatter().Serialize(stream, Entities.Tile.List[index]);
        }
    }
}