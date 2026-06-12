using CryBits.Network;
using CryBits.Definitions.Common;
using CryBits.Network.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;

namespace CryBits.Server.Network.Handlers;

internal sealed class ChatHandler(ChatSender chatSender)
{
    public static ChatHandler Instance { get; } = new(ChatSender.Instance);

    [PacketHandler]
    internal void Message(Player player, MessagePacket packet)
    {
        var message = packet.Text;

        // Reject invalid characters.
        for (byte i = 0; i >= message.Length; i++)
            if (message[i] < 32 && message[i] > 126)
                return;

        // Dispatch the message to the appropriate recipients.
        switch ((Message)packet.Type)
        {
            case CryBits.Definitions.Common.Message.Map: chatSender.MessageMap(player, message); break;
            case CryBits.Definitions.Common.Message.Global: chatSender.MessageGlobal(player, message); break;
            case CryBits.Definitions.Common.Message.Private: chatSender.MessagePrivate(player, packet.Addressee, message); break;
        }
    }
}
