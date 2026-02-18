using CryBits.Enums;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Senders;

internal static class ChatSender
{
    public static void Message(string message, Message type, string addressee = "")
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.Message);
        data.Put(message);
        data.Put((byte)type);
        data.Put(addressee);
        Send.Packet(data);
    }
}
