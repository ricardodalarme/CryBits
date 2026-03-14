using System;
using System.Collections.Generic;

namespace CryBits.Packets.Server;

[Serializable]
public struct MapsPacket : IServerPacket
{
    public Dictionary<Guid, Entities.Map.Map> List;
}

[Serializable]
public struct MapPacket : IServerPacket
{
    public Entities.Map.Map Map;
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
    public Guid InstanceId;
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital;
}

[Serializable]
public struct MapNpcMovementPacket : IServerPacket
{
    public Guid InstanceId;
    public byte X, Y;
    public byte Direction;
    public byte Movement;
    public float Speed;
}

[Serializable]
public struct MapNpcDirectionPacket : IServerPacket
{
    public Guid InstanceId;
    public byte Direction;
}

[Serializable]
public struct MapNpcVitalsPacket : IServerPacket
{
    public Guid InstanceId;
    public short[] Vital;
}

[Serializable]
public struct MapNpcAttackPacket : IServerPacket
{
    public Guid AttackerId;
    public Guid VictimId;
}

[Serializable]
public struct MapNpcDiedPacket : IServerPacket
{
    public Guid InstanceId;
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
    public Guid InstanceId;
    public Guid NpcId;
    public byte X, Y;
    public byte Direction;
    public short[] Vital;
}
