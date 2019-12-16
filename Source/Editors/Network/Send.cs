using System;
using Lidgren.Network;

partial class Send
{
    // Pacotes do cliente
    public enum Packets
    {
        Connect,
        Write_Server_Data,
        Write_Classes,
        Write_Tiles,
        Write_Maps,
        Write_NPCs,
        Write_Items,
        Request_Server_Data,
        Request_Classes,
        Request_Tiles,
        Request_Map,
        Request_Maps,
        Request_NPCs,
        Request_Items
    }

    public static void Packet(NetOutgoingMessage Data)
    {
        // Envia os dados ao servidor
        Socket.Device.SendMessage(Data, NetDeliveryMethod.ReliableOrdered);
    }

    public static void Connect()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Connect);
        Data.Write(Login.Objects.txtName.Text);
        Data.Write(Login.Objects.txtPassword.Text);
        Data.Write(true); // Acesso pelo editor
        Packet(Data);
    }

    public static void Write_Server_Data()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Server_Data);
        Packet(Data);
    }

    public static void Request_Server_Data()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Server_Data);
        Packet(Data);
    }

    public static void Write_Classes()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Classes);
        Packet(Data);
    }
    public static void Request_Classes()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Classes);
        Packet(Data);
    }

    public static void Write_Tiles()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Tiles);
        Packet(Data);
    }

    public static void Request_Tiles()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Tiles);
        Packet(Data);
    }

    public static void Write_Map(short Map_Num)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Maps);
        Data.Write(Map_Num);
        Packet(Data);
    }
    public static void Write_Maps()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Maps);
        Packet(Data);
    }
    public static void Request_Map(short Map_Num)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Map);
        Data.Write(Map_Num);
        Packet(Data);
    }

    public static void Request_Maps()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Maps);
        Packet(Data);
    }

    public static void Write_NPCs()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_NPCs);
        Packet(Data);
    }

    public static void Request_NPCs()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_NPCs);
        Packet(Data);
    }

    public static void Write_Items()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Items);
        Packet(Data);
    }

    public static void Request_Items()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Items);
        Packet(Data);
    }
}