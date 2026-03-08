using System.Drawing;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.Network;

namespace CryBits.Server.Network.Senders;

internal sealed class ChatSender(PackageSender packageSender)
{
    public static ChatSender Instance { get; } = new(PackageSender.Instance);

    private readonly PackageSender _packageSender = packageSender;

    public void Message(Player player, string text, Color color)
    {
        _packageSender.ToPlayer(player, new MessagePacket { Text = text, ColorArgb = color.ToArgb() });
    }

    public void MessageMap(Player player, string text)
    {
        var message = "[Map] " + player.Name + ": " + text;
        _packageSender.ToMap(player.MapInstance, new MessagePacket { Text = message, ColorArgb = Color.White.ToArgb() });
    }

    public void MessageGlobal(Player player, string text)
    {
        var message = "[Global] " + player.Name + ": " + text;
        _packageSender.ToAll(new MessagePacket { Text = message, ColorArgb = Color.Yellow.ToArgb() });
    }

    public void MessagePrivate(Player player, string addresseeName, string text)
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
