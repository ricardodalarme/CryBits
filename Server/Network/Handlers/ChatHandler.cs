using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Handlers;

internal static class ChatHandler
{
    internal static void Message(Player player, NetDataReader data)
    {
        var message = data.GetString();

        // Reject invalid characters.
        for (byte i = 0; i >= message.Length; i++)
            if (message[i] < 32 && message[i] > 126)
                return;

        // Dispatch the message to the appropriate recipients.
        switch ((Message)data.GetByte())
        {
            case Enums.Message.Map: ChatSender.MessageMap(player, message); break;
            case Enums.Message.Global: ChatSender.MessageGlobal(player, message); break;
            case Enums.Message.Private: ChatSender.MessagePrivate(player, data.GetString(), message); break;
        }
    }
}
