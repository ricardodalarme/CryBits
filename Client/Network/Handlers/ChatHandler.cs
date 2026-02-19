using System.Drawing;
using CryBits.Client.UI;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Handlers;

internal static class ChatHandler
{
    internal static void Message(NetDataReader data)
    {
        // Add chat message
        var text = data.GetString();
        var color = Color.FromArgb(data.GetInt());
        Chat.AddText(text, new SFML.Graphics.Color(color.R, color.G, color.B));
    }
}
