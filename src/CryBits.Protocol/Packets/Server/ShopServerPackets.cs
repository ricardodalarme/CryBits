using CryBits.Definitions.Shops;
using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class ShopsPacket : IServerPacket
{
    public Dictionary<Guid, Shop> List = [];
}

[MemoryPackable]
public partial class ShopOpenPacket : IServerPacket
{
    public Guid Id;
}
