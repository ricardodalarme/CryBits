using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Entities;
using Interface;

namespace Library
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
                Utils.Option = new Utils.Options();
                Write.Options();
                return;
            }

            // Lê os dados
            using (var Stream = Directories.Options.OpenRead())
                Utils.Option = (Utils.Options)new BinaryFormatter().Deserialize(Stream);
        }

        private static Buttons Button(BinaryReader Data)
        {
            // Lê os dados
            var Tool = new Buttons();
            Tool.Name = Data.ReadString();
            Tool.Position.X = Data.ReadInt32();
            Tool.Position.Y = Data.ReadInt32();
            Tool.Visible = Data.ReadBoolean();
            Tool.Window = (Windows.Types)Data.ReadByte();
            Tool.Texture_Num = Data.ReadByte();
            return Tool;
        }

        private static TextBoxes TextBox(BinaryReader Data)
        {
            // Lê os dados
            var Tool = new TextBoxes();
            Tool.Name = Data.ReadString();
            Tool.Position.X = Data.ReadInt32();
            Tool.Position.Y = Data.ReadInt32();
            Tool.Visible = Data.ReadBoolean();
            Tool.Window = (Windows.Types)Data.ReadByte();
            Tool.Lenght = Data.ReadInt16();
            Tool.Width = Data.ReadInt16();
            Tool.Password = Data.ReadBoolean();
            return Tool;
        }

        private static Panels Panel(BinaryReader Data)
        {
            // Carrega os dados
            Panels Tool = new Panels();
            Tool.Name = Data.ReadString();
            Tool.Position.X = Data.ReadInt32();
            Tool.Position.Y = Data.ReadInt32();
            Tool.Visible = Data.ReadBoolean();
            Tool.Window = (Windows.Types)Data.ReadByte();
            Tool.Texture_Num = Data.ReadByte();
            return Tool;
        }

        private static CheckBoxes CheckBox(BinaryReader Data)
        {
            // Carrega os dados
            var Tool = new CheckBoxes();
            Tool.Name = Data.ReadString();
            Tool.Position.X = Data.ReadInt32();
            Tool.Position.Y = Data.ReadInt32();
            Tool.Visible = Data.ReadBoolean();
            Tool.Window = (Windows.Types)Data.ReadByte();
            Tool.Text = Data.ReadString();
            Tool.Checked = Data.ReadBoolean();
            return Tool;
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
            Mapper.Autotile.Update();
        }
    }
}