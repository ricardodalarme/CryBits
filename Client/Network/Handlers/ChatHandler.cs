using System.Drawing;
using CryBits.Client.UI;
using CryBits.Enums;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class ChatHandler
{
    [PacketHandler(ServerPacket.Message)]
    internal static void Message(MessagePacket packet)
    {
        // Add chat message
        var text = packet.Text;
        var color = Color.FromArgb(packet.ColorArgb);
        Chat.AddText(text, new SFML.Graphics.Color(color.R, color.G, color.B));
    }
}
