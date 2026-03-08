using CryBits.Client.UI.Game;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class ChatHandler
{
    [PacketHandler]
    internal void Message(MessagePacket packet)
    {
        // Add chat message
        var text = packet.Text;
        var argb = packet.ColorArgb;
        Chat.AddText(text, new SFML.Graphics.Color(
            (byte)(argb >> 16),
            (byte)(argb >> 8),
            (byte)argb));
    }
}
