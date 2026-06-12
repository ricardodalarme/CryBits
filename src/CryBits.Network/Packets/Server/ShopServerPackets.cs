using CryBits.Definitions.Shops;
using System;
using System.Collections.Generic;

namespace CryBits.Network.Packets.Server;

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
