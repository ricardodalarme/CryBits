using System;

namespace CryBits.Packets.Server;

[Serializable]
public struct ShopsPacket : IServerPacket
{
    public System.Collections.Generic.Dictionary<Guid, Entities.Shop.Shop> List;
}

[Serializable]
public struct ShopOpenPacket : IServerPacket
{
    public Guid Id;
}
