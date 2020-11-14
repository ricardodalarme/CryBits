using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CryBits.Client.Logic;
using CryBits.Client.UI;
using static CryBits.Client.Logic.Game;

namespace CryBits.Client.Library
{
    internal static class Read
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
            return new Buttons
            {
                Name = data.ReadString(),
                Position = new Point(data.ReadInt32(), data.ReadInt32()),
                Visible = data.ReadBoolean(),
                Window = (WindowsTypes)data.ReadByte(),
                TextureNum = data.ReadByte(),
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
                Length = data.ReadInt16(),
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
                TextureNum = data.ReadByte(),
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
            FileInfo file = new FileInfo(Directories.ToolsData.FullName);
            for (byte i = 0; i < (byte)WindowsTypes.Count; i++) UI.Tools.AllOrder[i] = new List<Tools.OrderStructure>();

            // Lê todas as ferramentas
            using (var data = new BinaryReader(file.OpenRead()))
                for (byte n = 0; n < UI.Tools.AllOrder.Length; n++)
                    Tools(null, ref UI.Tools.AllOrder[n], data);
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
                    case UI.Tools.Types.Button: tempTool = Button(data); Buttons.List.Add(tempTool.Name, (Buttons)tempTool); break;
                    case UI.Tools.Types.TextBox: tempTool = TextBox(data); TextBoxes.List.Add(tempTool.Name, (TextBoxes)tempTool); break;
                    case UI.Tools.Types.Panel: tempTool = Panel(data); Panels.List.Add(tempTool.Name, (Panels)tempTool); break;
                    case UI.Tools.Types.CheckBox: tempTool = CheckBox(data); CheckBoxes.List.Add(tempTool.Name, (CheckBoxes)tempTool); break;
                }

                // Adiciona à lista
                node.Add(new Tools.OrderStructure
                {
                    Nodes = new List<Tools.OrderStructure>(),
                    Parent = parent,
                    Data = tempTool
                });

                // Pula pro próximo
                Tools(node[i], ref node[i].Nodes, data);
            }
        }

        public static void Map(Guid id)
        {
            FileInfo file = new FileInfo(Directories.MapsData.FullName + id + Directories.Format);

            // Lê os dados
            using (var stream = file.OpenRead())
                CryBits.Entities.Map.List.Add(id, (CryBits.Entities.Map)new BinaryFormatter().Deserialize(stream));

            // Redimensiona as partículas do clima
            Mapper.Weather_Update();
            Mapper.Current.Data.Update();
        }
    }
}