using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class ShopOpenPacket : IServerPacket
{
    public Guid Id;
}
