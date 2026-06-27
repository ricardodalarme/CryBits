using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using MemoryPack;

namespace CryBits.Protocol.Packets.Server;

[MemoryPackable]
public partial class MapPacket : IServerPacket
{
    public Map Map = new();
}

[MemoryPackable]
public partial class MapsPacket : IServerPacket
{
    public Dictionary<Guid, Map> List = [];
}

[MemoryPackable]
public partial class MapRevisionPacket : IServerPacket
{
    public Guid MapId;
}

[MemoryPackable]
public partial class ClassesPacket : IServerPacket
{
    public Dictionary<Guid, Class> List = [];
}

[MemoryPackable]
public partial class NpcsPacket : IServerPacket
{
    public Dictionary<Guid, Npc> List = [];
}

[MemoryPackable]
public partial class ItemsPacket : IServerPacket
{
    public Dictionary<Guid, Item> List = [];
}

[MemoryPackable]
public partial class ShopsPacket : IServerPacket
{
    public Dictionary<Guid, Shop> List = [];
}
