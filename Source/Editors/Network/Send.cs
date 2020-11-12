using CryBits.Editors.Forms;
using CryBits.Entities;
using CryBits.Packets;
using Lidgren.Network;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CryBits.Editors.Network
{
    class Send
    {
        public static void ObjectToByteArray(NetOutgoingMessage Data, object obj)
        {
            var bf = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                bf.Serialize(stream, obj);
                Data.Write(stream.ToArray().Length);
                Data.Write(stream.ToArray());
            }
        }

        private static void Packet(NetOutgoingMessage Data)
        {
            // Envia os dados ao servidor
            Socket.Device.SendMessage(Data, NetDeliveryMethod.ReliableOrdered);
        }

        public static void Connect()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Connect);
            Data.Write(Login.Form.txtUsername.Text);
            Data.Write(Login.Form.txtPassword.Text);
            Data.Write(true); // Acesso pelo editor
            Packet(Data);
        }

        public static void Request_Server_Data()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Write_Settings);
            Packet(Data);
        }

        public static void Request_Classes()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Request_Classes);
            Packet(Data);
        }

        public static void Request_Map(Map Map)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Request_Map);
            Data.Write(Map.ID.ToString());
            Packet(Data);
        }

        public static void Request_Maps()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Request_Maps);
            Packet(Data);
        }

        public static void Request_NPCs()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Request_NPCs);
            Packet(Data);
        }

        public static void Request_Items()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Request_Items);
            Packet(Data);
        }

        public static void Request_Shops()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Request_Shops);
            Packet(Data);
        }

        public static void Write_Server_Data()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Write_Settings);
            Data.Write(Lists.Server_Data.Game_Name);
            Data.Write(Lists.Server_Data.Welcome);
            Data.Write(Lists.Server_Data.Port);
            Data.Write(Lists.Server_Data.Max_Players);
            Data.Write(Lists.Server_Data.Max_Characters);
            Data.Write(Lists.Server_Data.Max_Party_Members);
            Data.Write(Lists.Server_Data.Max_Map_Items);
            Data.Write(Lists.Server_Data.Num_Points);
            Data.Write(Lists.Server_Data.Min_Name_Length);
            Data.Write(Lists.Server_Data.Max_Name_Length);
            Data.Write(Lists.Server_Data.Min_Password_Length);
            Data.Write(Lists.Server_Data.Max_Password_Length);
            Packet(Data);
        }

        public static void Write_Classes()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Write_Classes);
            ObjectToByteArray(Data, Class.List);
            Packet(Data);
        }

        public static void Write_Maps()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Write_Maps);
            Data.Write((byte)Map.List.Count);
            ObjectToByteArray(Data, Map.List);
            Packet(Data);
        }

        public static void Write_NPCs()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Write_NPCs);
            ObjectToByteArray(Data, NPC.List);
            Packet(Data);
        }

        public static void Write_Items()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Write_Items);
            ObjectToByteArray(Data, Item.List);
            Packet(Data);
        }

        public static void Write_Shops()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)EditorServer.Write_Shops);
            ObjectToByteArray(Data, Shop.List);
            Packet(Data);
        }
    }
}