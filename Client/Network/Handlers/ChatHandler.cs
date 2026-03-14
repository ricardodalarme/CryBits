using System.Drawing;
using CryBits.Client.UI.Game;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ChatHandler(Chat chat)
{
    [PacketHandler]
    internal void Message(MessagePacket packet)
    {
        // Add chat message
        var text = packet.Text;
        var color = Color.FromArgb(packet.ColorArgb);
        chat.AddText(text, new SFML.Graphics.Color(color.R, color.G, color.B));
    }
}
