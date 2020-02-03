using Lidgren.Network;
using System;

partial class Send
{
    // Pacotes do cliente
    public enum Packets
    {
        Connect,
        Latency,
        Register,
        CreateCharacter,
        Character_Use,
        Character_Create,
        Character_Delete,
        Player_Direction,
        Player_Move,
        Player_Attack,
        RequestMap,
        Message,
        AddPoint,
        CollectItem,
        DropItem,
        Inventory_Change,
        Inventory_Use,
        Equipment_Remove,
        Hotbar_Add,
        Hotbar_Change,
        Hotbar_Use
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
        Data.Write(TextBoxes.Get("Connect_Username").Text);
        Data.Write(TextBoxes.Get("Connect_Password").Text);
        Data.Write(false); // Acesso pelo cliente
        Packet(Data);
    }

    public static void Register()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Register);
        Data.Write(TextBoxes.Get("Register_Username").Text);
        Data.Write(TextBoxes.Get("Register_Password").Text);
        Packet(Data);
    }

    public static void CreateCharacter()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.CreateCharacter);
        Data.Write(TextBoxes.Get("CreateCharacter_Name").Text);
        Data.Write(Game.CreateCharacter_Class);
        Data.Write(CheckBoxes.Get("GenderMale").Checked);
        Data.Write(Game.CreateCharacter_Tex);
        Packet(Data);
    }

    public static void Character_Use()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Character_Use);
        Data.Write(Game.SelectCharacter);
        Packet(Data);
    }

    public static void Character_Create()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Character_Create);
        Packet(Data);
    }

    public static void Character_Delete()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Character_Delete);
        Data.Write(Game.SelectCharacter);
        Packet(Data);
    }

    public static void RequestMap(bool Order)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.RequestMap);
        Data.Write(Order);
        Packet(Data);
    }

    public static void Latency()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Latency);
        Packet(Data);

        // Define a contaem na hora do envio
        Game.Latency_Send = Environment.TickCount;
    }

    public static void Message(string Message, Game.Messages Type, string Addressee = "")
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Message);
        Data.Write(Message);
        Data.Write((byte)Type);
        Data.Write(Addressee);
        Packet(Data);
    }

    public static void AddPoint(Game.Attributes Attribute)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.AddPoint);
        Data.Write((byte)Attribute);
        Packet(Data);
    }

    public static void CollectItem()
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.CollectItem);
        Packet(Data);
    }

    public static void DropItem(byte Slot, short Amount)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.DropItem);
        Data.Write(Slot);
        Data.Write(Amount);
        Packet(Data);
    }

    public static void Inventory_Change(byte Old, byte New)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Inventory_Change);
        Data.Write(Old);
        Data.Write(New);
        Packet(Data);

        // Fecha o painel de soltar item
        Panels.Get("Drop").Visible = false;
    }

    public static void Inventory_Use(byte Slot)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Inventory_Use);
        Data.Write(Slot);
        Packet(Data);

        // Fecha o painel de soltar item
        Panels.Get("Drop").Visible = false;
    }

    public static void Equipment_Remove(byte Slot)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Equipment_Remove);
        Data.Write(Slot);
        Packet(Data);
    }

    public static void Hotbar_Add(byte Hotbar_Slot, byte Type, byte Slot)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Hotbar_Add);
        Data.Write(Hotbar_Slot);
        Data.Write(Type);
        Data.Write(Slot);
        Packet(Data);
    }

    public static void Hotbar_Change(byte Old, byte New)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Hotbar_Change);
        Data.Write(Old);
        Data.Write(New);
        Packet(Data);
    }

    public static void Hotbar_Use(byte Slot)
    {
        NetOutgoingMessage Data = Socket.Device.CreateMessage();

        // Envia os dados
        Data.Write((byte)Packets.Hotbar_Use);
        Data.Write(Slot);
        Packet(Data);

        // Fecha o painel de soltar item
        Panels.Get("Drop").Visible = false;
    }
}