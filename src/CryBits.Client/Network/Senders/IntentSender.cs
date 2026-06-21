using CryBits.Client.Framework.Network;
using CryBits.Protocol.Packets.Client;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Intents;
using CryBits.Transport;
using MemoryPack;

namespace CryBits.Client.Network.Senders;

internal class IntentSender(Connection connection)
{
    public static IntentSender Instance { get; } = new(Connection.Instance);

    public void Send(Intent intent)
    {
        var tag = IntentRegistry.GetTag(intent.GetType());
        if (tag == null) return;
        var data = MemoryPackSerializer.Serialize(intent.GetType(), intent);
        connection.SendPacket(
            new IntentPacket { IntentTag = tag.Value, Data = data },
            DeliveryChannel.Sequenced);
    }
}
