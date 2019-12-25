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

    public static void Request_Server_Data()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Server_Data);
        Packet(Data);
    }

    public static void Request_Classes(bool OpenEditor = false)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Classes);
        Data.Write(OpenEditor);
        Packet(Data);
    }

    public static void Request_Tiles(bool OpenEditor = false)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Tiles);
        Data.Write(OpenEditor);
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

    public static void Request_Maps(bool OpenEditor = false)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Maps);
        Data.Write(OpenEditor);
        Packet(Data);
    }

    public static void Request_NPCs(bool OpenEditor = false)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_NPCs);
        Data.Write(OpenEditor);
        Packet(Data);
    }

    public static void Request_Items(bool OpenEditor = false)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Request_Items);
        Data.Write(OpenEditor);
        Packet(Data);
    }

    public static void Write_Server_Data()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Server_Data);
        Packet(Data);
    }

    public static void Write_Classes()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Classes);
        Packet(Data);
    }

    public static void Write_Tiles()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Tiles);
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

    public static void Write_NPCs()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_NPCs);
        Data.Write((short)Lists.NPC.GetUpperBound(0));
        for (short Index = 1; Index < Lists.NPC.Length; Index++)
        {
            Data.Write(Lists.NPC[Index].Name);
            Data.Write(Lists.NPC[Index].Texture);
            Data.Write(Lists.NPC[Index].Behaviour);
            Data.Write(Lists.NPC[Index].SpawnTime);
            Data.Write(Lists.NPC[Index].Sight);
            Data.Write(Lists.NPC[Index].Experience);
            for (byte i = 0; i < (byte)Globals.Vitals.Amount; i++) Data.Write(Lists.NPC[Index].Vital[i]);
            for (byte i = 0; i < (byte)Globals.Attributes.Amount; i++) Data.Write(Lists.NPC[Index].Attribute[i]);
            for (byte i = 0; i < Globals.Max_NPC_Drop; i++)
            {
                Data.Write(Lists.NPC[Index].Drop[i].Item_Num);
                Data.Write(Lists.NPC[Index].Drop[i].Amount);
                Data.Write(Lists.NPC[Index].Drop[i].Chance);
            }
        }
        Packet(Data);
    }

    public static void Write_Items()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Write_Items);
        Data.Write((short)Lists.Item.GetUpperBound(0));
        for (short Index = 1; Index < Lists.Item.Length; Index++)
        {
            Data.Write(Lists.Item[Index].Name);
            Data.Write(Lists.Item[Index].Description);
            Data.Write(Lists.Item[Index].Texture);
            Data.Write(Lists.Item[Index].Type);
            Data.Write(Lists.Item[Index].Price);
            Data.Write(Lists.Item[Index].Stackable);
            Data.Write(Lists.Item[Index].Bind);
            Data.Write(Lists.Item[Index].Req_Level);
            Data.Write(Lists.Item[Index].Req_Class);
            Data.Write(Lists.Item[Index].Potion_Experience);
            for (byte i = 0; i < (byte)Globals.Vitals.Amount; i++) Data.Write(Lists.Item[Index].Potion_Vital[i]);
            Data.Write(Lists.Item[Index].Equip_Type);
            for (byte i = 0; i < (byte)Globals.Attributes.Amount; i++) Data.Write(Lists.Item[Index].Equip_Attribute[i]);
            Data.Write(Lists.Item[Index].Weapon_Damage);
        }
        Packet(Data);
    }
}