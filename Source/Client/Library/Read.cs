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
            using (var stream = Directories.Options.OpenRead())
                Option = (Options)new BinaryFormatter().Deserialize(stream);
        }

        private static Buttons Button(BinaryReader data)
        {
            // Lê os dados
            return new Buttons()
            {
                Name = data.ReadString(),
                Position = new Point(data.ReadInt32(), data.ReadInt32()),
                Visible = data.ReadBoolean(),
                Window = (WindowsTypes)data.ReadByte(),
                Texture_Num = data.ReadByte(),
            };
        }

        private static TextBoxes TextBox(BinaryReader data)
        {
            // Lê os dados
            return new TextBoxes
            {
                Name = data.ReadString(),
                Position = new Point(data.ReadInt32(), data.ReadInt32()),
                Visible = data.ReadBoolean(),
                Window = (WindowsTypes)data.ReadByte(),
                Lenght = data.ReadInt16(),
                Width = data.ReadInt16(),
                Password = data.ReadBoolean(),
            };
        }

        private static Panels Panel(BinaryReader data)
        {
            // Lê os dados
            return new Panels
            {
                Name = data.ReadString(),
                Position = new Point(data.ReadInt32(), data.ReadInt32()),
                Visible = data.ReadBoolean(),
                Window = (WindowsTypes)data.ReadByte(),
                Texture_Num = data.ReadByte(),
            };
        }

        private static CheckBoxes CheckBox(BinaryReader data)
        {
            // Lê os dados
            return new CheckBoxes
            {
                Name = data.ReadString(),
                Position = new Point(data.ReadInt32(), data.ReadInt32()),
                Visible = data.ReadBoolean(),
                Window = (WindowsTypes)data.ReadByte(),
                Text = data.ReadString(),
                Checked = data.ReadBoolean(),
            };
        }

        private static void Tools()
        {
            FileInfo file = new FileInfo(Directories.Tools_Data.FullName);
            for (byte i = 0; i < (byte)WindowsTypes.Count; i++) Interface.Tools.All_Order[i] = new List<Tools.OrderStructure>();

            // Lê todas as ferramentas
            using (var data = new BinaryReader(file.OpenRead()))
                for (byte n = 0; n < Interface.Tools.All_Order.Length; n++)
                    Tools(null, ref Interface.Tools.All_Order[n], data);
        }

        private static void Tools(Tools.OrderStructure parent, ref List<Tools.OrderStructure> node, BinaryReader data)
        {
            // Lê todos os filhos
            byte size = data.ReadByte();

            for (byte i = 0; i < size; i++)
            {
                // Lê a ferramenta
                var tempTool = new Tools.Structure();
                switch ((Tools.Types)data.ReadByte())
                {
                    case Interface.Tools.Types.Button: tempTool = Button(data); Buttons.List.Add(tempTool.Name, (Buttons)tempTool); break;
                    case Interface.Tools.Types.TextBox: tempTool = TextBox(data); TextBoxes.List.Add(tempTool.Name, (TextBoxes)tempTool); break;
                    case Interface.Tools.Types.Panel: tempTool = Panel(data); Panels.List.Add(tempTool.Name, (Panels)tempTool); break;
                    case Interface.Tools.Types.CheckBox: tempTool = CheckBox(data); CheckBoxes.List.Add(tempTool.Name, (CheckBoxes)tempTool); break;
                }

                // Adiciona à lista
                Tools.OrderStructure tempOrder = new Tools.OrderStructure();
                tempOrder.Nodes = new List<Tools.OrderStructure>();
                tempOrder.Parent = parent;
                tempOrder.Data = tempTool;
                node.Add(tempOrder);

                // Pula pro próximo
                Tools(node[i], ref node[i].Nodes, data);
            }
        }

        public static void Map(Guid id)
        {
            FileInfo file = new FileInfo(Directories.Maps_Data.FullName + id.ToString() + Directories.Format);

            // Lê os dados
            using (var stream = file.OpenRead())
                // Map.List.Add(ID,(Objects.Map)new BinaryFormatter().Deserialize(Stream));
                TempMap.List.Add(id, new TempMap(Entities.Map.List[id]));

            // Redimensiona as partículas do clima
            Mapper.Weather_Update();
            MapAutoTile.Update();
        }
    }
}