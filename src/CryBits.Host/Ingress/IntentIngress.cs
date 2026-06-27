using CryBits.Protocol;
using CryBits.Protocol.Packets.Client;
using CryBits.Protocol.Serialization;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using MemoryPack;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace CryBits.Host.Ingress;

public sealed class IntentIngress(IntentFunnel funnel, ILogger<IntentIngress> logger)
{
    [PacketHandler]
    public void Handle(EntityId entityId, IntentPacket packet)
    {
        var intentType = IntentRegistry.GetTypeForTag(packet.IntentTag);
        if (intentType is null)
        {
            logger.ZLogWarning($"Unknown intent tag {packet.IntentTag} from entity {entityId.Value}");
            return;
        }

        if (MemoryPackSerializer.Deserialize(intentType, packet.Data) is Intent intent)
        {
            funnel.Submit(intent with { SourceEntityId = entityId });
        }
        else
        {
            logger.ZLogError($"Failed to deserialize intent tag {packet.IntentTag} from entity {entityId.Value}");
        }
    }
}
