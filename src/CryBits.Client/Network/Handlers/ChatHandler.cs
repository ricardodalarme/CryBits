using CryBits.Client.UI.Game;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;
using Microsoft.Xna.Framework;

namespace CryBits.Client.Network.Handlers;

internal class ChatHandler(Chat chat)
{
    [PacketHandler]
    internal void Message(MessagePacket packet)
    {
        // Add chat message
        var text = packet.Text;
        chat.AddText(text, new Color((uint)packet.ColorArgb));
    }
}
