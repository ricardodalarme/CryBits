using CryBits.Editors.Forms;
using CryBits.Entities;
using CryBits.Packets;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace CryBits.Editors.Network
{
    class Receive
    {
        public static object ByteArrayToObject(NetIncomingMessage data)
        {
            int size = data.ReadInt32();
            byte[] array = data.ReadBytes(size);

            using (var stream = new MemoryStream(array))
                return new BinaryFormatter().Deserialize(stream);
        }

        public static void Handle(NetIncomingMessage Data)
        {
            // Manuseia os dados recebidos
            switch ((ServerEditor)Data.ReadByte())
            {
                case ServerEditor.Alert: Alert(Data); break;
                case ServerEditor.Connect: Connect(); break;
                case ServerEditor.Server_Data: Server_Data(Data); break;
                case ServerEditor.Classes: Classes(Data); break;
                case ServerEditor.Map: Map(Data); break;
                case ServerEditor.NPCs: NPCs(Data); break;
                case ServerEditor.Items: Items(Data); break;
                case ServerEditor.Shops: Shops(Data); break;
            }
        }

        private static void Alert(NetIncomingMessage Data)
        {
            // Mostra a mensagem
            MessageBox.Show(Data.ReadString());
        }

        private static void Connect()
        {
            // Abre a janela principal
            Login.Form.Visible = false;
            Editor_Maps.Form = new Editor_Maps();
        }

        private static void Server_Data(NetIncomingMessage Data)
        {
            // Lê os dados
            Lists.Server_Data.Game_Name = Data.ReadString();
            Lists.Server_Data.Welcome = Data.ReadString();
            Lists.Server_Data.Port = Data.ReadInt16();
            Lists.Server_Data.Max_Players = Data.ReadByte();
            Lists.Server_Data.Max_Characters = Data.ReadByte();
            Lists.Server_Data.Max_Party_Members = Data.ReadByte();
            Lists.Server_Data.Max_Map_Items = Data.ReadByte();
            Lists.Server_Data.Num_Points = Data.ReadByte();
            Lists.Server_Data.Min_Name_Length = Data.ReadByte();
            Lists.Server_Data.Max_Name_Length = Data.ReadByte();
            Lists.Server_Data.Min_Password_Length = Data.ReadByte();
            Lists.Server_Data.Max_Password_Length = Data.ReadByte();
        }

        private static void Classes(NetIncomingMessage Data)
        {
            // Recebe os dados
            Class.List = (Dictionary<Guid, Class>)ByteArrayToObject(Data);
        }

        private static void Map(NetIncomingMessage Data)
        {
            Map Map = (Map)ByteArrayToObject(Data);
            Guid ID = Map.ID;

            // Obtém o dado
            if (Map.List.ContainsKey(ID)) Map.List[ID] = Map;
            else
                Map.List.Add(ID, Map);
        }

        private static void NPCs(NetIncomingMessage Data)
        {
            // Recebe os dados
            NPC.List = (Dictionary<Guid, NPC>)ByteArrayToObject(Data);
        }

        private static void Items(NetIncomingMessage Data)
        {
            // Recebe os dados
            Item.List = (Dictionary<Guid, Item>)ByteArrayToObject(Data);
        }

        private static void Shops(NetIncomingMessage Data)
        {
            // Recebe os dados
            Shop.List = (Dictionary<Guid, Shop>)ByteArrayToObject(Data);
        }
    }
}