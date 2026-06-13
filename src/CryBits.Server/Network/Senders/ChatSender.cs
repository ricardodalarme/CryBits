using CryBits.Network.Packets.Server;
using CryBits.Server.Simulation.State;
using CryBits.Server.Simulation.State.Components;
using CryBits.Server.World;
using System.Drawing;

namespace CryBits.Server.Network.Senders;

internal sealed class ChatSender(PackageSender packageSender)
{
    public static ChatSender Instance { get; } = new(PackageSender.Instance);

    public void Message(EntityId entityId, string text, Color color)
    {
        packageSender.ToPlayer(entityId, new MessagePacket { Text = text, ColorArgb = color.ToArgb() });
    }

    public void MessageMap(EntityId entityId, string text)
    {
        var appearance = GameWorld.Current.Entities.Get(entityId)!.Get<PlayerAppearance>()!;
        var pos = GameWorld.Current.Entities.Get(entityId)!.Get<Position>()!;
        var message = "[Map] " + appearance.Name + ": " + text;
        packageSender.ToMap(pos.MapId, new MessagePacket { Text = message, ColorArgb = Color.White.ToArgb() });
    }

    public void MessageGlobal(EntityId entityId, string text)
    {
        var appearance = GameWorld.Current.Entities.Get(entityId)!.Get<PlayerAppearance>()!;
        var message = "[Global] " + appearance.Name + ": " + text;
        packageSender.ToAll(new MessagePacket { Text = message, ColorArgb = Color.Yellow.ToArgb() });
    }

    public void MessagePrivate(EntityId entityId, string addresseeName, string text)
    {
        var addressee = GameWorld.Current.FindPlayer(addresseeName);

        // Check if the addressee is connected.
        if (addressee == null)
        {
            Message(entityId, addresseeName + " is currently offline.", Color.Blue);
            return;
        }

        // Send private messages.
        var appearance = GameWorld.Current.Entities.Get(entityId)!.Get<PlayerAppearance>()!;
        Message(entityId, "[To] " + addresseeName + ": " + text, Color.Pink);
        Message(addressee.Value, "[From] " + appearance.Name + ": " + text, Color.Pink);
    }
}
