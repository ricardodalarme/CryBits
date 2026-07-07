using CryBits.Host.Core;
using CryBits.Protocol.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using System.Drawing;

namespace CryBits.Host.Network.Senders;

public sealed class ChatSender(PackageSender packageSender, EntityRegistry entities)
{
    public void Message(EntityId entityId, string text, Color color)
    {
        packageSender.ToPlayer(entityId, new MessagePacket { Text = text, ColorArgb = color.ToArgb() });
    }

    public void SendMessage(Session session, string text, int colorArgb)
    {
        packageSender.ToPlayer(session, new MessagePacket { Text = text, ColorArgb = colorArgb });
    }

    public void MessageMap(EntityId entityId, string text)
    {
        var appearance = entities.Get<PlayerAppearance>(entityId)!;
        var pos = entities.Get<Position>(entityId)!;
        var message = "[Map] " + appearance.Name + ": " + text;
        packageSender.ToMap(pos.MapId, new MessagePacket { Text = message, ColorArgb = Color.White.ToArgb() });
    }

    public void MessageGlobal(EntityId entityId, string text)
    {
        var appearance = entities.Get<PlayerAppearance>(entityId)!;
        var message = "[Global] " + appearance.Name + ": " + text;
        packageSender.ToAll(new MessagePacket { Text = message, ColorArgb = Color.Yellow.ToArgb() });
    }

    public void MessagePrivate(EntityId entityId, string addresseeName, string text, WorldHost host)
    {
        var addressee = host.FindPlayer(addresseeName);

        if (addressee == null)
        {
            Message(entityId, addresseeName + " is currently offline.", Color.Blue);
            return;
        }

        var appearance = entities.Get<PlayerAppearance>(entityId)!;
        Message(entityId, "[To] " + addresseeName + ": " + text, Color.Pink);
        Message(addressee.Value, "[From] " + appearance.Name + ": " + text, Color.Pink);
    }
}
