using CryBits.Client.Replication;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal sealed class KeyframeHandler(SnapshotApplier applier)
{
    [PacketHandler]
    internal void Handle(KeyframePacket packet)
    {
        applier.Apply(packet);
    }
}
