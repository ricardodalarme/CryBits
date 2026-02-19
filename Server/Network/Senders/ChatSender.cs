using System.Drawing;
using CryBits.Enums;
using CryBits.Server.Entities;
using LiteNetLib.Utils;

namespace CryBits.Server.Network.Senders;

internal static class ChatSender
{
    public static void Message(Player player, string text, Color color)
    {
        var data = new NetDataWriter();

        data.Put((byte)ServerPacket.Message);
        data.Put(text);
        data.Put(color.ToArgb());
        Send.ToPlayer(player, data);
    }

    public static void MessageMap(Player player, string text)
    {
        var data = new NetDataWriter();
        var message = "[Map] " + player.Name + ": " + text;

        data.Put((byte)ServerPacket.Message);
        data.Put(message);
        data.Put(Color.White.ToArgb());
        Send.ToMap(player.Map, data);
    }

    public static void MessageGlobal(Player player, string text)
    {
        var data = new NetDataWriter();
        var message = "[Global] " + player.Name + ": " + text;

        data.Put((byte)ServerPacket.Message);
        data.Put(message);
        data.Put(Color.Yellow.ToArgb());
        Send.ToAll(data);
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
