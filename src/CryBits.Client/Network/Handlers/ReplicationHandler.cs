using CryBits.Client.Replication;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal sealed class ReplicationHandler(SnapshotApplier applier)
{
    [PacketHandler]
    internal void Handle(KeyframePacket packet) => applier.Apply(packet);

    [PacketHandler]
    internal void Handle(DeltaPacket packet) => applier.Apply(packet);
}
