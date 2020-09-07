using Editors;
using Entities;
using Logic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Drawing;

namespace Library
{
    static class Read
    {
        public static void Options()
        {
            // Lê os dados
            using (var Stream = Directories.Options.OpenRead())
                Lists.Options = (Lists.Structures.Options)new BinaryFormatter().Deserialize(Stream);

            // Define os dados
            Directories.SetClient();
        }

        public static void Tools()
        {
            FileInfo File = new FileInfo(Directories.Tools.FullName);

            // Limpa a árvore de ordem
            Lists.Tool = new TreeNode();
            for (byte i = 0; i < Enum.GetValues(typeof(Windows)).Length; i++) Lists.Tool.Nodes.Add(((Windows)i).ToString());

            // Cria o arquivo caso ele não existir
            if (!File.Exists)
            {
                Write.Tools();
                return;
            }

            // Cria um sistema binário para a manipulação dos dados
            using (var Data = new BinaryReader(File.OpenRead()))
                // Lê todos os nós
                for (byte n = 0; n < Lists.Tool.Nodes.Count; n++)
                    Tools(Lists.Tool.Nodes[n], Data);
        }

        public static void Tools(TreeNode Node, BinaryReader Data)
        {
            // Lê todos os filhos
            byte Size = Data.ReadByte();
            for (byte i = 0; i < Size; i++)
            {
                Entities.Tool Temp = new Entities.Tool();
                Tools_Types Type = (Tools_Types)Data.ReadByte();

                // Lê a ferramenta
                if (Type == Tools_Types.Button) Temp = Button(Data);
                else if (Type == Tools_Types.CheckBox) Temp = CheckBox(Data);
                else if (Type == Tools_Types.Panel) Temp = Panel(Data);
                else if (Type == Tools_Types.TextBox) Temp = TextBox(Data);

                // Adiciona o nó
                Node.Nodes.Add("[" + Type.ToString() + "] " + Temp.Name);
                Node.LastNode.Tag = Temp;

                // Pula pro próximo
                Tools(Node.Nodes[i], Data);
            }
        }

        private static Entities.Button Button(BinaryReader Data)
        {
            // Lê os dados
            return new Entities.Button
            {
                Name = Data.ReadString(),
                Position = new  Point(Data.ReadInt32(), Data.ReadInt32()),
                Visible = Data.ReadBoolean(),
                Window = (Windows)Data.ReadByte(),
                Texture_Num = Data.ReadByte()
            };
        }

        private static Entities.TextBox TextBox(BinaryReader Data)
        {
            // Lê os dados
           return new Entities.TextBox
            {
                Name = Data.ReadString(),
                Position = new Point(Data.ReadInt32(), Data.ReadInt32()),
                Visible = Data.ReadBoolean(),
                Window = (Windows)Data.ReadByte(),
                Max_Characters = Data.ReadInt16(),
                Width = Data.ReadInt16(),
                Password = Data.ReadBoolean()
            };
        }

        private static Entities.Panel Panel(BinaryReader Data)
        {
            // Carrega os dados
            return  new Entities.Panel
            {
                Name = Data.ReadString(),
                Position = new  Point(Data.ReadInt32(), Data.ReadInt32()),
                Visible = Data.ReadBoolean(),
                Window = (Windows)Data.ReadByte(),
                Texture_Num = Data.ReadByte()
            };
        }

        private static Entities.CheckBox CheckBox(BinaryReader Data)
        {
            // Carrega os dados
           return new Entities.CheckBox
            {
                Name = Data.ReadString(),
                Position = new  Point(Data.ReadInt32(), Data.ReadInt32()),
                Visible = Data.ReadBoolean(),
                Window = (Windows)Data.ReadByte(),
                Text = Data.ReadString(),
                Checked = Data.ReadBoolean()
            };
        }

        public static void Tiles()
        {
            // Lê os dados
            Lists.Tile = new Tile[Graphics.Tex_Tile.Length];
            for (byte i = 1; i < Lists.Tile.Length; i++) Tile(i);
        }

        public static void Tile(byte Index)
        {
            FileInfo File = new FileInfo(Directories.Tiles.FullName + Index + Directories.Format);

            // Evita erros
            if (!File.Exists)
            {
                Lists.Tile[Index] = new Tile(Index);
                Write.Tile(Index);
                return;
            }

            // Lê os dados
            // using (var Stream = File.OpenRead())
            // Lists.Tile[Index] = (Tile)new BinaryFormatter().Deserialize(Stream);
        }
    }
}