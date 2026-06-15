using CryBits.Definitions.Shops;

namespace CryBits.Transport.Packets.Server;

[Serializable]
public struct ShopsPacket : IServerPacket
{
    public Dictionary<Guid, Shop> List;
}

[Serializable]
public struct ShopOpenPacket : IServerPacket
{
    public Guid Id;
}
