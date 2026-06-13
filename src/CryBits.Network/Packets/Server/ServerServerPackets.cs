using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Npcs;
using System;
using System.Collections.Generic;

namespace CryBits.Network.Packets.Server;

[Serializable]
public struct ClassesPacket : IServerPacket
{
    public Dictionary<Guid, Class> List;
}

[Serializable]
public struct NpcsPacket : IServerPacket
{
    public Dictionary<Guid, Npc> List;
}

[Serializable]
public struct ItemsPacket : IServerPacket
{
    public Dictionary<Guid, Item> List;
}
[Serializable]
public struct JoinPacket : IServerPacket
{
    public long PlayerId;
}
