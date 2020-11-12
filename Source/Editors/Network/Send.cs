using CryBits.Editors.Forms;
using CryBits.Editors.Library;
using CryBits.Entities;
using CryBits.Packets;
using Lidgren.Network;
using static CryBits.Utils;

namespace CryBits.Editors.Network
{
    class Send
    {
        private static void Packet(NetOutgoingMessage data)
        {
            // Envia os dados ao servidor
            Socket.Device.SendMessage(data, NetDeliveryMethod.ReliableOrdered);
        }

        public static void Connect()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Connect);
            data.Write(Login.Form.txtUsername.Text);
            data.Write(Login.Form.txtPassword.Text);
            data.Write(true); // Acesso pelo editor
            Packet(data);
        }

        public static void Request_Server_Data()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Write_Settings);
            Packet(data);
        }

        public static void Request_Classes()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Request_Classes);
            Packet(data);
        }

        public static void Request_Map(Map map)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Request_Map);
            data.Write(map.ID.ToString());
            Packet(data);
        }

        public static void Request_Maps()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Request_Maps);
            Packet(data);
        }

        public static void Request_NPCs()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Request_NPCs);
            Packet(data);
        }

        public static void Request_Items()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Request_Items);
            Packet(data);
        }

        public static void Request_Shops()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Request_Shops);
            Packet(data);
        }

        public static void Write_Server_Data()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Write_Settings);
            data.Write(Lists.Server_Data.Game_Name);
            data.Write(Lists.Server_Data.Welcome);
            data.Write(Lists.Server_Data.Port);
            data.Write(Lists.Server_Data.Max_Players);
            data.Write(Lists.Server_Data.Max_Characters);
            data.Write(Lists.Server_Data.Max_Party_Members);
            data.Write(Lists.Server_Data.Max_Map_Items);
            data.Write(Lists.Server_Data.Num_Points);
            data.Write(Lists.Server_Data.Min_Name_Length);
            data.Write(Lists.Server_Data.Max_Name_Length);
            data.Write(Lists.Server_Data.Min_Password_Length);
            data.Write(Lists.Server_Data.Max_Password_Length);
            Packet(data);
        }

        public static void Write_Classes()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Write_Classes);
            ObjectToByteArray(data, Class.List);
            Packet(data);
        }

        public static void Write_Maps()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Write_Maps);
            ObjectToByteArray(data, Map.List);
            Packet(data);
        }

        public static void Write_NPCs()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Write_NPCs);
            ObjectToByteArray(data, NPC.List);
            Packet(data);
        }

        public static void Write_Items()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Write_Items);
            ObjectToByteArray(data, Item.List);
            Packet(data);
        }

        public static void Write_Shops()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.Write_Shops);
            ObjectToByteArray(data, Shop.List);
            Packet(data);
        }
    }
}