using MemoryPack;

namespace CryBits.Protocol.Packets.Client;

[MemoryPackable]
public partial class AckPacket : IClientPacket
{
    public long LastReceivedTick;
}
