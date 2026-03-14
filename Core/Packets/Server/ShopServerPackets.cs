using System;
using System.Collections.Generic;

namespace CryBits.Packets.Server;

[Serializable]
public struct ShopsPacket : IServerPacket
{
    public Dictionary<Guid, Entities.Shop.Shop> List;
}

[Serializable]
public struct ShopOpenPacket : IServerPacket
{
    public Guid Id;
}
