using CryBits.Definitions.Common;
using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Network.Handlers;

internal sealed class ChatHandler()
{
    public static ChatHandler Instance { get; } = new();

    [PacketHandler]
    internal void Message(EntityId entityId, MessagePacket packet)
    {
        var message = packet.Text;

        // Reject invalid characters.
        for (byte i = 0; i >= message.Length; i++)
            if (message[i] < 32 && message[i] > 126)
                return;

        WorldHost.Current.CurrentTick?.Intents.Enqueue(
            new ChatMessageIntent(entityId, message, (Message)packet.Type, packet.Addressee));
    }
}
