using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class ChatSender
{
    public static void Message(string message, Message type, string addressee = "") =>
        Send.Packet(new MessagePacket { Text = message, Type = (byte)type, Addressee = addressee });
}
