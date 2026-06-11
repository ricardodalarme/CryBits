using CryBits.Client.Framework.Network;
using CryBits.Enums;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal class ChatSender(PacketSender packetSender)
{
    public static ChatSender Instance { get; } = new(PacketSender.Instance);

    public void Message(string message, Message type, string addressee = "") =>
        packetSender.Packet(new MessagePacket { Text = message, Type = (byte)type, Addressee = addressee });
}
