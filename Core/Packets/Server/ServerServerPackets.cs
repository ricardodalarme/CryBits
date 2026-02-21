using System;

namespace CryBits.Packets.Server;

[Serializable] public struct ServerDataPacket : IServerPacket { public ServerConfig Config; }
[Serializable]
public struct ClassesPacket : IServerPacket
{
    public System.Collections.Generic.Dictionary<Guid, Entities.Class> List;
}

[Serializable]
public struct NpcsPacket : IServerPacket
{
    public System.Collections.Generic.Dictionary<Guid, Entities.Npc.Npc> List;
}

[Serializable]
public struct ItemsPacket : IServerPacket
{
    public System.Collections.Generic.Dictionary<Guid, Entities.Item> List;
}
[Serializable]
public struct JoinPacket : IServerPacket
{
    public string Name;
}
