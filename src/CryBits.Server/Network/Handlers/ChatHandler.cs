using CryBits.Definitions.Common;
using CryBits.Network;
using CryBits.Network.Packets.Client;
using CryBits.Server.World;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;

namespace CryBits.Server.Network.Handlers;

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

        GameWorld.Current.CurrentTick?.Intents.Enqueue(
            new ChatMessageIntent(entityId, message, (Message)packet.Type, packet.Addressee));
    }
}
