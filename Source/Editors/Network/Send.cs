using CryBits.Editors.Forms;
using CryBits.Editors.Library;
using CryBits.Entities;
using CryBits.Packets;
using Lidgren.Network;
using static CryBits.Utils;

namespace CryBits.Editors.Network
{
    internal static class Send
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
            data.Write((byte)EditorServer.WriteSettings);
            Packet(data);
        }

        public static void Request_Classes()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.RequestClasses);
            Packet(data);
        }

        public static void Request_Map(Map map)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.RequestMap);
            data.Write(map.ID.ToString());
            Packet(data);
        }

        public static void Request_Maps()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.RequestMaps);
            Packet(data);
        }

        public static void Request_NPCs()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.RequestNPCs);
            Packet(data);
        }

        public static void Request_Items()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.RequestItems);
            Packet(data);
        }

        public static void Request_Shops()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.RequestShops);
            Packet(data);
        }

        public static void Write_Server_Data()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.WriteSettings);
            data.Write(Lists.ServerData.GameName);
            data.Write(Lists.ServerData.Welcome);
            data.Write(Lists.ServerData.Port);
            data.Write(Lists.ServerData.MaxPlayers);
            data.Write(Lists.ServerData.MaxCharacters);
            data.Write(Lists.ServerData.MaxPartyMembers);
            data.Write(Lists.ServerData.MaxMapItems);
            data.Write(Lists.ServerData.NumPoints);
            data.Write(Lists.ServerData.MinNameLength);
            data.Write(Lists.ServerData.MaxNameLength);
            data.Write(Lists.ServerData.MinPasswordLength);
            data.Write(Lists.ServerData.MaxPasswordLength);
            Packet(data);
        }

        public static void Write_Classes()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.WriteClasses);
            ObjectToByteArray(data, Class.List);
            Packet(data);
        }

        public static void Write_Maps()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.WriteMaps);
            ObjectToByteArray(data, Map.List);
            Packet(data);
        }

        public static void Write_NPCs()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.WriteNPCs);
            ObjectToByteArray(data, NPC.List);
            Packet(data);
        }

        public static void Write_Items()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.WriteItems);
            ObjectToByteArray(data, Item.List);
            Packet(data);
        }

        public static void Write_Shops()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)EditorServer.WriteShops);
            ObjectToByteArray(data, Shop.List);
            Packet(data);
        }
    }
}