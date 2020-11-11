using CryBits.Client.Entities;
using CryBits.Client.Interface;
using CryBits.Client.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static CryBits.Client.Logic.Game;

namespace CryBits.Client.Library
{
    static class Read
    {
        public static void Data()
        {
            // Carrega todos os dados
            Tools();
            Options();
        }

        private static void Options()
        {
            // Cria o arquivo se ele não existir
            if (!Directories.Options.Exists)
            {
                Option = new Options();
                Write.Options();
                return;
            }

            // Lê os dados
            using (var Stream = Directories.Options.OpenRead())
                Option = (Options)new BinaryFormatter().Deserialize(Stream);
        }

        private static Buttons Button(BinaryReader Data)
        {
            // Lê os dados
            return new Buttons()
            {
                Name = Data.ReadString(),
                Position = new Point(Data.ReadInt32(), Data.ReadInt32()),
                Visible = Data.ReadBoolean(),
                Window = (Windows.Types)Data.ReadByte(),
                Texture_Num = Data.ReadByte(),
            };
        }

        private static TextBoxes TextBox(BinaryReader Data)
        {
            // Lê os dados
            return new TextBoxes
            {
                Name = Data.ReadString(),
                Position = new Point(Data.ReadInt32(), Data.ReadInt32()),
                Visible = Data.ReadBoolean(),
                Window = (Windows.Types)Data.ReadByte(),
                Lenght = Data.ReadInt16(),
                Width = Data.ReadInt16(),
                Password = Data.ReadBoolean(),
            };
        }

        private static Panels Panel(BinaryReader Data)
        {
            // Lê os dados
            return new Panels
            {
                Name = Data.ReadString(),
                Position = new Point(Data.ReadInt32(), Data.ReadInt32()),
                Visible = Data.ReadBoolean(),
                Window = (Windows.Types)Data.ReadByte(),
                Texture_Num = Data.ReadByte(),
            };
        }

        private static CheckBoxes CheckBox(BinaryReader Data)
        {
            // Lê os dados
            return new CheckBoxes
            {
                Name = Data.ReadString(),
                Position = new Point(Data.ReadInt32(), Data.ReadInt32()),
                Visible = Data.ReadBoolean(),
                Window = (Windows.Types)Data.ReadByte(),
                Text = Data.ReadString(),
                Checked = Data.ReadBoolean(),
            };
        }

        private static void Tools()
        {
            FileInfo File = new FileInfo(Directories.Tools_Data.FullName);
            for (byte i = 0; i < (byte)Windows.Types.Count; i++) Interface.Tools.All_Order[i] = new List<Tools.Order_Structure>();

            // Lê todas as ferramentas
            using (var Data = new BinaryReader(File.OpenRead()))
                for (byte n = 0; n < Interface.Tools.All_Order.Length; n++)
                    Tools(null, ref Interface.Tools.All_Order[n], Data);
        }

        private static void Tools(Tools.Order_Structure Parent, ref List<Tools.Order_Structure> Node, BinaryReader Data)
        {
            // Lê todos os filhos
            byte Size = Data.ReadByte();

            for (byte i = 0; i < Size; i++)
            {
                // Lê a ferramenta
                var Temp_Tool = new Tools.Structure();
                switch ((Tools.Types)Data.ReadByte())
                {
                    case Interface.Tools.Types.Button: Temp_Tool = Button(Data); Buttons.List.Add(Temp_Tool.Name, (Buttons)Temp_Tool); break;
                    case Interface.Tools.Types.TextBox: Temp_Tool = TextBox(Data); TextBoxes.List.Add(Temp_Tool.Name, (TextBoxes)Temp_Tool); break;
                    case Interface.Tools.Types.Panel: Temp_Tool = Panel(Data); Panels.List.Add(Temp_Tool.Name, (Panels)Temp_Tool); break;
                    case Interface.Tools.Types.CheckBox: Temp_Tool = CheckBox(Data); CheckBoxes.List.Add(Temp_Tool.Name, (CheckBoxes)Temp_Tool); break;
                }

                // Adiciona à lista
                Tools.Order_Structure Temp_Order = new Tools.Order_Structure();
                Temp_Order.Nodes = new List<Tools.Order_Structure>();
                Temp_Order.Parent = Parent;
                Temp_Order.Data = Temp_Tool;
                Node.Add(Temp_Order);

                // Pula pro próximo
                Tools(Node[i], ref Node[i].Nodes, Data);
            }
        }

        public static void Map(Guid ID)
        {
            FileInfo File = new FileInfo(Directories.Maps_Data.FullName + ID.ToString() + Directories.Format);

            // Lê os dados
            using (var Stream = File.OpenRead())
                // Map.List.Add(ID,(Objects.Map)new BinaryFormatter().Deserialize(Stream));
                TempMap.List.Add(ID, new TempMap(Entities.Map.List[ID]));

            // Redimensiona as partículas do clima
            Mapper.Weather_Update();
            MapAutoTile.Update();
        }
    }
}