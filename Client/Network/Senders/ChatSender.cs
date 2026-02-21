using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class ChatSender
{
    public static void Message(string message, Message type, string addressee = "") =>
        Send.Packet(ClientPacket.Message, new MessagePacket { Text = message, Type = (byte)type, Addressee = addressee });
}
