using CryBits.Definitions.Common;
using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Core;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Host.Services;

internal sealed class ChatService(WorldHost host)
{
    [PacketHandler]
    internal void Message(EntityId entityId, MessagePacket packet)
    {
        var message = packet.Text;

        for (byte i = 0; i < message.Length; i++)
            if (message[i] < 32 && message[i] > 126)
                return;

        host.CurrentTick?.Intents.Enqueue(
            new ChatMessageIntent(entityId, message, (Message)packet.Type, packet.Addressee));
    }
}
