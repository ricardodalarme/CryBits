using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using MemoryPack;

namespace CryBits.Protocol.Packets.Client;

[MemoryPackable]
public partial class WriteClassesPacket : IClientPacket
{
    public Dictionary<Guid, Class> Classes = [];
}

[MemoryPackable]
public partial class WriteMapsPacket : IClientPacket
{
    public Dictionary<Guid, Map> Maps = [];
}

[MemoryPackable]
public partial class WriteNpcsPacket : IClientPacket
{
    public Dictionary<Guid, Npc> Npcs = [];
}

[MemoryPackable]
public partial class WriteItemsPacket : IClientPacket
{
    public Dictionary<Guid, Item> Items = [];
}

[MemoryPackable]
public partial class WriteShopsPacket : IClientPacket
{
    public Dictionary<Guid, Shop> Shops = [];
}

[MemoryPackable]
public partial class RequestClassesPacket : IClientPacket;

[MemoryPackable]
public partial class RequestMapPacket : IClientPacket
{
    public Guid Id;
    public bool SendMap;
}

[MemoryPackable]
public partial class RequestMapsPacket : IClientPacket;

[MemoryPackable]
public partial class RequestNpcsPacket : IClientPacket;

[MemoryPackable]
public partial class RequestItemsPacket : IClientPacket;

[MemoryPackable]
public partial class RequestShopsPacket : IClientPacket;
