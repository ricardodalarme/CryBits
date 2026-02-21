using System.Drawing;
using CryBits.Packets.Server;
using CryBits.Server.Entities;

namespace CryBits.Server.Network.Senders;

internal static class ChatSender
{
    public static void Message(Player player, string text, Color color)
    {
        Send.ToPlayer(player, new MessagePacket { Text = text, ColorArgb = color.ToArgb() });
    }

    public static void MessageMap(Player player, string text)
    {
        var message = "[Map] " + player.Name + ": " + text;
        Send.ToMap(player.Map, new MessagePacket { Text = message, ColorArgb = Color.White.ToArgb() });
    }

    public static void MessageGlobal(Player player, string text)
    {
        var message = "[Global] " + player.Name + ": " + text;
        Send.ToAll(new MessagePacket { Text = message, ColorArgb = Color.Yellow.ToArgb() });
    }

    public static void MessagePrivate(Player player, string addresseeName, string text)
    {
        var addressee = Player.Find(addresseeName);

        // Check if the addressee is connected.
        if (addressee == null)
        {
            Message(player, addresseeName + " is currently offline.", Color.Blue);
            return;
        }

        // Send private messages.
        Message(player, "[To] " + addresseeName + ": " + text, Color.Pink);
        Message(addressee, "[From] " + player.Name + ": " + text, Color.Pink);
    }
}
