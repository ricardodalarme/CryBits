using MemoryPack;

namespace CryBits.Protocol.Packets.Client;

[MemoryPackable]
public partial class IntentPacket : IClientPacket
{
    public byte IntentTag;
    public byte[] Data = [];
}
