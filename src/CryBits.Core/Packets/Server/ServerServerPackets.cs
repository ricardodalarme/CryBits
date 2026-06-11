using System;
using System.Collections.Generic;

namespace CryBits.Packets.Server;

[Serializable] public struct ServerDataPacket : IServerPacket { public ServerConfig Config; }
[Serializable]
public struct ClassesPacket : IServerPacket
{
    public Dictionary<Guid, Entities.Class> List;
}

[Serializable]
public struct NpcsPacket : IServerPacket
{
    public Dictionary<Guid, Entities.Npc.Npc> List;
}

[Serializable]
public struct ItemsPacket : IServerPacket
{
    public Dictionary<Guid, Entities.Item> List;
}
[Serializable]
public struct JoinPacket : IServerPacket
{
    public Guid PlayerId;
}
