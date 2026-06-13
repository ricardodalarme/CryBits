using CryBits.Definitions.Maps;
using System;
using System.Collections.Generic;

namespace CryBits.Network.Packets.Server;

[Serializable]
public struct MapsPacket : IServerPacket
{
    public Dictionary<Guid, Map> List;
}

[Serializable]
public struct MapPacket : IServerPacket
{
    public Map Map;
}

[Serializable] public struct JoinMapPacket : IServerPacket;

[Serializable]
public struct MapRevisionPacket : IServerPacket
{
    public Guid MapId;
    public short Revision;
}

[Serializable]
public struct MapNpcsPacket : IServerPacket
{
    public PacketsMapNpc[] Npcs;
}

[Serializable]
public struct MapNpcPacket : IServerPacket
{
    public long InstanceId;
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital;
}

[Serializable]
public struct MapNpcMovementPacket : IServerPacket
{
    public long InstanceId;
    public byte X, Y;
    public byte Direction;
    public byte Movement;
    public float Speed;
}

[Serializable]
public struct MapNpcDirectionPacket : IServerPacket
{
    public long InstanceId;
    public byte Direction;
}

[Serializable]
public struct MapNpcVitalsPacket : IServerPacket
{
    public long InstanceId;
    public short[] Vital;
}

[Serializable]
public struct MapNpcDiedPacket : IServerPacket
{
    public long InstanceId;
}

[Serializable]
public struct MapItemsPacket : IServerPacket
{
    public PacketsMapItem[] Items;
}

[Serializable]
public struct PacketsMapItem
{
    public Guid ItemId;
    public byte X, Y;
}

[Serializable]
public struct PacketsMapNpc
{
    public long InstanceId;
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital;
}
