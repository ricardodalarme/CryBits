using CryBits.Editors.Entities;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace CryBits.Editors.Library
{
    static class Read
    {
        public static void Options()
        {
            // Cria o arquivo se ele não existir
            if (!Directories.Options.Exists)
            {
                Lists.Options = new Lists.Structures.Options();
                Write.Options();
                return;
            }

            // Lê os dados
            using (var stream = Directories.Options.OpenRead())
                Lists.Options = (Lists.Structures.Options)new BinaryFormatter().Deserialize(stream);
        }

        public static void Tools()
        {
            FileInfo file = new FileInfo(Directories.Tools.FullName);

            // Limpa a árvore de ordem
            Lists.Tool = new TreeNode();
            for (byte i = 0; i < Enum.GetValues(typeof(WindowsTypes)).Length; i++) Lists.Tool.Nodes.Add(((WindowsTypes)i).ToString());

            // Cria o arquivo caso ele não existir
            if (!file.Exists)
            {
                Write.Tools();
                return;
            }

            // Cria um sistema binário para a manipulação dos dados
            using (var data = new BinaryReader(file.OpenRead()))
                // Lê todos os nós
                for (byte n = 0; n < Lists.Tool.Nodes.Count; n++)
                    Tools(Lists.Tool.Nodes[n], data);
        }

        public static void Tools(TreeNode node, BinaryReader data)
        {
            // Lê todos os filhos
            byte size = data.ReadByte();
            for (byte i = 0; i < size; i++)
            {
                Entities.Tool temp = new Entities.Tool();
                ToolsTypes type = (ToolsTypes)data.ReadByte();

                // Lê a ferramenta
                if (type == ToolsTypes.Button) temp = Button(data);
                else if (type == ToolsTypes.CheckBox) temp = CheckBox(data);
                else if (type == ToolsTypes.Panel) temp = Panel(data);
                else if (type == ToolsTypes.TextBox) temp = TextBox(data);

                // Adiciona o nó
                node.Nodes.Add("[" + type.ToString() + "] " + temp.Name);
                node.LastNode.Tag = temp;

                // Pula pro próximo
                Tools(node.Nodes[i], data);
            }
        }

        private static Entities.Button Button(BinaryReader data)
        {
            // Lê os dados
            return new Entities.Button
            {
                Name = data.ReadString(),
                Position = new Point(data.ReadInt32(), data.ReadInt32()),
                Visible = data.ReadBoolean(),
                Window = (WindowsTypes)data.ReadByte(),
                Texture_Num = data.ReadByte()
            };
        }

        private static Entities.TextBox TextBox(BinaryReader data)
        {
            // Lê os dados
            return new Entities.TextBox
            {
                Name = data.ReadString(),
                Position = new Point(data.ReadInt32(), data.ReadInt32()),
                Visible = data.ReadBoolean(),
                Window = (WindowsTypes)data.ReadByte(),
                Max_Characters = data.ReadInt16(),
                Width = data.ReadInt16(),
                Password = data.ReadBoolean()
            };
        }

        private static Entities.Panel Panel(BinaryReader data)
        {
            // Carrega os dados
            return new Entities.Panel
            {
                Name = data.ReadString(),
                Position = new Point(data.ReadInt32(), data.ReadInt32()),
                Visible = data.ReadBoolean(),
                Window = (WindowsTypes)data.ReadByte(),
                Texture_Num = data.ReadByte()
            };
        }

        private static Entities.CheckBox CheckBox(BinaryReader data)
        {
            // Carrega os dados
            return new Entities.CheckBox
            {
                Name = data.ReadString(),
                Position = new Point(data.ReadInt32(), data.ReadInt32()),
                Visible = data.ReadBoolean(),
                Window = (WindowsTypes)data.ReadByte(),
                Text = data.ReadString(),
                Checked = data.ReadBoolean()
            };
        }

        public static void Tiles()
        {
            // Lê os dados
            Lists.Tile = new Tile[Graphics.Tex_Tile.Length];
            for (byte i = 1; i < Lists.Tile.Length; i++) Tile(i);
        }

        public static void Tile(byte index)
        {
            FileInfo file = new FileInfo(Directories.Tiles.FullName + index + Directories.Format);

            // Evita erros
            if (!file.Exists)
            {
                Lists.Tile[index] = new Tile(index);
                Write.Tile(index);
                return;
            }

            // Lê os dados
            using (var stream = file.OpenRead())
                Lists.Tile[index] = (Tile)new BinaryFormatter().Deserialize(stream);
        }
    }
}