using CryBits.Protocol;
using CryBits.Protocol.Packets.Client;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using MemoryPack;

namespace CryBits.Host.Ingress;

public sealed class IntentIngress(IntentFunnel funnel)
{
    [PacketHandler]
    public void Handle(EntityId entityId, IntentPacket packet)
    {
        var intentType = IntentRegistry.GetTypeForTag(packet.IntentTag);
        if (intentType is null) return;

        if (MemoryPackSerializer.Deserialize(intentType, packet.Data) is Intent intent)
        {
            funnel.Submit(intent with { SourceEntityId = entityId });
        }
    }
}
